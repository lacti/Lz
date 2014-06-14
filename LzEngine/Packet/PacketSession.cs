using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LzEngine.Util;

namespace LzEngine.Packet
{
    public class PacketSession
    {
        private readonly SinglyAccessQueue<byte[]> _sendQueue;
        private readonly Socket _socket;

        public PacketSession()
            : this(new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
        {
        }

        public PacketSession(Socket socket)
        {
            _socket = socket;
            _socket.NoDelay = true;
            _sendQueue = new SinglyAccessQueue<byte[]>(this, ProcessSend);
        }

        public void Connect(string host, int port)
        {
            _socket.Connect(host, port);
        }

        #region TypeIndex

        private static readonly Dictionary<ushort, Type> IndexPacketTypeMap = new Dictionary<ushort, Type>();
        private static readonly Dictionary<Type, ushort> PacketTypeIndexMap = new Dictionary<Type, ushort>();
        private static readonly Type PacketInterfaceType = typeof (IPacket);

        static PacketSession()
        {
            var packetIndex = (ushort)0;
            var packetTypes =
                Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .Where(e => e != PacketInterfaceType && PacketInterfaceType.IsAssignableFrom(e))
                        .OrderBy(e => e.Name);

            foreach (var type in packetTypes)
            {
                IndexPacketTypeMap.Add(packetIndex, type);
                PacketTypeIndexMap.Add(type, packetIndex);
                ++packetIndex;
            }
        }

        #endregion

        #region Send & Receive

        public void Send(IPacket packet)
        {
            using (var stream = new MemoryStream())
            {
                var writer = new BinaryWriter(stream);

                // length 기록할 부분 확보
                writer.Write((ushort)0);

                // packet 기록
                WritePacket(writer, packet);

                var bytes = stream.ToArray();

                // length 기록
                var length = (ushort)(bytes.Length - sizeof(short));
                var lengthBytes = BitConverter.GetBytes(length);
                Array.Copy(lengthBytes, 0, bytes, 0, lengthBytes.Length);

                Send(bytes);
            }
        }

        private void Send(byte[] bytes)
        {
            _sendQueue.EnqueueOrProcess(bytes);
        }

        private async void ProcessSend(byte[] bytes)
        {
            try
            {
                await _socket.SendAsync(bytes);
            }
            catch
            {
            }
        }

        public async Task<IPacket> Receive()
        {
            var lengthBytes = await _socket.ReceiveAsync(sizeof (ushort));
            var length = BitConverter.ToUInt16(lengthBytes, 0);
            var dataBytes = await _socket.ReceiveAsync(length);
            using (var stream = new MemoryStream(dataBytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var packet = ReadPacket(reader);
                    if (packet == null)
                        throw new NullReferenceException();

                    return packet;
                }
            }
        }

        #endregion

        #region Byte Serialize & Deserialize

        private static void WritePacket(BinaryWriter writer, IPacket packet)
        {
            var packetType = packet.GetType();
            writer.Write(PacketTypeIndexMap[packetType]);

            foreach (var eachProperty in packetType.GetProperties().Where(e => e.CanRead && e.CanWrite))
            {
                var propertyType = eachProperty.PropertyType;
                var propertyValue = eachProperty.GetValue(packet, null);

                WriteValue(writer, propertyType, propertyValue);
            }
        }
        
        private static void WriteValue(BinaryWriter writer, Type valueType, object value)
        {
            if (valueType.IsPrimitive)
            {
                writer.Write(valueType, value);
            }
            else if (valueType == typeof(byte[]))
            {
                var bytesValue = (byte[])value;
                writer.Write((ushort)bytesValue.Length);
                writer.Write(bytesValue);
            }
            else if (valueType == typeof(string))
            {
                if (value == null)
                    writer.Write((ushort)0);
                else
                {
                    var stringValue = (string)value;
                    var stringBytes = Encoding.UTF8.GetBytes(stringValue);
                    writer.Write((ushort)stringBytes.Length);
                    writer.Write(stringBytes);
                }
            }
            else if (PacketInterfaceType.IsAssignableFrom(valueType))
            {
                var childPacket = (IPacket)value;
                WritePacket(writer, childPacket);
            }
            else if (typeof(IList).IsAssignableFrom(valueType))
            {
                var list = (IList)value;
                writer.Write((ushort)list.Count);
                var elementType = valueType.GetGenericArguments()[0];
                foreach (var child in list)
                {
                    WriteValue(writer, elementType, child);
                }
            }
            else if (valueType.IsEnum)
            {
                writer.Write((byte)(int)value);
            }
            else if (valueType == typeof (Point))
            {
                var point = (Point)value;
                writer.Write(point.X);
                writer.Write(point.Y);
            }
            else throw new InvalidOperationException("Invalid Type: " + valueType);
        }

        public static IPacket ReadPacket(BinaryReader reader)
        {
            var packetIndex = reader.ReadUInt16();
            if (!IndexPacketTypeMap.ContainsKey(packetIndex))
                return null;

            var packetType = IndexPacketTypeMap[packetIndex];
            var packet = (IPacket)Activator.CreateInstance(packetType);

            ReadPacket(reader, packet);
            return packet;
        }

        private static void ReadPacket(BinaryReader reader, IPacket packet)
        {
            var packetType = packet.GetType();
            foreach (var eachProperty in packetType.GetProperties().Where(e => e.CanRead && e.CanWrite))
            {
                eachProperty.SetValue(packet, ReadValue(reader, eachProperty.PropertyType), null);
            }
        }

        private static object ReadValue(BinaryReader reader, Type valueType)
        {
            if (valueType.IsPrimitive)
            {
                return reader.Read(valueType);
            }

            if (valueType == typeof(byte[]))
            {
                var length = reader.ReadUInt16();
                var bytes = reader.ReadBytes(length);

                return bytes;
            }

            if (valueType == typeof(string))
            {
                var length = reader.ReadUInt16();
                var stringBytes = reader.ReadBytes(length);
                var value = Encoding.UTF8.GetString(stringBytes);

                return value;
            }

            if (PacketInterfaceType.IsAssignableFrom(valueType))
            {
                return ReadPacket(reader);
            }

            if (typeof(IList).IsAssignableFrom(valueType))
            {
                var list = (IList)Activator.CreateInstance(valueType);
                var count = reader.ReadUInt16();
                var elementType = valueType.GetGenericArguments()[0];
                foreach (
                    var child in
                        Enumerable.Range(0, count)
                                  .Select(_ => ReadValue(reader, elementType))
                                  .Where(e => e != null))
                {
                    list.Add(child);
                }

                return list;
            }

            if (valueType.IsEnum)
            {
                return System.Enum.ToObject(valueType, (int)reader.ReadByte());
            }

            if (valueType == typeof(Point))
            {
                var x = reader.ReadInt32();
                var y = reader.ReadInt32();
                return new Point(x, y);
            }

            throw new InvalidOperationException("Invalid Type: " + valueType);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using LzEngine.Util;

namespace LzEngine.Packet
{
    public static class PacketSocketExtension
    {
        public static void SendPacket(this Socket socket, PacketBase packet)
        {
            SendPacket(new[] {socket}, packet);
        }

        public static void SendPacket(this IEnumerable<Socket> sockets,
                                      PacketBase packet)
        {
            using (var stream = new MemoryStream())
            {
                var lengthWriter = new BinaryWriter(stream);
                lengthWriter.Write(0);

                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, packet);

                lengthWriter.Seek(0, SeekOrigin.Begin);
                lengthWriter.Write((int) (stream.Length - sizeof (int)));

                var bytes = stream.ToArray();
                try
                {
                    var tasks =
                        sockets.Select(socket => socket.SendAsync(bytes))
                               .Cast<Task>()
                               .ToList();
                    Task.WaitAll(tasks.ToArray());
                }
                catch
                {
                }
            }
        }

        public static async Task<PacketBase> ReceivePacket(this Socket socket)
        {
            var lengthBytes = await socket.ReceiveAsync(sizeof (int));
            var length = BitConverter.ToInt32(lengthBytes, 0);
            var dataBytes = await socket.ReceiveAsync(length);
            using (var stream = new MemoryStream(dataBytes))
            {
                var formatter = new BinaryFormatter();
                var packet = formatter.Deserialize(stream) as PacketBase;
                if (packet == null)
                    throw new NullReferenceException();

                return packet;
            }
        }
    }
}
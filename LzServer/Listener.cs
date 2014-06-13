using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using LzEngine.Packet;
using LzEngine.Util;

namespace LzServer
{
    public class Listener : PacketDispatcher
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        private readonly List<Socket> _sockets = new List<Socket>();

        public int Port { get; set; }

        public async void Start()
        {
            try
            {
                var listener = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream, ProtocolType.Tcp);
                var localEndPoint = new IPEndPoint(IPAddress.Any, Port);

                listener.Bind(localEndPoint);
                listener.Listen(100);

                while (true)
                {
                    var clientSocket = await listener.AcceptAsync();
                    ProcessSocket(clientSocket);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }
        }

        public void BroadcastPacket(PacketBase packet, Socket exceptSocket = null)
        {
            _lock.DoReadLock(() =>
                {
                    if (exceptSocket != null)
                        _sockets.Except(new[] {exceptSocket}).SendPacket(packet);
                    else _sockets.SendPacket(packet);
                });
        }

        private async void ProcessSocket(Socket socket)
        {
            Logger.Write("Connected from: " + socket.RemoteEndPoint);

            _lock.DoWriteLock(() => _sockets.Add(socket));
            Dispatch(new ConnectionPacket {Connected = true}, socket);
            try
            {
                while (true)
                {
                    var packet = await socket.ReceivePacket();
                    Dispatch(packet, socket);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }

            Dispatch(new ConnectionPacket {Connected = false}, socket);
            _lock.DoWriteLock(() => _sockets.Remove(socket));

            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
        }
    }
}
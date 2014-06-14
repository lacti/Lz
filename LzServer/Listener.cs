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
        private readonly List<PacketSession> _sessions = new List<PacketSession>();

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

        public void BroadcastPacket(IPacket packet, PacketSession exceptSession = null)
        {
            _lock.DoReadLock(() =>
                {
                    var targetSessions = exceptSession != null ? _sessions.Where(e => e != exceptSession) : _sessions;
                    foreach (var session in targetSessions)
                        session.Send(packet);
                });
        }

        private async void ProcessSocket(Socket socket)
        {
            Logger.Write("Connected from: " + socket.RemoteEndPoint);

            var session = new PacketSession(socket);
            _lock.DoWriteLock(() => _sessions.Add(session));
            Dispatch(new ConnectionPacket { Connected = true }, session);
            try
            {
                while (true)
                {
                    var packet = await session.Receive();
                    Dispatch(packet, session);
                }
            }
            catch (Exception e)
            {
                Logger.Write(e);
            }

            Dispatch(new ConnectionPacket {Connected = false}, session);
            _lock.DoWriteLock(() => _sessions.Remove(session));

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
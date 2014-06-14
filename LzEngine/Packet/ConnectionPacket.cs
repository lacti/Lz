using System;

namespace LzEngine.Packet
{
    public class ConnectionPacket : IPacket
    {
        public bool Connected { get; set; }
    }
}
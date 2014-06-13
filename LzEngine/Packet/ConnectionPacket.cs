using System;

namespace LzEngine.Packet
{
    [Serializable]
    public class ConnectionPacket : PacketBase
    {
        public bool Connected { get; set; }
    }
}
using System;

namespace LzEngine.Packet
{
    public class ChatPacket : IPacket
    {
        public string Name { get; set; }
        public string Message { get; set; }
    }
}
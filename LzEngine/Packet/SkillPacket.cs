using System;

namespace LzEngine.Packet
{
    [Serializable]
    public class SkillPacket : PacketBase
    {
        public int AttackerObjectId { get; set; }
        public int AttackeeObjectId { get; set; }
    }
}
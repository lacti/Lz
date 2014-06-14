using System;

namespace LzEngine.Packet
{
    public class SkillPacket : IPacket
    {
        public int AttackerObjectId { get; set; }
        public int AttackeeObjectId { get; set; }
    }
}
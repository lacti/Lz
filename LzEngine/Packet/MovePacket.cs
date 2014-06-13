using System;
using System.Drawing;
using LzEngine.Enum;

namespace LzEngine.Packet
{
    [Serializable]
    public class MovePacket : PacketBase
    {
        public int ObjectId { get; set; }
        public MoveStateType State { get; set; }
        public DirectionType Direction { get; set; }

        public Point CurrentPoint { get; set; }
    }
}
﻿using System;
using System.Drawing;
using LzEngine.Enum;

namespace LzEngine.Packet
{
    [Serializable]
    public class ObjectPacket : PacketBase
    {
        public int ObjectId { get; set; }
        public string Name { get; set; }
        public ObjectState State { get; set; }
        public ObjectType Type { get; set; }

        public Point CurrentPosition { get; set; }
    }
}
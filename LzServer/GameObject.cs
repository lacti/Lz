using System.Drawing;
using LzEngine.Enum;
using LzEngine.Packet;

namespace LzServer
{
    partial class GameObject
    {
        public GameObject()
        {
            MoveState = MoveStateType.Stop;
            Direction = DirectionType.Down;
        }

        public int ObjectId { get; set; }
        public string Name { get; set; }
        public Point Position { get; set; }
        public MoveStateType MoveState { get; set; }
        public DirectionType Direction { get; set; }
        public ObjectType Type { get; set; }

        public bool IsSpawned { get; set; }
    }

    #region ToPacket

    partial class GameObject
    {
        public LoginPacket ToLoginPacket()
        {
            return new LoginPacket
                {
                    ObjectId = ObjectId,
                    Name = Name,
                    CurrentPosition = Position,
                    State = ObjectState.Appear,
                    Type = Type
                };
        }


        public ObjectPacket ToSpawnPacket()
        {
            return new ObjectPacket
                {
                    ObjectId = ObjectId,
                    Name = Name,
                    CurrentPosition = Position,
                    State = ObjectState.Appear,
                    Type = Type
                };
        }

        public ObjectPacket ToDespawnPacket()
        {
            return new ObjectPacket
                {
                    ObjectId = ObjectId,
                    State = ObjectState.Disappear
                };
        }

        public MovePacket ToMovePacket()
        {
            return new MovePacket
                {
                    ObjectId = ObjectId,
                    CurrentPoint = Position,
                    Direction = Direction,
                    State = MoveState
                };
        }
    }

    #endregion
}
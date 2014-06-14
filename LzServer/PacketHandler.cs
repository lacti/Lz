using System.Linq;
using System.Threading.Tasks;
using LzEngine.Enum;
using LzEngine.Packet;

namespace LzServer
{
    public class PacketHandler
    {
        private readonly ServerContext _context;

        internal PacketHandler(ServerContext context)
        {
            _context = context;
        }

        public void Handle(ConnectionPacket packet, PacketSession peerSession)
        {
            if (packet.Connected)
            {
                // 로그인한 유저 객체 만들고 각 객체 상황 전달한다.
                var playerObject = _context.NewGameObject(ObjectType.Player);
                peerSession.Send(playerObject.ToLoginPacket());

                foreach (var each in _context.GameObjects.Select(e => e.ToSpawnPacket()))
                    peerSession.Send(each);

                foreach (var each in _context.GameObjects.Select(e => e.ToMovePacket()))
                    peerSession.Send(each);

                // 다른 유저들에게 해당 유저가 접속했다는 사실을 알려준다.
                _context.BroadcastPacket(playerObject.ToSpawnPacket(), peerSession);

                _context.AddPlayer(playerObject, peerSession);
            }
            else
            {
                var logoutObject = _context.RemovePlayer(peerSession);

                // 다른 유저들에게 유저가 나갔다는 사실을 알려준다.
                if (logoutObject != null)
                    _context.BroadcastPacket(logoutObject.ToDespawnPacket(), peerSession);
            }
        }

        public void Handle(MovePacket packet, PacketSession peerSession)
        {
            var targetObject = _context.GetGameObjecct(packet.ObjectId);
            if (targetObject == null)
                return;

            targetObject.Position = packet.CurrentPoint;
            targetObject.MoveState = packet.State;
            targetObject.Direction = packet.Direction;

            // 다른 유저들에게 해당 유저가 움직인다는 사실을 알려준다.
            _context.BroadcastPacket(targetObject.ToMovePacket(), peerSession);
        }

        public void Handle(SkillPacket packet, PacketSession peerSession)
        {
            // 한 대 맞으면 즉사한다.
            var attackeeObject = _context.RemoveGameObject(packet.AttackeeObjectId);
            if (attackeeObject == null)
                return;

            _context.BroadcastPacket(packet);

            // 1s 뒤에 해당 객체 삭제 패킷을 전달
            const int despawnDelay = 1000;
            Task.Delay(despawnDelay).ContinueWith(_ => _context.BroadcastPacket(attackeeObject.ToDespawnPacket()));
        }

        public void Handle(ChatPacket packet, PacketSession peerSession)
        {
            // 전체에 broadcasting
            _context.BroadcastPacket(packet);
        }
    }
}
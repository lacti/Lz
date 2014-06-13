using System.Linq;
using System.Net.Sockets;
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

        public void Handle(ConnectionPacket packet, Socket peerSocket)
        {
            if (packet.Connected)
            {
                // 로그인한 유저 객체 만들고 각 객체 상황 전달한다.
                var playerObject = _context.NewGameObject(ObjectType.Player);
                peerSocket.SendPacket(playerObject.ToLoginPacket());

                foreach (var each in _context.GameObjects.Select(e => e.ToSpawnPacket()))
                    peerSocket.SendPacket(each);

                foreach (var each in _context.GameObjects.Select(e => e.ToMovePacket()))
                    peerSocket.SendPacket(each);

                // 다른 유저들에게 해당 유저가 접속했다는 사실을 알려준다.
                _context.BroadcastPacket(playerObject.ToSpawnPacket(), peerSocket);

                _context.AddPlayer(playerObject, peerSocket);
            }
            else
            {
                var logoutObject = _context.RemovePlayer(peerSocket);

                // 다른 유저들에게 유저가 나갔다는 사실을 알려준다.
                if (logoutObject != null)
                    _context.BroadcastPacket(logoutObject.ToDespawnPacket(), peerSocket);
            }
        }

        public void Handle(MovePacket packet, Socket peerSocket)
        {
            var targetObject = _context.GetGameObjecct(packet.ObjectId);
            if (targetObject == null)
                return;

            targetObject.Position = packet.CurrentPoint;
            targetObject.MoveState = packet.State;
            targetObject.Direction = packet.Direction;

            // 다른 유저들에게 해당 유저가 움직인다는 사실을 알려준다.
            _context.BroadcastPacket(targetObject.ToMovePacket(), peerSocket);
        }

        public void Handle(SkillPacket packet, Socket peerSocket)
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
    }
}
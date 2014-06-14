using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using LzEngine.Enum;
using LzEngine.Util;

namespace LzServer
{
    internal class NpcAi
    {
        private readonly ServerContext _context;
        private readonly Random _random = new Random();

        private int _spawnedCount;

        public NpcAi(ServerContext context)
        {
            _context = context;
            _context.AddEntry(RegenerateEntry);
        }

        public void Restore(GameObject npc)
        {
            var newAi = new EachAi(this, npc);
            _context.AddEntry(newAi.AiLogicEntry);

            ++_spawnedCount;
        }

        public IEnumerable<int> RegenerateEntry()
        {
            const int regenerateBase = 3000;
            const int regenerateInterval = 5000;
            while (true)
            {
                if (_spawnedCount < _context.Data.World.Config.maxNpcCount)
                {
                    Interlocked.Increment(ref _spawnedCount);

                    // Npc를 새로 만들고, 주변에 알려준다.
                    var newNpc = _context.NewGameObject(ObjectType.Npc);
                    _context.AddGameObject(newNpc);
                    _context.BroadcastPacket(newNpc.ToSpawnPacket());
                    Logger.Write("Npc[{0}] is spawned!", newNpc.Name);

                    // NpcAi를 생성하고, Coroutine에 등록한다.
                    var newAi = new EachAi(this, newNpc);
                    _context.AddEntry(newAi.AiLogicEntry);
                }

                var nextInterval = _random.Next(regenerateInterval) + regenerateBase;
                yield return nextInterval;
            }
        }

        internal void DecreaseCount()
        {
            Interlocked.Decrement(ref _spawnedCount);
        }

        private class EachAi
        {
            private readonly NpcAi _generator;
            private readonly GameObject _npc;
            private readonly Random _random;

            public EachAi(NpcAi generator, GameObject npc)
            {
                _generator = generator;
                _npc = npc;

                _random = new Random(_npc.GetHashCode());
            }

            public IEnumerable<int> AiLogicEntry()
            {
                while (_npc.IsSpawned)
                {
                    // 랜덤하게 움직이도록 한다.
                    var direction = (DirectionType) _random.Next(4);
                    var moveUnit = GetUnitPointByDirection(direction);

                    const int moveCountBase = 2;
                    const int moveCountRandom = 8;
                    var moveCount = moveCountBase + _random.Next(moveCountRandom);
                    foreach (var _ in Enumerable.Range(0, moveCount))
                    {
                        _npc.MoveState = MoveStateType.Moving;
                        _npc.Direction = direction;
                        _generator._context.BroadcastPacket(_npc.ToMovePacket());

                        const int moveIntervalBase = 800;
                        const int moveIntervalRandom = 600;
                        yield return moveIntervalBase + _random.Next(moveIntervalRandom);

                        _npc.Position = new Point(_npc.Position.X + moveUnit.X, _npc.Position.Y + moveUnit.Y);
                        _npc.MoveState = MoveStateType.Stop;

                        if (!_npc.IsSpawned)
                            break;
                    }

                    const int waitIntervalBase = 1000;
                    const int waitIntervalRandom = 3000;
                    yield return waitIntervalBase + _random.Next(waitIntervalRandom);
                }
                _generator.DecreaseCount();
            }

            private static Point GetUnitPointByDirection(DirectionType direction)
            {
                switch (direction)
                {
                    case DirectionType.Down:
                        return new Point(0, 1);
                    case DirectionType.Left:
                        return new Point(-1, 0);
                    case DirectionType.Right:
                        return new Point(1, 0);
                    case DirectionType.Up:
                        return new Point(0, -1);
                }
                return Point.Empty;
            }
        }
    }
}
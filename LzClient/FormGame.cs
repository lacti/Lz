using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;
using LzClient.Object;
using LzClient.Util;
using LzEngine.Enum;
using LzEngine.Packet;

namespace LzClient
{
    public partial class FormGame : Form
    {
        private readonly Dictionary<int, Character> _characters = new Dictionary<int, Character>();

        private readonly PacketDispatcher _dispatcher = new PacketDispatcher();
        private readonly List<IEnumerator> _drawEnumerators = new List<IEnumerator>();

        private readonly GraphicsHolder _graphicsHolder = new GraphicsHolder();
        private readonly string _host = "127.0.0.1";
        private readonly List<DrawDelegate> _newDrawDelegates = new List<DrawDelegate>();
        private readonly int _port = 40123;
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly List<Character> _targetCacheX = new List<Character>();
        private readonly List<Character> _targetCacheY = new List<Character>();
        private Character _attackTarget;
        private bool _connected;
        private PointF _lastCameraPos = PointF.Empty;

        private Character _player;
        private bool _selectEnemyMode;

        public FormGame()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length >= 2) _host = args[1];
            if (args.Length >= 3) _port = int.Parse(args[2]);

            InitializeComponent();
            _dispatcher.RegisterHandler(this);
        }

        private RectangleF ViewRect
        {
            get
            {
                var viewSize = panelCanvas.ClientSize;
                return new RectangleF(_lastCameraPos.X, _lastCameraPos.Y, viewSize.Width, viewSize.Height);
            }
        }

        private IEnumerable<Character> CurrentCharacters
        {
            get
            {
                var viewRect = ViewRect;
                return _characters.Values.Where(each => viewRect.Contains(each.DrawPosition));
            }
        }

        private void FormGame_Load(object sender, EventArgs e)
        {
            Task.Factory.StartNew(PacketPump);
        }

        private T Spawn<T>(int objectId, string name, Point pos) where T : Character
        {
            var newChar = (Character) Activator.CreateInstance(typeof (T), objectId, pos);
            newChar.Name = name;
            if (_characters.ContainsKey(objectId))
                return null;

            _characters.Add(objectId, newChar);
            _newDrawDelegates.Add(newChar.Draw);
            return (T) newChar;
        }

        private void Despawn(int objectId)
        {
            Character findOne;
            if (!_characters.TryGetValue(objectId, out findOne))
                return;

            findOne.IsSpawned = false;
            _characters.Remove(objectId);
        }

        private async void PacketPump()
        {
            try
            {
                _socket.Connect(_host, _port);
                _connected = true;
                while (true)
                {
                    var packet = await _socket.ReceivePacket();
                    Invoke(
                        new MethodInvoker(
                            () => _dispatcher.Dispatch(packet, _socket)));
                }
            }
            catch
            {
            }
            _connected = false;
        }

        private void panelCanvas_Paint(object sender, PaintEventArgs e)
        {
            if (!_connected)
                return;

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (_player != null)
            {
                _lastCameraPos.X = _player.DrawPosition.X - panelCanvas.ClientSize.Width/2.0f + _player.Width/2.0f;
                _lastCameraPos.Y = _player.DrawPosition.Y - panelCanvas.ClientSize.Height/2.0f + _player.Height/2.0f;
            }
            g.Transform = new Matrix(1, 0, 0, 1, -_lastCameraPos.X, -_lastCameraPos.Y);

            foreach (var newOne in _newDrawDelegates)
                _drawEnumerators.Add(newOne(_graphicsHolder));
            _newDrawDelegates.Clear();

            _graphicsHolder.Value = g;
            foreach (
                var removal in
                    _drawEnumerators.Where(enumerator => !enumerator.MoveNext())
                                    .ToArray())
                _drawEnumerators.Remove(removal);

            if (_attackTarget != null)
            {
                var targetDrawPos = _attackTarget.DrawPosition;
                g.DrawRectangle(Pens.White, targetDrawPos.X, targetDrawPos.Y, _attackTarget.Width*1.33f, _attackTarget.Height*1.33f);
            }
        }

        internal void HandlePacket(ObjectPacket packet, Socket peedSocket)
        {
            switch (packet.State)
            {
                case ObjectState.Appear:
                    if (packet.Type == ObjectType.Player) Spawn<Player>(packet.ObjectId, packet.Name, packet.CurrentPosition);
                    else if (packet.Type == ObjectType.Npc) Spawn<Npc>(packet.ObjectId, packet.Name, packet.CurrentPosition);
                    break;

                case ObjectState.Disappear:
                    Despawn(packet.ObjectId);

                    if (_player != null && packet.ObjectId == _player.ObjectId)
                    {
                        _player = null;
                    }
                    break;
            }
        }

        internal void HandlePacket(LoginPacket packet, Socket peerSocket)
        {
            _player = Spawn<Player>(packet.ObjectId, packet.Name, packet.CurrentPosition);
        }

        internal void HandlePacket(MovePacket packet, Socket peedSocket)
        {
            if (_player != null && _player.ObjectId == packet.ObjectId)
                return;

            Character findOne;
            if (!_characters.TryGetValue(packet.ObjectId, out findOne))
                return;

            findOne.Correction(packet.CurrentPoint, packet.State, packet.Direction);
        }

        internal void HandlePacket(SkillPacket packet, Socket peerSocket)
        {
            Character findOne;
            if (!_characters.TryGetValue(packet.AttackeeObjectId, out findOne))
                return;

            _newDrawDelegates.Add(new Effect(findOne).Draw);
        }

        private void timerRender_Tick(object sender, EventArgs e)
        {
            panelCanvas.Invalidate();
        }

        private void FormGame_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (_player == null || !_connected)
                return;

            if (_selectEnemyMode)
            {
                SelectTargetAndExecuteSkill(e.KeyCode);
                return;
            }

            var direction = DirectionType.None;
            switch (e.KeyCode)
            {
                case Keys.Down:
                    direction = DirectionType.Down;
                    break;
                case Keys.Left:
                    direction = DirectionType.Left;
                    break;
                case Keys.Right:
                    direction = DirectionType.Right;
                    break;
                case Keys.Up:
                    direction = DirectionType.Up;
                    break;
                case Keys.Space:
                    _attackTarget = SelectNextTarget(DirectionType.Right);
                    if (_attackTarget != null)
                        _selectEnemyMode = true;
                    break;
            }

            if (direction != DirectionType.None)
            {
                _player.Move(direction);
                _socket.SendPacket(new MovePacket
                    {
                        ObjectId = _player.ObjectId,
                        CurrentPoint = _player.Position,
                        State = MoveStateType.Moving,
                        Direction = _player.Direction
                    });
            }
        }

        private void SelectTargetAndExecuteSkill(Keys keyCode)
        {
            var selectDir = DirectionType.None;
            switch (keyCode)
            {
                case Keys.Left:
                    selectDir = DirectionType.Left;
                    break;
                case Keys.Right:
                    selectDir = DirectionType.Right;
                    break;
                case Keys.Up:
                    selectDir = DirectionType.Up;
                    break;
                case Keys.Down:
                    selectDir = DirectionType.Down;
                    break;

                case Keys.Enter:
                case Keys.Space:
                    if (_attackTarget != null)
                    {
                        _socket.SendPacket(new SkillPacket
                            {
                                AttackerObjectId = _player.ObjectId,
                                AttackeeObjectId = _attackTarget.ObjectId
                            });
                    }
                    _attackTarget = null;
                    _selectEnemyMode = false;
                    break;

                case Keys.Escape:
                    _attackTarget = null;
                    _selectEnemyMode = false;
                    break;
            }

            if (selectDir == DirectionType.None)
                return;

            _attackTarget = SelectNextTarget(selectDir);
            if (_attackTarget == null)
            {
                _selectEnemyMode = false;
            }
        }

        private Character PrepareTargetCache(DirectionType direction)
        {
            _targetCacheX.Clear();
            _targetCacheY.Clear();
            _targetCacheX.AddRange(CurrentCharacters.Where(e => e != _player).OrderBy(e => e.Position.X));
            _targetCacheY.AddRange(CurrentCharacters.Where(e => e != _player).OrderBy(e => e.Position.Y));

            if (_targetCacheX.Count == 0 || _targetCacheY.Count == 0)
                return null;

            switch (direction)
            {
                case DirectionType.Left:
                    return _targetCacheX.Where(e => e.Position.X <= _player.Position.X).OrderByDescending(e => e.Position.X).FirstOrDefault() ??
                           _targetCacheX.First();
                case DirectionType.Right:
                    return _targetCacheX.Where(e => e.Position.X >= _player.Position.X).OrderBy(e => e.Position.X).FirstOrDefault() ?? _targetCacheX.Last();
                case DirectionType.Up:
                    return _targetCacheY.Where(e => e.Position.Y <= _player.Position.Y).OrderByDescending(e => e.Position.Y).FirstOrDefault() ??
                           _targetCacheY.First();
                case DirectionType.Down:
                    return _targetCacheY.Where(e => e.Position.Y >= _player.Position.Y).OrderBy(e => e.Position.Y).FirstOrDefault() ?? _targetCacheY.Last();
            }
            return null;
        }

        private Character SelectNextTarget(DirectionType direction)
        {
            if (_attackTarget == null)
                return (_attackTarget = PrepareTargetCache(direction));

            var viewRect = ViewRect;
            _targetCacheX.RemoveAll(e => !viewRect.Contains(e.Position));
            _targetCacheY.RemoveAll(e => !viewRect.Contains(e.Position));

            _targetCacheX.AddRange(CurrentCharacters.Except(_targetCacheX.Concat(new[] {_player})));
            _targetCacheY.AddRange(CurrentCharacters.Except(_targetCacheY.Concat(new[] {_player})));
            _targetCacheX.Sort((left, right) => left.Position.X - right.Position.X);
            _targetCacheY.Sort((left, right) => left.Position.Y - right.Position.Y);

            if (_targetCacheX.Count == 0 || _targetCacheY.Count == 0)
                return null;

            if (direction == DirectionType.Left || direction == DirectionType.Right)
            {
                var index = _targetCacheX.IndexOf(_attackTarget);
                index += direction == DirectionType.Left ? -1 : 1;
                index = (index + _targetCacheX.Count)%_targetCacheX.Count;
                return _targetCacheX[index];
            }
            else
            {
                var index = _targetCacheY.IndexOf(_attackTarget);
                index += direction == DirectionType.Up ? -1 : 1;
                index = (index + _targetCacheY.Count)%_targetCacheY.Count;
                return _targetCacheY[index];
            }
        }

        private delegate IEnumerator DrawDelegate(GraphicsHolder holder);
    }
}
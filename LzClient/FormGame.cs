using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using LzClient.Object;
using LzEngine.Enum;
using LzEngine.Packet;
using LzEngine.Util;
using LzEngine.World;

namespace LzClient
{
    public partial class FormGame : Form
    {
        private readonly Dictionary<int, Character> _characters = new Dictionary<int, Character>();

        private readonly Coroutine _coro = new Coroutine();
        private readonly List<IDrawObject> _drawObjects = new List<IDrawObject>();

        private readonly PacketDispatcher _dispatcher = new PacketDispatcher();

#if DEBUG
        private readonly string _host = "127.0.0.1";
#else
        private readonly string _host = "54.250.154.191";
#endif

        private readonly int _port = 40123;
        private readonly PacketSession _session = new PacketSession();
        private bool _connected;
        private PointF _lastCameraPos = PointF.Empty;

        private Character _player;
        private WorldMap _worldMap;
        private Bitmap _worldBackground;

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

            _coro.AddEntry(ClearChatMessage);
        }

        private T Spawn<T>(int objectId, string name, Point pos) where T : Character
        {
            var newChar = (Character) Activator.CreateInstance(typeof (T), objectId, pos);
            newChar.Name = name;
            if (_characters.ContainsKey(objectId))
                return null;

            _characters.Add(objectId, newChar);
            _coro.AddEntry(newChar.Update);
            _drawObjects.Add(newChar);
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
                _session.Connect(_host, _port);
                _connected = true;
                while (true)
                {
                    var packet = await _session.Receive();
                    Invoke(
                        new MethodInvoker(
                            () => _dispatcher.Dispatch(packet, _session)));
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

            if (_worldBackground != null)
                g.DrawImage(_worldBackground, 0, 0);

            foreach (var each in _drawObjects)
                each.Draw(g);

            if (_attackTarget != null)
            {
                var targetDrawPos = _attackTarget.DrawPosition;
                g.DrawRectangle(Pens.White, targetDrawPos.X, targetDrawPos.Y, _attackTarget.Width*1.33f, _attackTarget.Height*1.33f);
            }

            DrawChatMessages(g);
        }

        internal void HandlePacket(ObjectPacket packet, PacketSession peerSession)
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

        internal void HandlePacket(LoginPacket packet, PacketSession peerSession)
        {
            // initialize world
            _worldMap = MapObject.Load<WorldMap>(Path.Combine(ResourcePath.MapRoot, "World01.xml"));
            _worldBackground = new Bitmap(_worldMap.Width, _worldMap.Height, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(_worldBackground))
            {
                _worldMap.Paint(g, Point.Empty, false);
            }

            foreach (var each in _worldMap.ObjectPositions)
                _drawObjects.Add(new ObjectWrapper(each.Key, each.Value));

            _player = Spawn<Player>(packet.ObjectId, packet.Name, packet.CurrentPosition);
        }

        internal void HandlePacket(MovePacket packet, PacketSession peerSession)
        {
            if (_player != null && _player.ObjectId == packet.ObjectId)
                return;

            Character findOne;
            if (!_characters.TryGetValue(packet.ObjectId, out findOne))
                return;

            findOne.Correction(packet.CurrentPoint, packet.State, packet.Direction);
        }

        internal void HandlePacket(SkillPacket packet, PacketSession peerSession)
        {
            Character findOne;
            if (!_characters.TryGetValue(packet.AttackeeObjectId, out findOne))
                return;

            var newEffect = new Effect(findOne);
            _coro.AddEntry(newEffect.Update);
            _drawObjects.Add(newEffect);
        }

        private void timerGame_Tick(object sender, EventArgs e)
        {
            // update
            _coro.IterateLogic();

            // render
            _drawObjects.RemoveAll(each => !each.IsDrawable);
            _drawObjects.Sort((left, right) => left.DrawPrioirty - right.DrawPrioirty);
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
                case Keys.Enter:
                    if (textChat.Visible)
                    {
                        textChat.Enabled = false;
                        textChat.Visible = false;
                    }
                    else
                    {
                        textChat.Enabled = true;
                        textChat.Visible = true;
                        textChat.Focus();
                    }
                    break;
            }

            if (direction != DirectionType.None)
            {
                if (_player.Move(direction))
                {
                    _session.Send(new MovePacket
                    {
                        ObjectId = _player.ObjectId,
                        CurrentPoint = _player.Position,
                        State = MoveStateType.Moving,
                        Direction = _player.Direction
                    });
                }
            }
        }

        #region Select Target

        private readonly List<Character> _targetCacheX = new List<Character>();
        private readonly List<Character> _targetCacheY = new List<Character>();
        private Character _attackTarget;
        private bool _selectEnemyMode;

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
                        _session.Send(new SkillPacket
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

        #endregion

        #region Chat

        private void textChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter)
                return;


            var name = _player != null ? _player.Name : "Ghost";
            _session.Send(new ChatPacket
                {
                    Name = name, Message =  textChat.Text
                });

            textChat.Text = "";
            textChat.Enabled = false;
            textChat.Visible = false;
        }

        private readonly List<Tuple<DateTime, ChatPacket>> _lastChatPackets = new List<Tuple<DateTime, ChatPacket>>();

        internal void HandlePacket(ChatPacket packet, PacketSession peerSession)
        {
            _lastChatPackets.Add(Tuple.Create(DateTime.Now, packet));

            const int maxChatCount = 8;
            while (_lastChatPackets.Count > maxChatCount)
                _lastChatPackets.RemoveAt(0);
        }

        private IEnumerable<int> ClearChatMessage()
        {
            while (true)
            {
                const int cleanInterval = 5;
                var clearDateTime = DateTime.Now.AddSeconds(-cleanInterval);
                _lastChatPackets.RemoveAll(e => clearDateTime > e.Item1);

                const int checkInterval = 1000;
                yield return checkInterval;
            }
        }

        private void DrawChatMessages(Graphics g)
        {
            const int marginX = 18;
            const int marginY = 18;
            var y = marginY;
            foreach (var each in _lastChatPackets)
            {
                var message = string.Format("<{0}> {1}", each.Item2.Name, each.Item2.Message);
                var measure = g.MeasureString(message, Global.DefaultFont);
                DrawText(g, message, marginX + _lastCameraPos.X, y + _lastCameraPos.Y, Brushes.BlanchedAlmond, Brushes.Black);

                y += (int) (measure.Height*1.3);
            }
        }

        private void DrawText(Graphics g, string text, float x, float y, Brush color, Brush outlineColor)
        {
            foreach (var dx in new[] { -1, 0, 1 })
                foreach (var dy in new[] { -1, 0, 1 })
                    g.DrawString(text, Global.DefaultFont, outlineColor, x + dx, y + dy);

            g.DrawString(text, Global.DefaultFont, color, x, y);
        }

        #endregion
    }
}
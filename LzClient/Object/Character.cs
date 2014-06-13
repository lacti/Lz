using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using LzClient.Util;
using LzEngine.Enum;

namespace LzClient.Object
{
    internal class Character
    {
        private const int MoveDirection = 4;
        private const int MovePhase = 4;
        private readonly Lazy<int> _heightLazy;
        private readonly Bitmap _resource;
        private readonly Lazy<int> _widthLazy;
        private Point _currentPosition;
        private Point _drawPosition = Point.Empty;

        protected Character(int objectId, Bitmap resource, Point startPoint)
        {
            ObjectId = objectId;

            // Resources.Char001
            MoveState = MoveStateType.Stop;
            Direction = DirectionType.Down;

            _currentPosition = startPoint;
            _resource = resource;
            IsSpawned = true;

            _widthLazy = new Lazy<int>(() => _resource.Width/MoveDirection);
            _heightLazy = new Lazy<int>(() => _resource.Height/MoveDirection);
        }

        public int ObjectId { get; private set; }
        public MoveStateType MoveState { get; private set; }
        public DirectionType Direction { get; private set; }
        public string Name { get; set; }

        public Point Position
        {
            get { return _currentPosition; }
        }

        public Point DrawPosition
        {
            get { return _drawPosition; }
        }

        public bool IsSpawned { get; set; }

        public int Width
        {
            get { return _widthLazy.Value; }
        }

        public int Height
        {
            get { return _heightLazy.Value; }
        }

        public void Move(DirectionType direction)
        {
            if (MoveState == MoveStateType.Moving)
                return;

            Direction = direction;
            MoveState = MoveStateType.Moving;
        }

        public void Correction(Point position, MoveStateType moveState, DirectionType direction)
        {
            _currentPosition = position;
            MoveState = moveState;
            Direction = direction;
        }

        public IEnumerator Draw(GraphicsHolder g)
        {
            var srcRect = new Rectangle(0, 0, Width, Height);

            while (IsSpawned)
            {
                var charXPos = Position.X*Global.TileWidth + (Global.TileWidth - Width)/2;
                var charYPos = Position.Y*Global.TileHeight - Height;
                _drawPosition = new Point(charXPos, charYPos);

                switch (MoveState)
                {
                    case MoveStateType.Stop:
                        srcRect.X = 0;
                        srcRect.Y = (int) Direction*Height;
                        DrawCharacter(g.Value, srcRect);
                        break;

                    case MoveStateType.Moving:
                        const int animationCount = 4;
                        const int animtaionXDelta = Global.TileWidth/animationCount;
                        const int animtaionYDelta = Global.TileHeight/animationCount;
                        var unitMovePos = GetUnitPointByDirection(Direction);
                        foreach (var index in Enumerable.Range(1, animationCount))
                        {
                            srcRect.X = (index%MovePhase)*Width;
                            srcRect.Y = (int) Direction*Height;
                            _drawPosition = new Point(charXPos + unitMovePos.X*index*animtaionXDelta, charYPos + unitMovePos.Y*index*animtaionYDelta);
                            DrawCharacter(g.Value, srcRect);

                            if (index != animationCount)
                                yield return 0;
                        }
                        _currentPosition.Offset(unitMovePos);
                        MoveState = MoveStateType.Stop;
                        break;
                }

                yield return 0;
            }
        }

        private void DrawCharacter(Graphics g, Rectangle srcRect)
        {
            g.DrawImage(_resource, _drawPosition.X, _drawPosition.Y, srcRect, GraphicsUnit.Pixel);

            if (string.IsNullOrWhiteSpace(Name))
                return;

            var name = Name; // string.Format("{0} ({1},{2})", Name, Position.X, Position.Y);
            var measure = g.MeasureString(name, Global.DefaultFont);
            var nameX = _drawPosition.X + (Width - measure.Width)/2 + 4 /* magic-margin */;
            var nameY = _drawPosition.Y - (int) (measure.Height*1.2f);

            foreach (var dx in new[] {-1, 0, 1})
                foreach (var dy in new[] {-1, 0, 1})
                    g.DrawString(name, Global.DefaultFont, Brushes.Black, nameX + dx, nameY + dy);

            g.DrawString(name, Global.DefaultFont, Brushes.White, nameX, nameY);
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
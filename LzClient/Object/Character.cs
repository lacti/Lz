using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LzEngine.Enum;

namespace LzClient.Object
{
    internal class Character : IDrawObject
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

        public int DrawPrioirty
        {
            get { return _currentPosition.Y * 1000 + _currentPosition.X; }
        }

        public bool IsDrawable { get { return IsSpawned; } }

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

        public bool Move(DirectionType direction)
        {
            if (MoveState == MoveStateType.Moving)
                return false;

            Direction = direction;
            MoveState = MoveStateType.Moving;
            return true;
        }

        public void Correction(Point position, MoveStateType moveState, DirectionType direction)
        {
            _currentPosition = position;
            MoveState = moveState;
            Direction = direction;
        }

        private Rectangle _srcRect;
        
        public IEnumerable<int> Update()
        {
            _srcRect = new Rectangle(0, 0, Width, Height);

            while (IsSpawned)
            {
                var charXPos = Position.X * Global.TileWidth + (Global.TileWidth - Width) / 2;
                var charYPos = Position.Y * Global.TileHeight - Height;
                _drawPosition = new Point(charXPos, charYPos);

                switch (MoveState)
                {
                    case MoveStateType.Stop:
                        _srcRect.X = 0;
                        _srcRect.Y = (int)Direction * Height;
                        break;

                    case MoveStateType.Moving:
                        const int animationCount = 4;
                        const int animtaionXDelta = Global.TileWidth / animationCount;
                        const int animtaionYDelta = Global.TileHeight / animationCount;
                        var unitMovePos = GetUnitPointByDirection(Direction);
                        foreach (var index in Enumerable.Range(1, animationCount))
                        {
                            _srcRect.X = (index % MovePhase) * Width;
                            _srcRect.Y = (int)Direction * Height;
                            _drawPosition = new Point(charXPos + unitMovePos.X * index * animtaionXDelta, charYPos + unitMovePos.Y * index * animtaionYDelta);

                            if (index != animationCount)
                                yield return 33;
                        }
                        _currentPosition.Offset(unitMovePos);
                        MoveState = MoveStateType.Stop;
                        break;
                }

                yield return 33;
            }
        }

        public void Draw(Graphics g)
        {
            g.DrawImage(_resource, _drawPosition.X, _drawPosition.Y, _srcRect, GraphicsUnit.Pixel);

            if (string.IsNullOrWhiteSpace(Name))
                return;

            var name = string.Format("{0} ({1},{2})", Name, Position.X, Position.Y);
            var measure = g.MeasureString(name, Global.DefaultFont);
            var nameX = _drawPosition.X + (Width - measure.Width) / 2 + 4 /* magic-margin */;
            var nameY = _drawPosition.Y - (int)(measure.Height * 1.2f);

            foreach (var dx in new[] { -1, 0, 1 })
                foreach (var dy in new[] { -1, 0, 1 })
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
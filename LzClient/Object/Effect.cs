using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using LzClient.Properties;

namespace LzClient.Object
{
    internal class Effect : IDrawObject
    {
        private const int AnimationCount = 10;
        private const int RowCount = 2;
        private const int ColumnCount = AnimationCount/RowCount;

        private readonly Lazy<int> _heightLazy;
        private readonly Bitmap _resource;
        private readonly Character _target;
        private readonly Lazy<int> _widthLazy;

        public Effect(Character target)
        {
            _target = target;
            _resource = Resources.Skill001;

            _widthLazy = new Lazy<int>(() => _resource.Width/ColumnCount);
            _heightLazy = new Lazy<int>(() => _resource.Height/RowCount);
        }

        public int Width
        {
            get { return _widthLazy.Value; }
        }

        public int Height
        {
            get { return _heightLazy.Value; }
        }

        private int _effectXPos;
        private int _effectYPos;
        private Rectangle _srcRect;

        public IEnumerable<int> Update()
        {
            IsDrawable = true;

            foreach (var index in Enumerable.Range(0, AnimationCount))
            {
                var targetPos = _target.Position;
                _effectXPos = targetPos.X * Global.TileWidth + (Global.TileWidth - Width) / 2;
                _effectYPos = targetPos.Y * Global.TileHeight - Height + 40 /* magic-margin */;

                var frameColumn = index % ColumnCount;
                var frameRow = index / ColumnCount;
                _srcRect = new Rectangle(frameColumn * Width, frameRow * Height, Width, Height);
                yield return 80;
            }

            IsDrawable = false;
        }

        public int DrawPrioirty { get { return int.MaxValue; } }
        public bool IsDrawable { get; private set; }

        public void Draw(Graphics g)
        {
            g.DrawImage(_resource, _effectXPos, _effectYPos, _srcRect, GraphicsUnit.Pixel);
        }
    }
}
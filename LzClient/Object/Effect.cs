using System;
using System.Collections;
using System.Drawing;
using System.Linq;
using LzClient.Properties;
using LzClient.Util;

namespace LzClient.Object
{
    internal class Effect
    {
        private const int AnimationCount = 10;
        private const int RowCount = 2;
        private const int ColumnCount = AnimationCount/RowCount;

        private const int DelayCount = 2;

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

        public IEnumerator Draw(GraphicsHolder g)
        {
            foreach (var index in Enumerable.Range(0, AnimationCount))
            {
                var targetPos = _target.Position;
                var effectXPos = targetPos.X*Global.TileWidth + (Global.TileWidth - Width)/2;
                var effectYPos = targetPos.Y*Global.TileHeight - Height + 40 /* magic-margin */;

                var frameColumn = index%ColumnCount;
                var frameRow = index/ColumnCount;
                var srcRect = new Rectangle(frameColumn*Width, frameRow*Height, Width, Height);
                foreach (var _ in Enumerable.Range(0, DelayCount))
                {
                    g.Value.DrawImage(_resource, effectXPos, effectYPos, srcRect, GraphicsUnit.Pixel);
                    yield return 0;
                }
            }
        }
    }
}
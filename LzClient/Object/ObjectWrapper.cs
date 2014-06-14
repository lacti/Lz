using System;
using System.Drawing;
using LzClient.Object;
using LzEngine.World;

namespace LzClient.Object
{
    internal class ObjectWrapper : IDrawObject
    {
        private readonly MapObject _object;
        private readonly Point _index;

        public ObjectWrapper(MapObject mapObject, Point index)
        {
            _object = mapObject;
            _index = index;

            DrawPrioirty = _index.X * 1000 + _index.Y;
            IsDrawable = true;
        }

        public int DrawPrioirty { get; private set; }
        public bool IsDrawable { get; private set; }
        public void Draw(Graphics g)
        {
            _object.Paint(g, _index, false);
        }
    }
}
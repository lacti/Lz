using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LzEngine.World
{
    public class WorldMap : MapObject
    {
        private Bitmap _tileBuffer;
        private readonly Dictionary<Point, List<MapObject>> _objects = new Dictionary<Point, List<MapObject>>();

        public WorldMap(string tilesetName) : base(tilesetName)
        {
        }

        public void AddObject(Point index, MapObject obj)
        {
            if (!_objects.ContainsKey(index))
                _objects.Add(index, new List<MapObject>());

            _objects[index].Add(obj);
        }

        public void RemoveObject(MapObject obj)
        {
            foreach (var pair in _objects)
                pair.Value.Remove(obj);
        }

        public void RemoveObject(Point index)
        {
            foreach (var pair in _objects)
            {
                pair.Value.RemoveAll(e => e.Contains(pair.Key, index));
            }
        }

        public IEnumerable<MapObject> Objects { get { return _objects.SelectMany(e => e.Value).ToArray(); }}

        public IEnumerable<KeyValuePair<MapObject, Point>> ObjectPositions
        {
            get
            {
                return
                    _objects.SelectMany(pair => pair.Value.Select(e => new KeyValuePair<MapObject, Point>(e, pair.Key)));
            }
        }

        public override void Resize(int newX, int newY)
        {
            base.Resize(newX, newY);

            _tileBuffer = new Bitmap(Width, Height, PixelFormat.Format32bppArgb);

            using (var g = Graphics.FromImage(_tileBuffer))
            {
                var points = (from y in Enumerable.Range(0, Y)
                              from x in Enumerable.Range(0, X)
                              select new Point(x * WorldConstants.TileLength, y * WorldConstants.TileLength)).ToArray();

                var srcRect = new Rectangle(0, 0, WorldConstants.TileLength, WorldConstants.TileLength);
                foreach (var point in points)
                    g.DrawImage(Pallet.Image, point.X, point.Y, srcRect, GraphicsUnit.Pixel);
            }
        }

        public override void Paint(Graphics g, Point destPos, bool editor = true)
        {
            g.DrawImage(_tileBuffer, destPos.X, destPos.Y);

            base.Paint(g, destPos, editor);

            if (editor)
            {
                foreach (var pair in _objects.OrderBy(e => e.Key.Y * 1000 + e.Key.X))
                {
                    foreach (var each in pair.Value)
                        each.Paint(g, new Point(destPos.X + pair.Key.X - each.Origin.X, destPos.Y + pair.Key.Y - each.Origin.Y));
                }
            }
        }

        protected override void LoadFromXml(XElement xml)
        {
            base.LoadFromXml(xml);

            foreach (var objectNode in xml.Element("Objects").Elements("Object"))
            {
                var objectName = objectNode.Attribute("name").Value;
                var objectIndex = new Point(
                    int.Parse(objectNode.Attribute("x").Value),
                    int.Parse(objectNode.Attribute("y").Value)
                    );

                var obj = Load<MapObject>(Path.Combine(ResourcePath.ObjectRoot, objectName + ".xml"));
                AddObject(objectIndex, obj);
            }
        }

        protected override XElement ToXml()
        {
            var root = base.ToXml();
            root.Add(new XElement("Objects",
                ObjectPositions.Select(e =>
                    new XElement("Object",
                        new XAttribute("name", e.Key.Name),
                        new XAttribute("x", e.Value.X.ToString()),
                        new XAttribute("y", e.Value.Y.ToString()))
                    )
                ));
            return root;
        }
    }
}

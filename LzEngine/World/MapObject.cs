using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace LzEngine.World
{
    public class Pallet
    {
        public Image Image { get; private set; }
        public int XCount { get; private set; }
        public int YCount { get; private set; }

        public string FileName { get; private set; }

        private Pallet(string tilesetName)
        {
            Image = Image.FromFile(tilesetName);
            FileName = Path.GetFileName(tilesetName);

            XCount = Image.Width / WorldConstants.TileLength;
            YCount = Image.Height / WorldConstants.TileLength;
        }

        public Point IndexToPoint(int index)
        {
            return new Point(index % XCount, index / XCount);
        }

        public Point IndexToPixel(int index)
        {
            var point = IndexToPoint(index);
            return new Point(point.X * WorldConstants.TileLength, point.Y * WorldConstants.TileLength);
        }

        public Rectangle IndexToTile(int index)
        {
            var pixel = IndexToPixel(index);
            return new Rectangle(pixel, WorldConstants.TileSize);
        }

        public int PointToIndex(Point point)
        {
            return point.Y * XCount + point.X;
        }

        private static readonly Dictionary<string, Pallet> Cache = new Dictionary<string, Pallet>();
        public static Pallet Get(string tilesetName)
        {
            var tilesetPath = Path.Combine(ResourcePath.TilesetRoot, tilesetName);
            if (!File.Exists(tilesetPath))
                return null;

            lock (Cache)
            {
                if (!Cache.ContainsKey(tilesetName))
                    Cache.Add(tilesetName, new Pallet(tilesetPath));
                return Cache[tilesetName];
            }
        }
    }

    public class MapObject
    {
        public readonly List<Point> Obstacles = new List<Point>();
        public int[,] Tiles = new int[0, 0];
        public string Name { get; set; }
        public Point Origin { get; set; }

        public Pallet Pallet { get; set; }

        public MapObject(string tilesetName)
        {
            Pallet = Pallet.Get(tilesetName);

            Clear();
        }

        public bool Contains(Point basePos, Point checkPos)
        {
            var translatedPos = new Point(
                checkPos.X - basePos.X + Origin.X,
                checkPos.Y - basePos.Y + Origin.Y
                );
            if (translatedPos.X < 0 || translatedPos.X >= X || translatedPos.Y < 0 || translatedPos.Y >= Y)
                return false;

            return Tiles[translatedPos.Y, translatedPos.X] >= 0;
        }

        public virtual void Paint(Graphics g, Point destPos, bool editor = true)
        {
            foreach (var y in Enumerable.Range(0, Tiles.GetLength(0)))
            {
                foreach (var x in Enumerable.Range(0, Tiles.GetLength(1)))
                {
                    var tileIndex = Tiles[y, x];
                    if (tileIndex < 0)
                        continue;

                    g.DrawImage(Pallet.Image, (x + destPos.X) * WorldConstants.TileLength,
                                (y + destPos.Y) * WorldConstants.TileLength,
                                Pallet.IndexToTile(tileIndex), GraphicsUnit.Pixel);
                }
            }

            if (editor)
            {
                if (Obstacles.Count > 0)
                {
                    var obsBrush = new SolidBrush(Color.FromArgb(96, 255, 0, 0));
                    g.FillRectangles(obsBrush, Obstacles.Select(
                        p =>
                        new Rectangle((p.X + destPos.X) * WorldConstants.TileLength, (p.Y + destPos.Y) * WorldConstants.TileLength,
                                      WorldConstants.TileLength, WorldConstants.TileLength)).ToArray());
                }

                // origin
                g.DrawRectangle(Pens.Black, (Origin.X + destPos.X) * WorldConstants.TileLength + 1, (Origin.Y + destPos.Y) * WorldConstants.TileLength + 1,
                                         WorldConstants.TileLength - 2, WorldConstants.TileLength - 2);
            }
        }

        public virtual void Resize(int newX, int newY)
        {
            var tiles = new int[newY, newX];
            foreach (var y in Enumerable.Range(0, Math.Min(Tiles.GetLength(0), newY)))
            {
                foreach (var x in Enumerable.Range(0, Math.Min(Tiles.GetLength(1), newX)))
                {
                    tiles[y, x] = Tiles[y, x];
                }
            }

            Tiles = tiles;
        }

        public void Clear()
        {
            foreach (var y in Enumerable.Range(0, Tiles.GetLength(0)))
            {
                foreach (var x in Enumerable.Range(0, Tiles.GetLength(1)))
                {
                    Tiles[y, x] = -1;
                }
            }

            Origin = Point.Empty;
            Obstacles.Clear();
        }

        public int X { get { return Tiles.GetLength(1); } }
        public int Y { get { return Tiles.GetLength(0); } }
        public int Width { get { return X * WorldConstants.TileLength; } }
        public int Height { get { return Y * WorldConstants.TileLength; } }

        public void Save(string saveDirectory)
        {
            ToXml().Save(Path.Combine(saveDirectory, Name + ".xml"));
        }

        protected virtual XElement ToXml()
        {
            Func<Point, Point> toRelative = point => new Point(point.X - Origin.X, point.Y - Origin.Y);
            var tileMap = new Dictionary<Point, int>();
            foreach (var y in Enumerable.Range(0, Y))
            {
                foreach (var x in Enumerable.Range(0, X))
                {
                    if (Tiles[y, x] >= 0)
                        tileMap.Add(toRelative(new Point(x, y)), Tiles[y, x]);
                }
            }

            Func<Point, object[]> toAttr = point =>
                new object[] { new XAttribute("x", point.X.ToString()), new XAttribute("y", point.Y.ToString()) };

            var xml = new XElement("Object",
                                   new XAttribute("tileset", Pallet.FileName),
                                   new XAttribute("width", X.ToString()),
                                   new XAttribute("height", Y.ToString()),
                                   new XElement("Origin",
                                                new XAttribute("x", Origin.X.ToString()),
                                                new XAttribute("y", Origin.Y.ToString())),
                                   new XElement("Tiles", tileMap.Select(e =>
                                                                        new XElement("Tile",
                                                                                     toAttr(e.Key),
                                                                                     new XAttribute("index", e.Value)))),
                                   new XElement("Obstacles", Obstacles.Select(toRelative).Select(toAttr).Select(e => new XElement("Obstacle", e)))
                );
            return xml;
        }

        public static T Load<T>(string objectXmlPath) where T : MapObject
        {
            if (!File.Exists(objectXmlPath))
                throw new FileNotFoundException();

            var xml = XElement.Load(objectXmlPath);

            var palletName = xml.Attribute("tileset").Value;
            var obj = (T)Activator.CreateInstance(typeof (T), palletName);
            obj.Name = Path.GetFileNameWithoutExtension(objectXmlPath);
            obj.LoadFromXml(xml);

            return obj;
        }

        protected virtual void LoadFromXml(XElement xml)
        {
            Resize(int.Parse(xml.Attribute("width").Value), int.Parse(xml.Attribute("height").Value));
            Clear();

            var originNode = xml.Element("Origin");
            Origin = new Point(int.Parse(originNode.Attribute("x").Value),
                                   int.Parse(originNode.Attribute("y").Value));

            foreach (var tileNode in xml.Descendants("Tile"))
            {
                var tileX = int.Parse(tileNode.Attribute("x").Value) + Origin.X;
                var tileY = int.Parse(tileNode.Attribute("y").Value) + Origin.Y;
                Tiles[tileY, tileX] = int.Parse(tileNode.Attribute("index").Value);
            }

            foreach (var obsNode in xml.Descendants("Obstacle"))
            {
                Obstacles.Add(new Point(
                    int.Parse(obsNode.Attribute("x").Value) + Origin.X,
                    int.Parse(obsNode.Attribute("y").Value) + Origin.Y
                    ));
            }
        }
    }
}

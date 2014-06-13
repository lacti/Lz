using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using LzEngine.Enum;
using LzEngine.Packet;
using LzEngine.Util;

namespace LzServer
{
    internal class ServerContext
    {
        #region General Context

        private readonly Coroutine _coroutine;
        private readonly DataManager _data;
        private readonly Listener _listener;

        #endregion

        #region Game Context

        private readonly ConcurrentDictionary<int /* objectId */, GameObject> _gameObjects = new ConcurrentDictionary<int, GameObject>();
        private readonly NpcAi _npcAi;
        private readonly Random _random = new Random();

        private readonly ConcurrentDictionary<Socket, int /* objectId */> _sockets = new ConcurrentDictionary<Socket, int>();
        private int _objectId;

        #endregion

        internal ServerContext(Listener listener, DataManager data, Coroutine coroutine)
        {
            _listener = listener;
            _data = data;
            _coroutine = coroutine;

            _npcAi = new NpcAi(this);

            LoadObjectsFromStore();
            if (_gameObjects.Count > 0)
            {
                _objectId = _gameObjects.Values.Select(e => e.ObjectId).Max() + 1;
            }
        }

        public DataManager Data
        {
            get { return _data; }
        }

        public IEnumerable<GameObject> GameObjects
        {
            get { return _gameObjects.Values; }
        }

        #region Network

        public void BroadcastPacket(PacketBase packet, Socket exceptSocket = null)
        {
            _listener.BroadcastPacket(packet, exceptSocket);
        }

        #endregion

        #region Entry

        public void AddEntry(Coroutine.LogicEntryDelegate entry)
        {
            _coroutine.AddEntry(entry);
        }

        #endregion

        #region Object

        public void AddGameObject(GameObject gameObject)
        {
            const int retryCount = 1024;
            if (!Enumerable.Range(0, retryCount).Any(_ => _gameObjects.TryAdd(gameObject.ObjectId, gameObject)))
                throw new InvalidOperationException();

            gameObject.IsSpawned = true;
        }

        public GameObject RemoveGameObject(int objectId)
        {
            const int retryCount = 8;
            GameObject removal = null;
            if (!Enumerable.Range(0, retryCount).Any(_ => _gameObjects.TryRemove(objectId, out removal)))
                return null;

            if (removal != null)
                removal.IsSpawned = false;
            return removal;
        }

        public GameObject GetGameObjecct(int objectId)
        {
            const int retryCount = 4;
            GameObject findOne = null;
            return Enumerable.Range(0, retryCount).Any(_ => _gameObjects.TryGetValue(objectId, out findOne))
                       ? findOne
                       : null;
        }

        public void AddPlayer(GameObject playerObject, Socket playerSocket)
        {
            AddGameObject(playerObject);

            const int retryCount = 1024;
            if (!Enumerable.Range(0, retryCount).Any(_ => _sockets.TryAdd(playerSocket, playerObject.ObjectId)))
                throw new InvalidOperationException();
        }

        public GameObject RemovePlayer(Socket playerSocket)
        {
            const int retryCount = 64;
            var objectId = 0;
            return Enumerable.Range(0, retryCount).Any(_ => _sockets.TryRemove(playerSocket, out objectId))
                       ? RemoveGameObject(objectId)
                       : null;
        }

        public GameObject GetGameObject(int objectId)
        {
            GameObject gameObject;
            return _gameObjects.TryGetValue(objectId, out gameObject)
                       ? gameObject
                       : null;
        }

        #endregion

        #region Factory

        private Point GetRandomPosition()
        {
            return new Point(_random.Next(_data.World.Size.width), _random.Next(_data.World.Size.height));
        }

        private DirectionType GetRandomDirection()
        {
            return (DirectionType) _random.Next(4);
        }

        public GameObject NewGameObject(ObjectType type)
        {
            return new GameObject
                {
                    ObjectId = ++_objectId,
                    Name = _data.GetRandomName(),
                    Position = GetRandomPosition(),
                    Direction = GetRandomDirection(),
                    MoveState = MoveStateType.Stop,
                    Type = type
                };
        }

        #endregion

        #region Persistence

        private const string StoreFile = "Store.xml";

        private void LoadObjectsFromStore()
        {
            if (!File.Exists(StoreFile))
                return;

            var type = typeof (GameObject);
            foreach (var objectNode in XElement.Load(StoreFile).Elements("Object"))
            {
                var obj = new GameObject();
                foreach (var prop in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(e => e.CanRead && e.CanWrite))
                {
                    var attribute = objectNode.Attribute(prop.Name);
                    if (attribute == null)
                        continue;

                    if (typeof (Enum).IsAssignableFrom(prop.PropertyType))
                        prop.SetValue(obj, Enum.Parse(prop.PropertyType, attribute.Value), null);
                    else prop.SetValue(obj, DeserializeValue(prop.PropertyType, attribute.Value), null);
                }

                AddGameObject(obj);

                if (obj.Type == ObjectType.Npc)
                    _npcAi.Restore(obj);
            }
        }

        private void SaveObjectsToStore()
        {
            var type = typeof (GameObject);
            var xml = new XElement("Objects",
                         _gameObjects.Values.Select(
                             obj =>
                             new XElement("Object",
                                          type.GetProperties()
                                              .Where(e => e.CanRead && e.CanWrite)
                                              .Select(e => new XAttribute(e.Name,
                                                                          SerializeValue(e.PropertyType, e.GetValue(obj, null))
                                                               )))));

            using (var writer = XmlWriter.Create(StoreFile, new XmlWriterSettings {OmitXmlDeclaration = false, Indent = true}))
                xml.WriteTo(writer);

            Logger.Write("Save Context.");
        }

        private static object DeserializeValue(Type type, string value)
        {
            if (typeof (Point) != type)
                return Convert.ChangeType(value, type);

            var pair = value.Split(',');
            return new Point(int.Parse(pair[0]), int.Parse(pair[1]));
        }

        private static string SerializeValue(Type type, object value)
        {
            if (typeof (Point) != type)
                return Convert.ToString(value);

            var p = (Point) value;
            return string.Format("{0},{1}", p.X, p.Y);
        }

        public IEnumerable<int> SaveContextEntry()
        {
            while (true)
            {
                SaveObjectsToStore();
                yield return 10000;
            }
        }

        #endregion
    }
}
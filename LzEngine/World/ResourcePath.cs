using System;
using System.IO;
using System.Reflection;

namespace LzEngine.World
{
    public class ResourcePath
    {
        public static string Root { get; private set; }

        private const string RootName = "Resources";

        static ResourcePath()
        {
            Root = FindProjectRoot();
        }

        private static string FindProjectRoot()
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            while (currentPath != null)
            {
                var rootPath = Path.Combine(currentPath, RootName);
                if (Directory.Exists(rootPath) && Directory.Exists(Path.Combine(rootPath, TilesetRootName)))
                    return rootPath;
                currentPath = Path.GetDirectoryName(currentPath);
            }
            return ".";
        }

        private const string TilesetRootName = "Tileset";
        private const string ObjectRootName = "Object";
        private const string NpcRootName = "Npc";
        private const string MapRootName = "Map";

        public static string TilesetRoot { get { return Path.Combine(Root, TilesetRootName); } }
        public static string ObjectRoot { get { return Path.Combine(Root, ObjectRootName); } }
        public static string NpcRoot { get { return Path.Combine(Root, NpcRootName); } }
        public static string MapRoot { get { return Path.Combine(Root, MapRootName); } }
    }
}

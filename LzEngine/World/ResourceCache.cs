using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzEngine.World
{
    public static class ResourceCache
    {
        private static readonly object ResourceLock = new object();

        private static readonly Dictionary<string, Bitmap> CharacterCache = new Dictionary<string, Bitmap>();

        public static Bitmap GetCharacter(string resourceName)
        {
            lock (ResourceLock)
            {
                if (!CharacterCache.ContainsKey(resourceName))
                    CharacterCache.Add(resourceName, 
                        (Bitmap)Image.FromFile(Path.Combine(ResourcePath.NpcRoot, resourceName + ".png")));

                return CharacterCache[resourceName];
            }
        }
    }
}

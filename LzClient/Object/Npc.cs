using System.Drawing;
using LzClient.Properties;

namespace LzClient.Object
{
    internal class Npc : Character
    {
        public Npc(int objectId, Point startPoint)
            : base(objectId, Resources.Enemy001, startPoint)
        {
        }
    }
}
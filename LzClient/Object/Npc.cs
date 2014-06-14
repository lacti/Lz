using System.Drawing;
using LzClient.Properties;
using LzEngine.World;

namespace LzClient.Object
{
    internal class Npc : Character
    {
        public Npc(int objectId, Point startPoint)
            : base(objectId, ResourceCache.GetCharacter("036-Mage04"), startPoint)
        {
        }
    }
}
using System.Drawing;
using LzClient.Properties;
using LzEngine.World;

namespace LzClient.Object
{
    internal class Player : Character
    {
        public Player(int objectId, Point startPoint)
            : base(objectId, ResourceCache.GetCharacter("001-Fighter01"), startPoint)
        {
        }
    }
}
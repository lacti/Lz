using System.Drawing;
using LzClient.Properties;

namespace LzClient.Object
{
    internal class Player : Character
    {
        public Player(int objectId, Point startPoint)
            : base(objectId, Resources.Char001, startPoint)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LzClient.Object
{
    interface IDrawObject
    {
        int DrawPrioirty { get; }
        bool IsDrawable { get; }

        void Draw(Graphics g);
    }
}

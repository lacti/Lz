using System.Windows.Forms;

namespace LzClient.Util
{
    internal sealed class CanvasPanel : Panel
    {
        public CanvasPanel()
        {
            DoubleBuffered = true;
        }
    }
}
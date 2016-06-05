using System.Windows.Forms;

namespace RemoteSupport.Client.UI
{
    public class GraphicsPanel : Panel
    {
        public GraphicsPanel()
        {
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
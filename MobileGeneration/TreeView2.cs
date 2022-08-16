using System.Windows.Forms;

namespace MobileGeneration
{
    public class TreeView2 : TreeView
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg != 0x203)
            {
                base.WndProc(ref m);
            }
        }
    }
}

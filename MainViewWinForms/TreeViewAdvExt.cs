using Syncfusion.Windows.Forms.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    public class TreeViewAdvExt : TreeViewAdv
    {
        private const int TVM_GETITEMSTATE = 0x1127;
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            if (m.Msg == TVM_GETITEMSTATE)
            {
                Object test = m;
                string str = "str";
                // Invoke the event delegate
            }
            base.WndProc(ref m);
        }
    }
}

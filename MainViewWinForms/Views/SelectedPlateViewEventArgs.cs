using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class SelectedPlateViewEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }
        public SelectedPlateViewEventArgs(int selectedIndex)
        {
            SelectedIndex = selectedIndex;
        }
    }
}

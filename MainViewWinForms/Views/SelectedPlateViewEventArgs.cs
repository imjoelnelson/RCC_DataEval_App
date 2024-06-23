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
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }
        public SelectedPlateViewEventArgs(int selectedIndex, int rowIndex, int columnIndex)
        {
            SelectedIndex = selectedIndex;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }
}

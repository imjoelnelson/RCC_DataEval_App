using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class DgvCellClickEventArgs : EventArgs
    {
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        public DgvCellClickEventArgs(int rowIndex, int columnIndex)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }
    }
}

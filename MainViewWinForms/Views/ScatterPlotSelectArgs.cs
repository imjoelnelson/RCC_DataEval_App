using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class ScatterPlotSelectArgs : EventArgs
    {
        public int RowIndex { get; set; }
        public int ColIndex { get; set; }

        public ScatterPlotSelectArgs(int row, int col)
        {
            RowIndex = row;
            ColIndex = col;
        }
    }
}

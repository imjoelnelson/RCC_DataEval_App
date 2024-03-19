using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class PkcSelectBoxEventArgs
    {
        public int Row { get; set; }
        public string CartridgeID { get; set; }
        public string[] PkcNames { get; set; }

        public PkcSelectBoxEventArgs(int row, string cartridgeId, string[] pkcNames)
        {
            Row = row;
            CartridgeID = cartridgeId;
            PkcNames = pkcNames;
        }
    }
}

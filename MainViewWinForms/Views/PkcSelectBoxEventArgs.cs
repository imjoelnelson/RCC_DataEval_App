using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class PkcSelectBoxEventArgs
    {
        public string CartridgeID { get; set; }
        public string[] PkcNames { get; set; }

        public PkcSelectBoxEventArgs(string cartridgeId, string[] pkcNames)
        {
            CartridgeID = cartridgeId;
            PkcNames = pkcNames;
        }
    }
}

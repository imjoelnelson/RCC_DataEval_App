using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class ModelPkcSelectBoxArgs : EventArgs
    {
        public string CartridgeID { get; set; }
        public string[] PkcNames { get; set; }

        public ModelPkcSelectBoxArgs(string cartridgeID, string[] pkcNames)
        {
            CartridgeID = cartridgeID;
            PkcNames = pkcNames;
        }
    }
}

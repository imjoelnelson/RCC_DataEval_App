using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class ModelPkcAddRemoveArgs : EventArgs
    {
        public string[] PkcNames { get; set; }

        public ModelPkcAddRemoveArgs(string[] pkcNames)
        {
            PkcNames = pkcNames;
        }
    }
}

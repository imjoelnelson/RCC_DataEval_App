using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public class DataHolder
    {
        public BindingList<Rcc> Rccs { get; set; }
        public List<Rlf> Rlfs { get; set; }

        public DataHolder() { }

        public void CreateObjectsFromFiles(List<string> fileNames)
        {

        }
    }
}

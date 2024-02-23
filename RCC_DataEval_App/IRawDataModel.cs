using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public interface IRawDataModel
    {
        BindingList<Rcc> Rccs { get; set; }
        Dictionary<string, Rlf> Rlfs { get; set; }
        Dictionary<string, PkcReader> Pkcs { get; set; }

        void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex);
    }
}

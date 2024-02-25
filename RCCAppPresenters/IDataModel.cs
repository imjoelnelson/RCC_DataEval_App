using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCCAppPresenters
{
    public interface IDataModel
    {
        BindingList<Rcc> Rccs { get; set; }
        Dictionary<string, Rlf> Rlfs { get; set; }
        Dictionary<string, PkcReader> Pkcs { get; set; }

        event EventHandler RCC_ListChanged;

        void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex);
    }
}

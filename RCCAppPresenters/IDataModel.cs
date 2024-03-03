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
        QcThresholds Thresholds { get; set; }

        void SetThresholds(QcThresholds thresholds);
        void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex);
    }
}

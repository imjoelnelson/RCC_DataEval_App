using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RccAppDataModels
{
    public interface IDataModel
    {
        BindingList<Rcc> Rccs { get; set; }
        BindingSource RccSource { get; set; }
        Dictionary<string, Rlf> Rlfs { get; set; }
        Dictionary<string, string> Pkcs { get; set; }

        event EventHandler RccListChanged;
        event EventHandler AppFolderCreationFailed;
        event EventHandler DspRccsLoaded;

        void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex, QcThresholds thresholds);
        void UpdateThresholds(QcThresholds thresholds);
        void ListChanged();
        void ClearRccs();
        void SortTable(Dictionary<string, bool> sortList);
        void AddPkc(string pkcPath);
        void RemovePkc(string pkcPath);
        void ApplyRlfToDspRccs(List<Rcc> rccs, string cartridgeID, string rlfName, Dictionary<string, ProbeItem> translator);
        string[][] BuildRawDataTable(List<Rcc> rccs, string[] rccProperties);
        string[][] TransformTable(string[][] lines);
    }
}

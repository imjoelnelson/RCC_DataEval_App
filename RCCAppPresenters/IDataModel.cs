﻿using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCCAppPresenters
{
    public interface IDataModel
    {
        BindingList<Rcc> Rccs { get; set; }
        BindingSource RccSource { get; set; }

        event EventHandler RccListChanged;

        void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex, QcThresholds thresholds);
        void UpdateThresholds(QcThresholds thresholds);
        void ListChanged();
        void ClearRccs();
        void SortTable(Dictionary<string, bool> sortList);
    }
}

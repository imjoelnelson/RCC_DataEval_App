using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCCAppPresenters
{
    public interface IMainView
    {
        // Properties
        int FileTypeIndex { get; set; }
        MainViewPresenter Presenter { get; set; }

        // Events
        event EventHandler FilesLoading;
        event EventHandler ThresholdsSet;
        event EventHandler RccListCleared;
        event EventHandler SentToQueue;
        event EventHandler ExportToCsv;
        event EventHandler CreateQCPlot;
        event EventHandler ReorderRows;
        event EventHandler Filter;

        // Methods
        void RccListChanged(List<Rcc2> rccs);
        QcThresholds CollectThresholds();
    }
}

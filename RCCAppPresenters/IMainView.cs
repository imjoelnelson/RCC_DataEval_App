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
        int FileTypeIndex { get; set; }
        MainViewPresenter Presenter { get; set; }

        // Events
        event EventHandler FilesLoading;
        event EventHandler RccListCleared;
        event EventHandler SentToQueue;
        event EventHandler ExportToCsv;
        event EventHandler CreateQCPlot;
        event EventHandler ReorderRows;
        event EventHandler Filter;

        void SetViewDataSource(BindingList<Rcc> list);
        void RccListChanged(List<Rcc> rccs);
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    public interface IRawDataView
    {
        int FileTypeIndex { get; set; }
        RawDataPresenter Presenter { get; set; }

        // Events
        event EventHandler FilesLoading;
        event EventHandler RccListCleared;
        event EventHandler SentToQueue;
        event EventHandler ExportToCsv;
        event EventHandler CreateQCPlot;
        event EventHandler ReorderRows;
        event EventHandler Filter;

        void SetViewDataSource(BindingList<Rcc> list);
        void RccListChanged(List<Rcc> rccs)
    }
}

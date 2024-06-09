using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    public interface IMainView
    {
        // Properties
        int FileTypeIndex { get; set; }
        MainViewPresenter Presenter { get; set; }
        List<string> SelectedProperties { get; set; }
        Dictionary<string, bool> SortList { get; set; }
        List<RlfType> SelectedRlfTypes { get; set; }

        // Events
        event EventHandler FilesLoading;
        event EventHandler FormLoaded;
        event EventHandler RccListCleared;
        event EventHandler ThresholdsUpdated;
        event EventHandler SelectingColumns;
        event EventHandler ColumnsSelected;
        event EventHandler SortClick;
        event EventHandler ThisFormClosed;
        event EventHandler<Views.RccSelectEventArgs> BuildRawCountsTable;
        event EventHandler OpenRawCountTablePreferences;
        event EventHandler<Views.RccSelectEventArgs> DgvSelectionChanged;
        event EventHandler<Views.RccSelectEventArgs> BuildPlateViewTable;
        event EventHandler<Views.RccSelectEventArgs> OpenSampleVSampleScatterDialog;

        // Methods
        void ShowErrorMessage(string message, string caption);
        void SetDgv(Dictionary<string, Tuple<bool, string, int>> properties,
            List<string> selectedProperties, System.Windows.Forms.BindingSource source);
        void DgvSourceChanged(int count);
        QcThresholds CollectThresholds();
        void ShowSelectColumnsDialog(List<Tuple<string, string>> columns, List<string> selectedProperties);
        void FormClose();
        void SaveTable(string[][] tableLines);
        void UpdateTypesPresent(List<RlfType> types);
    }
}

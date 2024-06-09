using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface IRawCountsPlateView
    {
        string SelectedQcProperty { get; set; }
        int Threshold { get; set; }

        event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        event EventHandler ComboBoxSelectionChanged;

        void SetQcPropertySelectorComboItems(string[] items);
        void SetDgv1Values(string[][] mat);
        void SetDgv2Values(string[][] mat);
        void ShowThisDialog();
        void CloseThisDialog();
    }
}

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

        event EventHandler<SelectedPlateViewEventArgs> CalculateQcMetricForSelectedPlateviewPage;
        event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;

        void SetQcPropertySelectorComboItems(string[] items);
        void SetDgv1Values(string[][] mat, int index);
        void SetDgv2Values(string[][] mat, int index);
        void ShowThisDialog();
        void CloseThisDialog();
    }
}

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

        event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        event EventHandler ComboBoxSelectionChanged;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    interface IRawCountsPlateView
    {
        event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        void BindToLaneQcDgv(System.Windows.Forms.BindingSource source);
        void BindToPlateDgv(System.Windows.Forms.BindingSource source);
    }
}

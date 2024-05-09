using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface IRccPropertyConfigView
    {
        List<string> SelectedProperties { get; set; }

        event EventHandler OkButtonClicked;

        void ShowForm();
        void CloseForm();
    }
}

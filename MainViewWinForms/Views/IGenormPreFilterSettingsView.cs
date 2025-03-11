using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface IGenormPreFilterSettingsView
    {
        event EventHandler OKButtonClicked;
        int CountThreshold { get; set; }
        bool UseCountThreshold { get; set; }
        int AvgCountThreshold { get; set; }
        bool UseAvgCountThreshold { get; set; }
        void ShowForm();
        void CloseForm();
    }
}

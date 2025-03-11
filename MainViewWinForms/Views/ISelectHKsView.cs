using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface ISelectHKsView
    {
        event EventHandler SettingsButtonClicked;
        Tuple<string, bool> ErrorMessage { get; set; }
        void UpdateChart(Tuple<string, double?, bool>[] input);
        void ShowForm();
        void CloseForm();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface ISelectHKsView
    {
        void UpdateChart(Tuple<double?, bool>[] input);
        void ShowForm();
    }
}

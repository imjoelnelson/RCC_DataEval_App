using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface IBinnedCountsView
    {
        event EventHandler<Views.SelectedPlateViewEventArgs> ComboBoxIndexChanged;
        void SetChart(double[][] vals, string[] binNames, string[] sampleIDs);
        void SetChartOverlay(string name, double[] vals, string[] sampleIDs);
    }
}

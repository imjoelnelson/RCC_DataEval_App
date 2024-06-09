using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface ISampleVsScatterplotModel
    {
        BindingList<ScatterSelectItem> Samples { get; set; }

        void UpdateSelection(int row, int col);
        ScatterChartDto GetChartData(bool thresholded, int threshold);
    }
}

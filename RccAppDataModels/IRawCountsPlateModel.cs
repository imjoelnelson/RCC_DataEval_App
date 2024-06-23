using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface IRawCountsPlateModel
    {
        string[] PlexQcPropertyList { get; set; }

        string[][] GetSelectedCellQcData(string selectedProperty, int pageIndex);
        string[][] GetSelectedLaneQcData(int pageIndex);
        double[][] GetDataForChart(int pageIndex, string selectedProperty);
        void ExportQcData(int selectedIndex);
    }
}

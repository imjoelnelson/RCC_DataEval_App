using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    /// <summary>
    /// Provides display value for PlateView Well QC DGV binding and provides all QC metric values for QC Summary figure
    /// </summary>
    public class WellQcSummaryItem
    {
        public Dictionary<string, double> NamedQcMetrics { get; set; }
        public int RowIndex { get; set; }
        public int ColumnIndex { get; set; }

        public WellQcSummaryItem(Rcc rcc, int rowIndex, int threshold, string initialQcValue)
        {
            RowIndex = rowIndex;
            ColumnIndex = rcc.LaneID - 1;
            NamedQcMetrics = new Dictionary<string, double>();
            for(int i = 0; i < RawCountsPlateModel.AvailableQcMetrics.Length; i++)
            {
                NamedQcMetrics.Add(RawCountsPlateModel.AvailableQcMetrics[i].Name,
                                    RawCountsPlateModel.AvailableQcMetrics[i].Callback(rcc, rowIndex, rcc.ThisRLF.ThisType == RlfType.DSP, threshold));
            }
        }
    }
}

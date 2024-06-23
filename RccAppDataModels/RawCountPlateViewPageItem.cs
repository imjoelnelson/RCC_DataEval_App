using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class RawCountPlateViewPageItem
    {
        public int Index { get; set; }
        public List<Rcc> Rccs { get; set; }
        public List<WellQcSummaryItem[]> WellData { get; set; }

        public RawCountPlateViewPageItem(int index, List<Rcc> rccs, string initialProperty, int threshold)
        {
            Index = index;
            Rccs = rccs;

            WellData = new List<WellQcSummaryItem[]>();
            for(int i = 0; i < 8; i++)
            {
                WellQcSummaryItem[] temp = new WellQcSummaryItem[12];
                for(int j = 0; j < 12; j++)
                {
                    Rcc tempRcc = rccs.Where(x => x.LaneID == j + 1).FirstOrDefault();
                    temp[j] = tempRcc != null ? new WellQcSummaryItem(tempRcc, i, threshold, initialProperty) : null;
                }
                WellData.Add(temp);
            }
        }
    }
}
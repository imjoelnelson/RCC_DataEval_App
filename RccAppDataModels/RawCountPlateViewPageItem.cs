using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class RawCountPlateViewPageItem
    {
        public int Index { get; set; }
        public List<Rcc> Rccs { get; set; }

        public RawCountPlateViewPageItem(int index, List<Rcc> rccs, string initialProperty)
        {
            Index = index;
            Rccs = rccs;
        }
    }
}

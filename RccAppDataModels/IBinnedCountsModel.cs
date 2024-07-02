using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface IBinnedCountsModel
    {
        List<NCounterCore.Rcc> Rccs { get; set; }
        string[] BinNames { get; set; }
        double[][] CountProportionsMatrix { get; set; }
    }
}

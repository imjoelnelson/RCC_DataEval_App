using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface ISelectHKsModel
    {
        Tuple<string, bool> ErrorMessage { get; set; }
        Tuple<string, double?, bool>[] HousekeeperList { get; set; }

        bool UpdateHousekeeperList(bool useMinCountThreshold, bool useAvgCountThreshold, int countThreshold, int avgCountThreshold);
    }
}

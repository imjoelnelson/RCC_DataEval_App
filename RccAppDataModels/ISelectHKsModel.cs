using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface ISelectHKsModel
    {
        Tuple<string, double?, bool>[] HousekeeperList { get; set; }
    }
}

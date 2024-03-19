using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface IPkcSelectModel
    {
        List<CartridgePkcSelectItem> CartridgePkcs { get; set; }

        void RowPkcsChanged(string cartridgeID, int row, List<Tuple<string, string>> selectedPkcs);
    }
}

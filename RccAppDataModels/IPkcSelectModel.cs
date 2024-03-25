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
        Dictionary<string, string> SavedPkcs { get; set; }
        List<CartridgePkcSelectItem> CartridgePkcs { get; set; }

        void PkcsChanged(string cartridgeID, string[] selectedPkcs);
        void AddPkcToSavedList(string pkcPath);
        void RemovePkcFromSavedList(string pkcKey);
    }
}

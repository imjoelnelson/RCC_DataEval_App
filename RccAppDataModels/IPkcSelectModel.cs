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

        event EventHandler<ModelPkcSelectBoxArgs> SelectedPkcsChanged;
        event EventHandler<ModelPkcAddRemoveArgs> SavedPkcsChanged;

        void AddNewCartridgePkcs(string cartridgeID, string[] selectedPkcs, Dictionary<string, string> translator);
        void ClearSelectedCartridgePkcs(string cartridgeID, string[] pkcsToRemove);
        void AddPkcToSavedList(string pkcPath);
        void RemovePkcFromSavedList(string pkcKey);
    }
}

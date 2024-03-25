
using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class PkcSelectModel : IPkcSelectModel
    {
        public Dictionary<string, string> SavedPkcs { get; set; }
        public List<CartridgePkcSelectItem> CartridgePkcs { get; set; }

        public PkcSelectModel(List<string> CartridgeIDs, Dictionary<string, string> savedPkcs)
        {
            CartridgePkcs = new List<CartridgePkcSelectItem>();
            SavedPkcs = savedPkcs;
        }

        public void PkcsChanged(string cartridgeID, string[] selectedPkcs)
        {
            CartridgePkcSelectItem cartItem = CartridgePkcs.Where(x => x.CartridgeID.Equals(cartridgeID)).FirstOrDefault();
            if (cartItem != null)
            {
                cartItem.AddSelectedPkcs(cartridgeID, selectedPkcs);
                if(cartItem.ErrorMessage != string.Empty)
                {
                    System.Windows.Forms.MessageBox.Show(cartItem.ErrorMessage, "Incompatible PKCs", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
        }

        public void AddPkcToSavedList(string pkcPath)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(pkcPath);
            if(!SavedPkcs.ContainsKey(name))
            {
                SavedPkcs.Add(name, pkcPath);
            }
        }

        public void RemovePkcFromSavedList(string pkcKey)
        {
            if(SavedPkcs.ContainsKey(pkcKey))
            {
                SavedPkcs.Remove(pkcKey);
            }
        }
    }
}
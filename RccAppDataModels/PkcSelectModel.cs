
using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RccAppDataModels
{
    public class PkcSelectModel : IPkcSelectModel
    {
        public Dictionary<string, string> SavedPkcs { get; set; } // Changed SavedPkcs to BindingList (_SavedPKCs) but need to refactor a lot so keeping both for now
        public List<CartridgePkcSelectItem> CartridgePkcs { get; set; }

        public event EventHandler<ModelPkcSelectBoxArgs> SelectedPkcsChanged;
        public event EventHandler<ModelPkcAddRemoveArgs> SavedPkcsChanged;

        public PkcSelectModel(List<string> cartridgeIDs, Dictionary<string, string> savedPkcs)
        {
            CartridgePkcs = new List<CartridgePkcSelectItem>(cartridgeIDs.Count);
            for(int i = 0; i < cartridgeIDs.Count; i++)
            {
                CartridgePkcSelectItem item = new CartridgePkcSelectItem(cartridgeIDs[i]);
                item.PkcsChanged += new EventHandler<ModelPkcSelectBoxArgs>(CartridgeItem_PkcsChanged);
                CartridgePkcs.Add(item);
            }
            SavedPkcs = savedPkcs;
        }

        /// <summary>
        /// Method called by Presenter when it has recieved notice that selected PKCs have changed for a cartridge
        /// </summary>
        /// <param name="cartridgeID">The cartridge for which PKCs have changed</param>
        /// <param name="selectedPkcs">The paths of the newly selected PKC files</param>
        public void AddNewCartridgePkcs(string cartridgeID, string[] selectedPkcNames, Dictionary<string, string> translator)
        {
            CartridgePkcSelectItem cartItem = CartridgePkcs.Where(x => x.CartridgeID.Equals(cartridgeID)).FirstOrDefault();
            if (cartItem != null)
            {
                cartItem.AddSelectedPkcs(cartridgeID, selectedPkcNames, SavedPkcs);
                if(cartItem.ErrorMessage != string.Empty)
                {
                    System.Windows.Forms.MessageBox.Show(cartItem.ErrorMessage, "Incompatible PKCs", System.Windows.Forms.MessageBoxButtons.OK);
                }
            }
        }

        public void ClearSelectedCartridgePkcs(string cartridgeID, string[] pkcsToRemove)
        {
            CartridgePkcSelectItem item = CartridgePkcs.Where(x => x.CartridgeID.Equals(cartridgeID)).FirstOrDefault();
            if(item == null)
            {
                throw new Exception("Item to clear cannot be null");
            }
            item.ClearSelectedPkcs(pkcsToRemove, SavedPkcs);
        }

        public void AddPkcToSavedList(string pkcPath)
        {
            string name = System.IO.Path.GetFileNameWithoutExtension(pkcPath);
            if(!SavedPkcs.ContainsKey(name))
            {
                // Validate that file is a properly formatted PKC
                try
                {
                    _ = new PkcReader(pkcPath);
                }
                catch(Exception er)
                {
                    System.Windows.Forms.MessageBox.Show($"{name} could not be added due to the following error:\r\n\r\n{er.Message}");
                    return;
                }
                // Add to SavedPkcs dictionary
                SavedPkcs.Add(name, pkcPath);
                // Update View via Presenter
                SavedPkcsChanged.Invoke(this, new ModelPkcAddRemoveArgs(SavedPkcs.Select(x => x.Key).ToArray()));
            }
        }

        public void RemovePkcFromSavedList(string pkcKey)
        {
            if(SavedPkcs.ContainsKey(pkcKey))
            {
                SavedPkcs.Remove(pkcKey);
                // Update View via Presenter
                SavedPkcsChanged.Invoke(this, new ModelPkcAddRemoveArgs(SavedPkcs.Select(x => x.Key).ToArray()));
            }
        }

        private void CartridgeItem_PkcsChanged(object sender, ModelPkcSelectBoxArgs e)
        {
            SelectedPkcsChanged.Invoke(sender, e);
        }
    }
}

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
        public List<CartridgePkcSelectItem> CartridgePkcs { get; set; }

        public PkcSelectModel(List<string> CartridgeIDs)
        {
            CartridgePkcs = new List<CartridgePkcSelectItem>();
        }

        public void RowPkcsChanged(string cartridgeID, int row, List<Tuple<string, string>> selectedPkcs)
        {
            CartridgePkcSelectItem cartItem = CartridgePkcs.Where(x => x.CartridgeID.Equals(cartridgeID)).FirstOrDefault();
            RowPkcSelectItem rowItem = cartItem.SelectedPkcsPerRow[row];
            rowItem.SetRowPkcSelectItem(cartridgeID, row, selectedPkcs);
        }
    }
}

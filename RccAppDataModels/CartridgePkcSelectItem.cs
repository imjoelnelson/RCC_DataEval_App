using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class CartridgePkcSelectItem
    {
        public string CartridgeID { get; set; }
        /// <summary>
        /// Collection containing Tuple<"PKC name", "PKC full path">
        /// </summary>
        public RowPkcSelectItem[] SelectedPkcsPerRow { get; set; }

        public CartridgePkcSelectItem(string cartridgeID)
        {
            CartridgeID = cartridgeID;
            SelectedPkcsPerRow = new RowPkcSelectItem[8];
            for (int i = 0; i < 8; i++)
            {
                SelectedPkcsPerRow[i] = new RowPkcSelectItem();
            }
        }

        public void AddSelectedPkcs(string cartridgeID, int row, List<Tuple<string, string>> selectedPkcs)
        {
            SelectedPkcsPerRow[row].SetRowPkcSelectItem(cartridgeID, row, selectedPkcs);
        }

        //SelectedPkcs = selectedPkcs;
        //    List<PkcReader> readers = SelectedPkcs.Select(x => new PkcReader(x.Item2)).ToList();
        //Collector = new PkcCollector(readers);
        //    if(Collector.OverlappingIDs.Count > 0)
        //    {
        //        ErrorMessage = $"In cartridge, {CartridgeID}, the following DSP_IDs were used by more than one PKC:\r\n{string.Join("\r\n", Collector.OverlappingIDs)}\r\n\r\nCheck that you selected the correct PKCs.";
        //    }
    }
}

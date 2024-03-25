using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class CartridgePkcSelectItem
    {
        // ID of the cartridge that PKCs are being selected for
        public string CartridgeID { get; set; }
        /// <summary>
        /// Collection containing Tuple<"PKC name", "PKC full path">
        /// </summary>
        public PkcCollector Collector { get; set; }
        /// <summary>
        /// Error message indicating DSP_IDs that are incompatible with the selected PKCs
        /// </summary>
        public string ErrorMessage { get; set; }

        public CartridgePkcSelectItem(string cartridgeID)
        {
            CartridgeID = cartridgeID;
            ErrorMessage = string.Empty;
        }

        public void AddSelectedPkcs(string cartridgeID, string[] selectedPkcs)
        {
            IEnumerable<PkcReader> readers = selectedPkcs.Select(x => new PkcReader(x));
            Collector = new PkcCollector(readers);
            if(Collector.OverlappingIDs.Count > 0)
            {
                ErrorMessage = $"In cartridge, {CartridgeID}, the following DSP_IDs were used by more than one PKC:\r\n{string.Join("\r\n", Collector.OverlappingIDs)}\r\n\r\nCheck that you selected the correct PKCs.";
            }
            else
            {
                ErrorMessage = string.Empty;
            }
        }
    }
}

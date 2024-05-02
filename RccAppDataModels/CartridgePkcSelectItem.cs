using NCounterCore;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// Collection of readers, indexed with their PKC's name, added for this cartridge
        /// </summary>
        public Dictionary<string, PkcReader> PkcReaders { get; set; }
        /// <summary>
        /// Collection containing Tuple<"PKC name", "PKC full path">
        /// </summary>
        public PkcCollector Collector { get; set; }
        /// <summary>
        /// Error message indicating DSP_IDs that are incompatible with the selected PKCs
        /// </summary>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Event to send back, via model and presenter, the PKCs that were successfully added, to keep view up to date with model state
        /// </summary>
        public event EventHandler<ModelPkcSelectBoxArgs> PkcsChanged;

        public CartridgePkcSelectItem(string cartridgeID)
        {
            PkcReaders = new Dictionary<string, PkcReader>();
            CartridgeID = cartridgeID;
            ErrorMessage = string.Empty;
        }

        public void AddSelectedPkcs(string cartridgeID, string[] pkcNames, Dictionary<string, string> translator)
        {
            // Add PKC name and reader (created from file) if PKC not previously added
            for(int i = 0; i < pkcNames.Length; i++)
            {
                if(!PkcReaders.ContainsKey(pkcNames[i]))
                {
                    PkcReaders.Add(pkcNames[i], new PkcReader(translator[pkcNames[i]]));
                }
            }

            // Create collector from cumulated PKCs and check for compatibility
            Collector = new PkcCollector(PkcReaders.Select(x => x.Value));
            if(Collector.OverlappingIDs.Count > 0)
            {
                ErrorMessage = $"In cartridge, {CartridgeID}, the following DSP_IDs were used by more than one PKC:\r\n{string.Join("\r\n", Collector.OverlappingIDs)}\r\n\r\nCheck that you selected the correct PKCs.";
            }
            else
            {
                ErrorMessage = string.Empty;
            }

            // Send message back through Model and Presenter to View
            var args = new ModelPkcSelectBoxArgs(cartridgeID, PkcReaders.Select(x => x.Key).ToArray());
            PkcsChanged.Invoke(this, args);
        }

        public void ClearSelectedPkcs(string[] pkcNames, Dictionary<string, string> translator)
        {
            // Remove selected from PkcReader dictionary
            for (int i = 0; i < pkcNames.Length; i++)
            {
                if(PkcReaders.ContainsKey(pkcNames[i]))
                {
                    PkcReaders.Remove(pkcNames[i]);
                }
            }

            // Update collector
            if(PkcReaders.Count > 0)
            {
                Collector = new PkcCollector(PkcReaders.Select(x => x.Value));
            }

            // Send message back through Model and Presenter to View
            var args = new ModelPkcSelectBoxArgs(CartridgeID, PkcReaders.Select(x => x.Key).ToArray());
            PkcsChanged.Invoke(this, args);
        }
    }
}

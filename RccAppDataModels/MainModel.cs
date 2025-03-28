using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class MainModel
    {
        // Properties

        // 1 - SampleItem list (all sample properties and sample main key)
        // 2 - ProbeItem list (all probe properties, including RlfId, codeclass?, analyte?, thresholded designations? and main key)
        // 3 - RlfItem List (for keeping track for loading RLFs after probes created and for multi-RLF stuff; include a main key
        // 4 - CountItem List (count item = sampleID, probeID, raw count (for miRNA, should include lig background subtracted values; for proprietary purposes), list of normfactors (that include calibration factored in), one for each normalization group, including group IDs for each)
        // 5 - NormalizationGroup list (NormGroup = method, samples, probes, calibration factor info

        public List<RccItem> Rccs { get; set; }
        public List<RlfItem> Rlfs { get; set; }
        
        public MainModel()
        {
            Rccs = new List<RccItem>();
            Rlfs = new List<RlfItem>();
        }

        public void AddRccs(List<string> rccFilenames)
        {
            int currentCount = Rccs.Count;
            // Add new RccItems for each RCC file
            Rccs.AddRange(rccFilenames.Select((x, i) => new RccItem(x, i + currentCount)));
            // Retrieve RLFs, probes, and counts for each new RccItem
            for(int i = currentCount; i < Rccs.Count; i++)
            {
                RccItem item = new RccItem(rccFilenames[i], i + currentCount);
                if(item.ThisRlfType != RlfType.DSP)
                {
                    string currentRccRlfName = Rccs[i].GetStringAnnot("RlfName");
                    if (!Rlfs.Any(x => x.Name == currentRccRlfName))
                    {
                        int currentRlfId = Rlfs.Count;
                        Rlfs.Add(new RlfItem(currentRccRlfName, currentRlfId, false));

                        // Add probes to probe list (including RLF ID)
                        // Add Count data using probe and sample IDs
                    }
                    else
                    {
                        // Get RLF ID from RLFs in list
                        // Get probe ID info based on RLF
                        // Add Count data using probe and sample IDs
                    }
                }
                else
                {
                    // Get PKC Reader info first
                    // Then Add probes (or should DSP probe info be saved to DB?)
                    // Then add count data using probe and sample ID
                }
            }
        }
    }
}
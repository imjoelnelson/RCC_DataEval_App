using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class SelectHKsModel : ISelectHKsModel
    {
        public Tuple<string, double?, bool>[] HousekeeperList { get; set; }

        private int MinCountThreshold { get; set; }
        private int MinAvgCountThreshold { get; set; }
        private List<NCounterCore.Rcc> Rccs { get; set; }
        private List<NCounterCore.ProbeItem> HKs { get; set; }
        

        public SelectHKsModel(List<NCounterCore.Rcc> rccs, int[] props)
        {
            Rccs = rccs;
            MinCountThreshold = props[1];

            var rlfs = rccs.Select(x => x.ThisRLF).Distinct(new NCounterCore.RlfEqualityComparer());

            // If multiple RLFs detected, identify HKs that overlap and set HKnames to the overlap
            if (rlfs.Count() > 1)
            {
                if (rlfs.Any(x => !x.FromRlfFile))
                {
                    System.Windows.Forms.MessageBox.Show("RCCs from multiple RLFs were selected but some RLFs were not loaded. Please load RLFs for all RCCs first and then re-try",
                                                         "RLF Not Loaded",
                                                         System.Windows.Forms.MessageBoxButtons.OK);
                    // CALL VIEW AND MODEL DISPOSE METHODS
                    return;
                }
                HKs = rlfs.SelectMany(x => x.Probes.Where(y => y.Value.CodeClass.Equals("Housekeeping")))
                          .Where(x => rlfs.All(y => y.Probes.Select(z => x.Value.ProbeID).Contains(x.Value.ProbeID)))
                          .Select(x => x.Value)
                          .Distinct(new NCounterCore.ProbeItemEqualityComparer())
                          .ToList();
            }
            else
            {
                HKs = rlfs.First().Probes.Where(x => x.Value.CodeClass.Equals("Housekeeping"))
                                         .Select(x => x.Value)
                                         .ToList();
            }

            UpdateHousekeeperList(props);
        }

        public void UpdateHousekeeperList(int[] props)
        {
            List<string> preFiltered = new List<string>(HKs.Count);
            if(props[0] > -1)
            {
                preFiltered.AddRange(Rccs.SelectMany(x => x.ProbeCounts)
                                         .Where(y => HKs.Select(z => z.TargetName).ToList().Contains(y.Key) && y.Value < props[0])
                                         .Select(x => x.Key));
            }
            if(props[1] > -1)
            {
                preFiltered.AddRange(HKs.Where(x => Rccs.SelectMany(y => y.ProbeCounts)
                                                        .Where(z => z.Key.Equals(x.TargetName))
                                                        .Select(z => z.Value)
                                                        .Average() < props[1])
                                         .Select(x => x.TargetName));
            }

            var hkNames = HKs.Where(x => !preFiltered.Contains(x.TargetName));

            if(hkNames.Count() < 2)
            {
                System.Windows.Forms.MessageBox.Show("Fewer than two housekeeping genes remain after prefiltering using the user-defined count thresholds. Either decrease the thresholds or identify potential housekeepers with sufficient counts that have not been annotated as housekeepers in the RLF.",
                                                     "Insufficient HKs Remaining",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
            }
            
            NCounterCore.GenNormCalculation geNorm = new NCounterCore.GenNormCalculation(Rccs, hkNames.Select(x => x.TargetName).ToList(), 2);
            HousekeeperList = geNorm.HousekeeperList;
        }
    }
}

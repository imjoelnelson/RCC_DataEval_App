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
        /// <summary>
        /// List of all non-prefiltered housekeepers including gene name (item1), pairwise variation (item2), and bool indicating recommendation to retain (item3)
        /// </summary>
        public Tuple<string, double?, bool>[] HousekeeperList { get; set; }
        /// <summary>
        /// For passing exceptions and error messages to presenter and ultimately, the view; item2 bool indicates whether view should close after error
        /// </summary>
        public Tuple<string, bool> ErrorMessage { get; set; }
        /// <summary>
        /// Selected list of RCCs for GeNorm to be calculated on
        /// </summary>
        private List<NCounterCore.Rcc> Rccs { get; set; }
        /// <summary>
        /// List of housekeeping probe items to be considered taken from all included RLFs
        /// </summary>
        private List<NCounterCore.ProbeItem> HKs { get; set; }
        

        public SelectHKsModel(List<NCounterCore.Rcc> rccs)
        {
            Rccs = rccs;

            var rlfs = rccs.Select(x => x.ThisRLF).Distinct(new NCounterCore.RlfEqualityComparer());

            // If multiple RLFs detected, identify HKs that overlap and set HKnames to the overlap
            if (rlfs.Count() > 1)
            {
                if (rlfs.Any(x => !x.FromRlfFile))
                {
                    ErrorMessage = Tuple.Create("RCCs from multiple RLFs were selected but some RLFs were not loaded. Please load RLFs for all RCCs first and then re-try",
                                                true);
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
        }

        /// <summary>
        /// Public method called by presenter to prefilter HKs and then run GeNorm calculation, resulting in updated HousekeeperList; returns true if no error
        /// </summary>
        /// <param name="minCountThreshold">Threshold that the counts for a housekeeping gene must be above in all samples for it to be retained</param>
        /// <param name="minAvgCountThreshold">Threshold that the average count for a housekeeping gene must have across all samples for it to be retained</param>
        /// <returns>Bool indicating if no error occurred</returns>
        public bool UpdateHousekeeperList(bool useMinCountThreshold, bool useAvgCountThreshold, int countThreshold, int avgCountThreshold)
        {
            List<string> preFiltered = new List<string>(HKs.Count);
            if(useMinCountThreshold)
            {
                preFiltered.AddRange(Rccs.SelectMany(x => x.ProbeCounts)
                                         .Where(y => HKs.Select(z => z.TargetName).ToList().Contains(y.Key) && y.Value < countThreshold)
                                         .Select(x => x.Key));
            }
            if(useAvgCountThreshold)
            {
                preFiltered.AddRange(HKs.Where(x => Rccs.SelectMany(y => y.ProbeCounts)
                                                        .Where(z => z.Key.Equals(x.TargetName))
                                                        .Select(z => z.Value)
                                                        .Average() < avgCountThreshold)
                                         .Select(x => x.TargetName));
            }

            var hkNames = HKs.Where(x => !preFiltered.Contains(x.TargetName));

            if(hkNames.Count() < 2)
            {
                ErrorMessage = Tuple.Create("Fewer than two housekeeping genes remain after prefiltering using the user-defined count thresholds. Either decrease the thresholds or identify potential housekeepers with sufficient counts that have not been annotated as housekeepers in the RLF.",
                                                     false);
                return false;
            }
            
            NCounterCore.GenNormCalculation geNorm = new NCounterCore.GenNormCalculation(Rccs, hkNames.Select(x => x.TargetName).ToList(), 2);
            HousekeeperList = geNorm.HousekeeperList;
            return true;
        }
    }
}

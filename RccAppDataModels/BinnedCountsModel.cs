using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class BinnedCountsModel : IBinnedCountsModel
    {
        public List<Rcc> Rccs { get; set; }
        public string[] Well { get; set; }
        public string[] BinNames { get; set; }
        public double[][] CountProportionsMatrix { get; set; } // Each double[] is from a count bin with each double inside being from one sample

        /// <summary>
        /// Constructor for non-sample multiplexed RCCs
        /// </summary>
        /// <param name="rccs"></param>
        /// <param name="cutoffs"></param>
        public BinnedCountsModel(List<Rcc> rccs, int[] cutoffs)
        {
            Rccs = rccs;
            CountProportionsMatrix = new double[cutoffs.Length][];
            BinNames = new string[cutoffs.Length];
            // Get included content for each represented RLF
            Dictionary<string, HashSet<string>> rlfContent = Rccs.Select(x => x.ThisRLF)
                                                                 .Distinct()
                                                                 .ToDictionary(x => x.Name,
                                                                               x => x.Probes.Where(y => y.Value.CodeClass.StartsWith("E") ||
                                                                                                        y.Value.CodeClass.StartsWith("H") ||
                                                                                                        y.Value.CodeClass.StartsWith("I"))
                                                                                            .Select(y => y.Value.TargetName)
                                                                                            .ToHashSet());
            // Get count bin proportions for included content of each RCC
            for(int i = 0; i < cutoffs.Count(); i++)
            {
                // Get count ranges for current bin and add bin names
                int[] minMax;
                if(i == cutoffs.Length - 1)
                {
                    minMax = new int[] { cutoffs[i], 5000000 };
                    BinNames[i] = $"> {cutoffs[i]}";
                }
                else
                {
                    minMax = new int[] { cutoffs[i], cutoffs[i + 1] };
                    BinNames[i] = $"{cutoffs[i] + 1}-{cutoffs[i + 1]}";
                }

                // Get proportions of targets in bin for each sample
                CountProportionsMatrix[i] = new double[Rccs.Count];
                for (int j = 0; j < rccs.Count; j++)
                {
                    IEnumerable<int> tempCounts = rccs[j].ProbeCounts.Where(x => rlfContent[rccs[j].ThisRLF.Name].Contains(x.Key))
                                                                     .Select(x => x.Value);
                    CountProportionsMatrix[i][j] = tempCounts.Where(x => x > minMax[0] && x <= minMax[1]).Count() / (double)tempCounts.Count();
                }
            }
        }

        public BinnedCountsModel(List<Rcc> rccs, int[] cutoffs, bool isDsp)
        {
            Rccs = rccs;
            if(Rccs.Count > 12)
            {
                System.Windows.Forms.MessageBox.Show("This figure can only be used with 12 RCCs when sample-multiplexed RCCs (DSP or PlexSet) are used. Select only 12 or fewer sample-multiplexed RCCs and try again.",
                                                     "Too Many RCCs",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
            }
            CountProportionsMatrix = new double[cutoffs.Length][];
            BinNames = new string[cutoffs.Length];
            
        }
    }
}

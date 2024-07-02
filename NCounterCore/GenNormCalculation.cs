using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class GenNormCalculation
    {
        /// <summary>
        /// The list of HKs selected based on stability (M value)
        /// </summary>
        Tuple<string, bool>[] HousekeeperList { get; set; }
        /// <summary>
        /// List of the log2 ratios of norm factors calculated with each HK in the m value-ordered list added vs. the previous factor without that HK
        /// </summary>
        double[] Variation { get; set; }

        /// <summary>
        /// Constructor with housekeeping genes (HKs) selected in another method/class
        /// </summary>
        /// <param name="rccs">The RCCs providing the data used to evaluate the stability of the HKs</param>
        /// <param name="hkProbes">Genes identified as housekeepers; crucial that these are based on established HKs unless this calculation is used as part of a RefFinder implementation</param>
        public GenNormCalculation(List<Rcc> rccs, List<string> hkNames, List<string>endoNames)
        {

            Tuple<string, double>[] mValues = hkNames.Select(x => Tuple.Create(x,
                                                                               GetMValue(rccs, x, endoNames.Where(y => y != x).ToList())))
                                                     .OrderBy(z => z.Item2)
                                                     .ToArray();
            string[] orderedNames = mValues.Select(x => x.Item1).ToArray();

            // Get norm factor ratios (i.e. norm factor ratio without/with each HK in the m value ordered list (including all HKs up to that HK in the list))
            List<double> variation = new List<double>(mValues.Length - 1);
            for (int i = mValues.Length - 1; i > 0; i--)
            {
                double[] normFactorsWith = GetNormFactors(rccs, orderedNames.Take(i + 1).ToList());
                double[] normFactorsWithout = GetNormFactors(rccs, orderedNames.Take(i).ToList());

                double var = MathNet.Numerics.Statistics.Statistics.StandardDeviation(Enumerable.Range(0, rccs.Count).Select(
                                                                                      x => Math.Log(normFactorsWithout[x] / normFactorsWith[x], 2)));
                variation.Add( var);
            }

            // Find the minimum value of the norm factor ratio list to identify the selected HK cutoff
            double min = variation.Min();
            int ind = mValues.Length - variation.Select((x, i) => x == min ? i : -1)
                                                .Where(y => y > -1)
                                                .First(); // Index of first gene (in m value ordered list) where variation value == min
            // Ensures at least 3 genes selected, regardless of threshold
            int thresh = ind > 2 ? ind : 3;

            // Create an HK list with indicator of rejected HKs
            HousekeeperList = Enumerable.Range(0, thresh).Select(x => Tuple.Create(orderedNames[x], x < thresh)).ToArray();
        }

        /// <summary>
        /// Calculate variability of each HK with respect to all of the other genes to get stability measure; use to order genes by stability
        /// </summary>
        /// <param name="rccs">The source of gene counts for each sample</param>
        /// <param name="gene">The HK that M value is being calculated for</param>
        /// <param name="otherGenes">List of all the other genes in the codeset</param>
        /// <returns></returns>
        private double GetMValue(List<Rcc> rccs, string gene, List<string> otherGenes)
        {
            if(rccs.Any(x => x == null))
            {
                if (rccs.Where(x => x != null).Count() < 10)
                {
                    System.Windows.Forms.MessageBox.Show("GeNorm requires at least 10 samples for valid results. Fewer than 10 samples were selected or not null",
                                                         "Insufficient Samples Selected",
                                                         System.Windows.Forms.MessageBoxButtons.OK);
                    return -1;
                }
            }
            
            // Array to hold standard deviation of ratios of the hk gene vs each other gene across the samples
            double[] sd = new double[otherGenes.Count];

            // Calculate standard deviation of ratio between HK and other genes for each sample
            for (int i = 0; i < otherGenes.Count; i++)
            {
                double[] ratios = new double[rccs.Count];
                for(int j = 0; j < rccs.Count; j++)
                {
                    // Get ratio of the hk gene vs. one of the other genes in the codeset across all samples
                    ratios[j] = Math.Log(rccs[j].ProbeCounts[gene] / rccs[j].ProbeCounts[otherGenes[i]], 2);
                }
                sd[i] = MathNet.Numerics.Statistics.Statistics.StandardDeviation(ratios);
            }

            return sd.Average();
        }

        /// <summary>
        /// Calcualte Norm Factors, used for permutation test
        /// </summary>
        /// <param name="rccs">The source of the gene count data</param>
        /// <param name="selectedHKs">Selected HKs for the specific round, ordered by ascending M value</param>
        /// <returns>An array of norm factors, one for each RCC</returns>
        private double[] GetNormFactors(List<Rcc> rccs, List<string> selectedHKs)
        {
            double[] geomeans = new double[rccs.Count];
            for (int i = 0; i < rccs.Count; i++)
            {
                geomeans[i] = Util.GetGeoMean(rccs[i].ProbeCounts.Where(x => selectedHKs.Contains(x.Key))
                                     .Select(x => (double)x.Value).ToArray());
            }

            return geomeans;
        }


    }
}

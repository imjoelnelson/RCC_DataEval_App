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
        /// List of all included hks as Tuple of item1 = string indicating the gene name and item2 = boolean indicating whether HK is included (true) or discarded (false)
        /// </summary>
        public Tuple<string, bool>[] HousekeeperList { get; set; }

        /// <summary>
        /// Array of HKs in order removed (most stable == last) as a tuple including name and Norm Factor Variation w/ vs w/o (Item3); 
        /// Last two genes' variation will be null as they cannot be calculated
        /// </summary>
        public List<Tuple<string, double?>> Rank { get; set; }

        /// <summary>
        /// Constructor with housekeeping genes (HKs) selected in another method/class
        /// </summary>
        /// <param name="rccs">The RCCs providing the data used to evaluate the stability of the HKs</param>
        /// <param name="hkProbes">Genes identified as housekeepers; crucial that these are based on established HKs unless this calculation is used as part of a RefFinder implementation</param>
        public GenNormCalculation(List<Rcc> rccs, List<string> hkNames, int minHKs)
        {
            // Convert RCCs to log2 transformed matrix, genes in rows, samples in columns
            Tuple<string, double[]>[] mat = ConvertRccsToLog2Matrix(rccs, hkNames);

            if (mat.Any(x => x.Item2.Length != mat[0].Item2.Length))
            {
                throw new Exception("Method, GetMValue, cannot operate on a jagged array.");
            }

            if (mat[0].Item2.Length < 10)
            {
                System.Windows.Forms.MessageBox.Show("GeNorm requires at least 10 samples for valid results. Select more samples and try again.",
                                                        "Insufficient Samples Selected",
                                                        System.Windows.Forms.MessageBoxButtons.OK);
                return;
            }

            if(minHKs < 2)
            {
                System.Windows.Forms.MessageBox.Show("Minimum number of housekeeping genes retained cannot be less than two. Setting minHKs to 2.",
                                                     "MinHKs Set Too Low",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
                minHKs = 2;
            }

            //*****    RANK AND CALCULATE NORM FACTOR VARIATION

            Rank = new List<Tuple<string, double?>>(mat.Length);
            // Iterate through HKs and calculate rank and norm factor variation w/ and w/o
            while (hkNames.Count > 0)
            {
                if(hkNames.Count > 2)
                {
                    // Find the least stable gene remaining
                    Tuple<string, double>[] mValues = hkNames.Select(x => Tuple.Create(x,
                                                                                       GetMValue(mat, x, hkNames.Where(y => y != x).ToList())))
                                                             .OrderBy(x => x.Item2)
                                                             .ToArray();
                    string leastStable = mValues[mValues.Length - 1].Item1;
                    string[] withoutLeastStable = hkNames.Where(x => !x.Equals(leastStable)).ToArray();

                    // Caclulate normfactor variation with and without least stable gene remaining
                    double[] meansWith = Enumerable.Range(0, mat[0].Item2.Length).Select(x => mat.Where(z => hkNames.Contains(z.Item1))
                                                                                                 .Select(y => y.Item2[x])
                                                                                                 .Average())
                                                                                 .ToArray();
                    double[] meansWithout = Enumerable.Range(0, mat[0].Item2.Length).Select(x => mat.Where(y => withoutLeastStable.Contains(y.Item1))
                                                                                                    .Select(y => y.Item2[x])
                                                                                                    .Average())
                                                                                    .ToArray();

                    Rank.Add(Tuple.Create(leastStable, (double?)MathNet.Numerics.Statistics.Statistics.StandardDeviation(meansWithout.Select((x, y) => x - meansWith[y]))));

                    int ind = hkNames.Select((x, y) => x.Equals(leastStable) ? y : -1)
                                     .Where(z => z > -1)
                                     .First();
                    if (ind != hkNames.Count - 1)
                    {
                        hkNames[ind] = hkNames[hkNames.Count - 1];
                        hkNames.RemoveAt(hkNames.Count - 1);
                    }
                    else
                    {
                        hkNames.RemoveAt(hkNames.Count - 1);
                    }
                }
                else
                {
                    // Variation cannot be calculated for last two genes as it requires 3 genes; -1 is a place holder
                    Rank.AddRange(hkNames.Select(x => Tuple.Create<string, double?>(x, null)));
                    break;
                }
            }

            //*****     SELECT HOUSEKEEPERS     *****

            var min = Rank.Select(x => x.Item2).Min();
            var ind2 = Rank.Select((x, i) => x.Item2.Equals(min) ? i : -1).Max();
            // Retained == true for all HKs with an index greater than ind2 (i.e. the inflection point of the Order Removed vs. Norm Factor Variation Plot)
            HousekeeperList = Rank.Select((x, i) => i > ind2 ? Tuple.Create(x.Item1, true) : Tuple.Create(x.Item1, false)).ToArray();
        }

        /// <summary>
        /// Convert a list of RCCs to an expression matrix (List of Tuples where each Tuple == gene name (string) and expression across all samples (double[])
        /// </summary>
        /// <param name="rccs">List of RCCs to convert</param>
        /// <param name="selectedTargets">List of targets to include in the matrix</param>
        /// <returns>An array of string, double[] tuples, each representing expression for a target (item1 = name) across all samples (item2 = counts)</returns>
        private Tuple<string, double[]>[] ConvertRccsToLog2Matrix(List<Rcc> rccs, List<string> selectedTargets)
        {
            Tuple<string, double[]>[] retMatrix = new Tuple<string, double[]>[selectedTargets.Count];

            for(int i = 0; i < selectedTargets.Count; i++)
            {
                double[] vals = new double[rccs.Count];
                for(int j = 0; j < rccs.Count; j++)
                {
                    int val;
                    bool present = rccs[j].ProbeCounts.TryGetValue(selectedTargets[i], out val);
                    if(!present)
                    {
                        throw new Exception($"Targets {selectedTargets[i]} was not present in all RCCs included. selectedTargets should be present in all RCCs");
                    }
                    vals[j] = val > 0 ? Math.Log(val, 2) : 0;
                }
                retMatrix[i] = (Tuple.Create(selectedTargets[i], vals));
            }

            return retMatrix;
        }

        /// <summary>
        /// Calculate variability of each HK with respect to all of the other genes to get stability measure M (From GeNorm Algorithm); use to order genes by stability; 
        /// </summary>
        /// <param name="rccs">The source of gene counts for each sample</param>
        /// <param name="gene">The HK that M value is being calculated for</param>
        /// <param name="otherGenes">List of all the other genes in the dataset</param>
        /// <returns></returns>
        private double GetMValue(Tuple<string, double[]>[] mat, string gene, List<string> otherGenes)
        {
            // Commented out here because this check is handled already in the GeNormCalculation  method
            //if(!mat.All(x => x.Item2.Length == mat[0].Item2.Length)) 
            //{
            //    throw new Exception("GetMValue Error: Input 'array' is jagged. Method requires all input double[] to have same length");
            //}
            // Holder for squared delta from mean for calculating sd of diffs from 'gene'
            double[][] squareDiffs = new double[otherGenes.Count][];
            // All samples' expression for 'gene' (the gene M is being calculated for)
            var thisGeneVector = mat.Where(x => x.Item1.Equals(gene)).FirstOrDefault().Item2;
            if(thisGeneVector == null)
            {
                throw new Exception("GetMValue Error: input 'gene' is not present in input matrix");
            }
            
            // Get diffs for all samples between 'gene' and all other genes
            for (int i = 0; i < otherGenes.Count; i++)
            { 
                var ratios = new double[mat[0].Item2.Length];
                var thatGeneVector = mat.Where(x => x.Item1.Equals(otherGenes[i])).FirstOrDefault().Item2;
                if(thatGeneVector == null)
                {
                    throw new Exception($"GetMValue Error: gene, {otherGenes[i]}, is not present in input matrix.");
                }
                for (int j = 0; j < ratios.Length; j++)
                {
                    // Get ratio of the hk gene vs. one of the other genes in the codeset across all samples
                    ratios[j] = (double)thisGeneVector[j] - thatGeneVector[j];
                }
                // Get mean for the row
                var mean = ratios.Average();
                // Get squared diffs from the mean
                squareDiffs[i] = ratios.Select(x => Math.Pow(x - mean, 2)).ToArray();
            }

            // Get 0.5 root of summed square diffs (variance) divided by degrees of freedom = standard deviation for each row/gene
            var rootSums = squareDiffs.Select(x => Math.Pow(x.Sum() / (x.Length - 1), 0.5));
            // Get average of row/gene SDs (i.e. M value)
            var meanSD = rootSums.Average();

            return meanSD;
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

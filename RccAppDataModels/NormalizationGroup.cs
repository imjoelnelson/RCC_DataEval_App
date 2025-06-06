using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    /// <summary>
    /// Class for defining a normalized dataset; RCCs (raw data and metadata), NormFactors, Normalized expression 
    /// </summary>
    public class NormalizationGroup
    {
        /// <summary>
        /// RCCs for the samples to be normalized together
        /// </summary>
        public List<Rcc> Rccs { get; set; }
        /// <summary>
        /// Dictionary for translating between int probeID and the probe object
        /// </summary>
        public Dictionary<int, ProbeItem> ProbeList { get; set; }
        /// <summary>
        /// List containing norm counts for each gene in each sample in the dataset; Item1 = GeneID, Item2 = SampleID, Item3 = norm count
        /// </summary>
        public List<NormDataItem> NormTable { get; set; }
        /// <summary>
        /// List containing norm factors for each sample; item1 = sampleID, item2 = normFactor
        /// </summary>
        public Dictionary<int, double> NormFactors { get; set; }
        /// <summary>
        /// List containing the names of the reference genes included for normalization
        /// </summary>
        public List<string> RefGenesUsed { get; set; }
        /// <summary>
        /// The name of the norm method used
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// Indicates object had an error/handled exception requiring it to close
        /// </summary>
        public bool ErrorState { get; set; }
        /// <summary>
        /// Message if object had an error/handled exception occur
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Indicates whether user should be prompted to identify RCCs that should be used for calibrating overlapping probes in cross-codeset analysis
        /// </summary>
        private bool PromptForCalibration { get; set; }

        /// <summary>
        /// List of norm methods available for generating normalization factors
        /// </summary>
        public static Tuple<string, NormMethod>[] NormMethods = new Tuple<string, NormMethod>[]
        {
            new Tuple<string, NormMethod>("GeNorm_GeoMean", GetGeNormGeoMean)
        };

        /// <summary>
        /// Methods for creating normalization factors given a set of rccs and possibly other data (as a generic object)
        /// </summary>
        /// <param name="rccs">List of samples/RCCs to create normalization factors for </param>
        /// <param name="arg">Additional data needed for specifying the normalization</param>
        /// <returns>A Tuple containing item1 = dictionary of norm factors and item2 = a list of ref genes used for normalization</returns>
        public delegate Tuple<Dictionary<int, double>, List<string>> NormMethod(List<Rcc> rccs, object arg);

        /// <summary>
        /// Default constructor for norm groups
        /// </summary>
        /// <param name="rccs">List of RCCs representing the samples to be normalized; Rcc.ID used in Item2 field of NormTable</param>
        /// <param name="probeNames">List of probes included, called out here instead of in RCC.ThisRlf in case data is from cross-codeset</param>
        public NormalizationGroup(List<Rcc> rccs, List<string> probes, string method)
        {
            Rccs = rccs;
            List<Rlf> rlfs = Rccs.Select(x => x.ThisRLF).Distinct().ToList();
            if(rlfs.Count < 1) { throw new Exception("Normalization Error: No RLF object detected for selected RCCs"); }
            if(rlfs.Count > 1)
            {
                // Logic to see if RLF files loaded
                if(rlfs.All(x => x.FromRlfFile))
                {
                    PromptForCalibration = true;
                }
                else
                {
                    PromptForCalibration = false;
                    ErrorState = true;
                    ErrorMessage = $"Selected RCCs included multiple RLFs however one or more of these RLFs were not loaded and thus there is insufficient information to merge the data from these codesets. Please load RLF files for {string.Join(", ", rlfs.Where(x => !x.FromRlfFile).Select(x => x.Name))} and try again.";
                    return;
                }
            }
            else
            {
                PromptForCalibration = false;
            }

            ProbeList = new Dictionary<int, ProbeItem>();

            // Fill probe list; if PromptForCalibration, check if probe has already been added (for probes that occur in both RLFs)
            if (PromptForCalibration)
            {
                HashSet<string> probeNamesAdded = new HashSet<string>(rlfs.SelectMany(x => x.Probes).Count());
                for(int j = 0; j < rlfs.Count; j++)
                {
                    List<string> keys = rlfs[j].Probes.Select(x => x.Key).ToList();
                    for (int i = 0; i < keys.Count; i++)
                    {
                        if (!probeNamesAdded.Contains(keys[i]))
                        {
                            // Check probeID
                            ProbeItem probe1 = ProbeList.Where(x => x.Value.TargetName.Equals(keys[i])).Select(x => x.Value).First();
                            ProbeItem probe2 = rlfs[j].Probes[keys[i]];
                            if(probe1.ProbeID != probe2.ProbeID)
                            {
                                ProbeList.Add(i, rlfs[j].Probes[keys[i]]);
                            }
                        }
                    }
                }
            }
            else
            {
                List<string> keys = rlfs[0].Probes.Select(x => x.Key).ToList();
                for (int i = 0; i < keys.Count; i++)
                {
                    ProbeList.Add(i, rlfs[0].Probes[keys[i]]);
                }
            }

            // Set the method for normalization
            NormMethod normMethod = NormMethods.Where(x => x.Item1 == method).Select(x => x.Item2).FirstOrDefault();
            if(normMethod == null)
            {
                System.Windows.Forms.MessageBox.Show($"A normalization method could not be inferred from the given method name: {method}",
                                                     "Unknown Normalization Method",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
                return;
            }
            Method = method;

            // Get norm factors and probes used for normalization
            var normResults = normMethod(rccs, probes);
            NormFactors = normResults.Item1;
            RefGenesUsed = normResults.Item2;

            // Get normalized count items (properties = sampleID, probeID, normalized count, raw count)
            NormTable = new List<NormDataItem>(probes.Count * rccs.Count);
            for(int i = 0; i < rlfs.Count; i++)
            {
                List<Rcc> tempRccs = rccs.Where(x => x.RlfName == rlfs[i].Name).ToList();
                for(int j = 0; j < tempRccs.Count; j++)
                {
                    int sampID = tempRccs[i].ID;
                    foreach(KeyValuePair<string, int> count in tempRccs[i].ProbeCounts)
                    {
                        int probeID = rlfs[i].Probes.Where(x => x.Value.TargetName == count.Key)
                                                    .Select(x => x.Value.PrimaryKey)
                                                    .First();
                        NormTable.Add(new NormDataItem(sampID, probeID, count.Value * NormFactors[sampID], (double)count.Value));
                    }
                }
            }
        }

        /// <summary>
        /// Norm method using GeNorm to select HKs and geomean of selected HKs to calculate norm factors for single RLF datasets
        /// </summary>
        /// <param name="rccs">The RCCs to calculate normalization factors for</param>
        /// <param name="arg">List of probe names for targets remaining after thresholding</param>
        /// <returns>Collection of norm factors with their RCC ids as an array of int,double Tuples</returns>
        public static Tuple<Dictionary<int, double>, List<string>> GetGeNormGeoMean(List<Rcc> rccs, object arg)
        {
            IEnumerable<Rlf> rlf = rccs.Select(x => x.ThisRLF).Distinct();
            if(rlf.Count() > 1)
            {
                throw new Exception("GetGeNormGeoMean method is intended for single RLF datasets only.");
            }
            // Cast arg to probeNames; should be a list of targets that meet threshold requirements and are present in all included RLFs
            List<string> probeNames = (List<string>)arg;
            // Get included housekeeping targets
            var hks = rccs[0].ThisRLF.Probes.Where(x => (x.Value.CodeClass.StartsWith("H") 
                                                      || x.Value.CodeClass.StartsWith("I"))
                                                      && probeNames.Contains(x.Key));
            if(hks.Count() < 1)
            {
                System.Windows.Forms.MessageBox.Show("No housekeeping targets remain after thresholding. Either lower the threshold or choose a method that does not depend on known housekeeping targets.",
                                                     "Warning: No HKs",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            // Get included endogenous targets
            var endos = rccs[0].ThisRLF.Probes.Where(x => (x.Value.CodeClass.StartsWith("E")
                                                        || x.Value.CodeClass.StartsWith("H") 
                                                        || x.Value.CodeClass.StartsWith("I"))
                                                        && probeNames.Contains(x.Key));
            if(endos.Count() < 1)
            {
                System.Windows.Forms.MessageBox.Show("No endogenous targets remain after thresholding.",
                                                     "Warning: No Expression",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            // Get GeNorm object containing selected HKs based on expression stability as per GeNorm alogrithm
            GenNormCalculation genNorm = new GenNormCalculation(rccs, hks.Select(x => x.Key).ToList(), 3); // default min HKs set at 3
            // Get selected HK target name list
            var keeperNames = genNorm.HousekeeperList.Select(x => x.Item1);
            // Get sample HK geomeans
            Tuple<int, double>[] geomeans = rccs.Select(x => new Tuple<int, double>(x.ID, 
                                                                                    NCounterCore.Util.GetGeoMean(
                                                                                            x.ProbeCounts.Where(y => keeperNames.Contains(y.Key))
                                                                                                         .Select(y => y.Value)
                                                                                                         .ToArray()))).ToArray();
            if(geomeans.Any(x => x.Item2 == 0))
            {
                List<Tuple<int, double>> temp = new List<Tuple<int, double>>(geomeans.Length);
                foreach(Tuple<int, double> t in geomeans)
                {
                    temp.Add(new Tuple<int, double>(t.Item1, -1));
                }
                geomeans = temp.ToArray();
            }
            // Average of geomeans
            double avgGeo = geomeans.Select(x => x.Item2).Average();
            // Get return value
            Dictionary<int, double> retVal = geomeans.ToDictionary(x => x.Item1, x => avgGeo / x.Item2);
            return new Tuple<Dictionary<int, double>, List<string>>(retVal, genNorm.HousekeeperList.Where(x => x.Item3).Select(x => x.Item1).ToList());
        }

        public static Dictionary<int, double> GetTop100GeoMean(List<Rcc> rccs, object arg)
        {
            List<string> selectedProbes = (List<string>)arg;
            Tuple<int, double>[] geomeans = rccs.Select(x => new Tuple<int, double>(x.ID,
                                                                                    NCounterCore.Util.GetGeoMean(
                                                                                        x.ProbeCounts.Where(y => selectedProbes.Contains(y.Key))
                                                                                                     .Select(y => y.Value)
                                                                                                     .ToArray()))).ToArray();
            if (geomeans.Any(x => x.Item2 == 0))
            {
                List<Tuple<int, double>> temp = new List<Tuple<int, double>>(geomeans.Length);
                foreach (Tuple<int, double> t in geomeans)
                {
                    temp.Add(new Tuple<int, double>(t.Item1, -1));
                }
                geomeans = temp.ToArray();
            }
            double avgGeo = geomeans.Select(x => x.Item2).Average();
            Dictionary<int, double> retVal = geomeans.ToDictionary(x => x.Item1, x => avgGeo / x.Item2);

            return retVal;
        }
    }
}

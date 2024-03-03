using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public enum PropertyType { StringType, Numeric, Boolean }
    public class Rcc2
    {
        #region Infrastructure properties
        public string RccReadErrorMessage { get; private set; }
        public bool RlfImported { get; private set; }
        public bool IsSprint { get; private set; }
        public Rlf ThisRlf { get; private set; }
        private Dictionary<string, double> Thresholds { get; set; }
        #endregion

        #region Code Summary Properties
        /// <summary>
        /// Dictionary of key=TargetName, Value=ProbeItem for all probes in the codeset
        /// </summary>
        public Dictionary<string, int> ProbeCounts { get; private set; }
        #endregion

        #region Display properties
        public static Dictionary<string, PropertyType> PropertyOrder = new Dictionary<string, PropertyType>
        {
            { "FileName", PropertyType.StringType },
            { "SampleName", PropertyType.StringType },
            { "LaneID", PropertyType.Numeric },
            { "CartridgeID", PropertyType.StringType },
            { "CartridgeBarcode", PropertyType.StringType },
            { "Rlf", PropertyType.StringType },
            { "Date", PropertyType.StringType },
            { "Instrument", PropertyType.StringType },
            { "StagePostion", PropertyType.StringType },
            { "Owner", PropertyType.StringType },
            { "Comments", PropertyType.StringType },
            { "FovCount", PropertyType.Numeric },
            { "FovCounted", PropertyType.Numeric },
            { "PctFovCounted", PropertyType.Numeric },
            { "PctFovPass", PropertyType.Boolean },
            { "BindingDensity", PropertyType.Numeric },
            { "BindingDensityPass", PropertyType.Boolean },
            { "PosLinearity", PropertyType.Numeric },
            { "PosLinearityPass", PropertyType.Boolean },
            { "Lod", PropertyType.Numeric },
            { "LodPass", PropertyType.Boolean },
            { "PctAboveThresh", PropertyType.Numeric },
            { "GeoMeanOfPos", PropertyType.Numeric },
            { "GeoMeanOfHks", PropertyType.Numeric }
        };

        public static Dictionary<string, string> PropertyDisplayNames = new Dictionary<string, string>
        {
            { "FileName", "File Name" },
            { "SampleName", "Sample Name" },
            { "LaneID", "Lane ID" },
            { "CartridgeID", "Cartridge ID" },
            { "CartridgeBarcode", "Cartridge Barcode" },
            { "Rlf", "Rlf" },
            { "Date", "Date" },
            { "Instrument", "Instrument" },
            { "StagePostion", "Stage Position" },
            { "Owner", "Owner" },
            { "Comments", "Comments" },
            { "FovCount", "FovCount" },
            { "FovCounted", "FovCounted" },
            { "PctFovCounted", "Percent FOV Counted" },
            { "PctFovPass", "Imaging QC Flag" },
            { "BindingDensity", "Binding Density" },
            { "BindingDensityPass", "Spot Density QC Flag" },
            { "PosLinearity", "POS Control Linearity" },
            { "PosLinearityPass", "Linearity QC Flag" },
            { "Lod", "LOD" },
            { "LodPass", "LOD QC Flag" },
            { "PctAboveThresh", "Percent Genes Above Threshold" },
            { "GeoMeanOfPos", "GeoMean of POS Controls" },
            { "GeoMeanOfHks", "GeoMean of Normalization Genes" }
        };

        /// <summary>
        /// Dictionary for accessing string-based properties by name
        /// </summary>
        public Dictionary<string, string> StringProperties { get; private set; }
        /// <summary>
        /// Dictionary for accessing numeric-based properties by name
        /// </summary>
        public Dictionary<string, double> DoubleProperties { get; private set; }
        /// <summary>
        /// Dictionary for accessing bool properties by name
        /// </summary>
        public Dictionary<string, bool> BoolProperties { get; private set; }
        #endregion

        public Rcc2(string filePath, Dictionary<string, Rlf> rlfs)
        {
            // Initialize property collections
            StringProperties = new Dictionary<string, string>
                (PropertyOrder.Where(x => x.Value.Equals(PropertyType.StringType)).Count());
            DoubleProperties = new Dictionary<string, double>
                (PropertyOrder.Where(x => x.Value.Equals(PropertyType.Numeric)).Count());
            BoolProperties = new Dictionary<string, bool>
                (PropertyOrder.Where(x => x.Value.Equals(PropertyType.Boolean)).Count());

            // Add filename property
            StringProperties.Add("FileName", Path.GetFileNameWithoutExtension(filePath));

            // Read file
            List<string> lines = new List<string>();
            try
            {
                string[] tempLines = File.ReadAllLines(filePath);
                lines.AddRange(tempLines);
            }
            catch (Exception er)
            {
                RccReadErrorMessage = $"{er.Message}\r\n{er.StackTrace}";
                return;
            }

            // Identify section indices
            int[] indices = GetSectionIndices(lines);

            // Get SystemType
            IsSprint = GetSystemType(lines.Skip(indices[0] + 1).Take(indices[1] - (indices[0] + 1)).ToList());

            // Get properties from Sample_Attributes section
            GetSampleAttributes(lines.Skip(indices[2] + 1).Take(indices[3] - (indices[2] + 1)).ToList(), rlfs);

            // Get properties from Lane_Attribute section
            GetLaneAttributes(lines.Skip(indices[4] + 1).Take(indices[5] - (indices[4] + 1)).ToList());

            // Get codesummary
            if (ThisRlf.FromRlfFile) // When RLF loaded first
            {
                ProbeCounts = GetProbeCounts(lines.Skip(indices[6] + 1).Take(indices[7] - (indices[6] + 1)).ToList(),
                    ThisRlf.ThisType);
            }
            else // When no relevant RLF loaded
            {
                var result = ProcessCodeSum(lines.Skip(indices[6] + 2).Take(indices[7] - (indices[6] + 1)).ToList(),
                    ThisRlf.ThisType);
                ProbeCounts = result.Item2;
                ThisRlf.AddProbesFromRcc(result.Item1);
            }

            // Check nProbes
            if (ThisRlf.Probes.Count != ProbeCounts.Count)
            {
                throw new Exception("RLF vs. RCC count of probes do not match");
            }

            //        Get QC flags
            // Imaging flags
            DoubleProperties.Add("PctFovCounted", DoubleProperties["FovCount"] > 0 ? 
                DoubleProperties["FovCounted"] / DoubleProperties["FovCount"] : -1);
            BoolProperties.Add("PctFovPass", DoubleProperties["PctFovCounted"] > Util.ImagingPassThresh);
            // Spot density flags
            BoolProperties.Add("BindingDensityPass",  IsSprint ? DoubleProperties["BindingDensity"] <= Util.DensityPassThreshS :
                DoubleProperties["BindingDensity"] <= Util.DensityPassThreshDA);
            if (ThisRlf.ThisType != RlfType.Generic || ThisRlf.ThisType != RlfType.DSP
                || ThisRlf.ThisType != RlfType.PlexSet) // Controls processed differently for these assays
            {
                // Pos linearit flag
                IEnumerable<string> posNames = ThisRlf.Probes.Values
                    .Where(x => x.CodeClass.Equals("Positive"))
                    .OrderBy(x => x.TargetName)
                    .Select(y => y.TargetName);
                DoubleProperties.Add("PosLinearity", GetPosLinearity(posNames, ProbeCounts));
                BoolProperties.Add("PosLinearityPass", 
                                   DoubleProperties["PosLinearity"] >= Util.PosLinearityPassThresh);
                // Lod flag
                DoubleProperties.Add("Lod", GetLod(ThisRlf.Probes.Values
                    .Where(x => x.CodeClass.Equals("Negative"))
                    .Select(x => x.TargetName), ProbeCounts));
                BoolProperties.Add("LodPass", ProbeCounts["POS_E(0.5)"] > DoubleProperties["Lod"]);
                // Pos Geomeans
                DoubleProperties.Add("GeoMeanOfPos", 
                    Util.GetGeoMean(ProbeCounts.Where(x => posNames.Contains(x.Key)).Select(x => x.Value).ToArray()));
            }
            // Pct genes above threshold
            DoubleProperties.Add("PctAboveThresh", ProbeCounts.Where(x => 
                x.Value >= Util.CountThreshold).Count() / ProbeCounts.Count);
            // Hk Geomeans
            IEnumerable<string> hkNames = ThisRlf.Probes.Values
                    .Where(x => x.CodeClass.Equals("Housekeeping"))
                    .OrderBy(x => x.TargetName)
                    .Select(y => y.TargetName);
            if(hkNames.Count() > 0)
            {
                DoubleProperties.Add("GeoMeanOfHks",
                    Util.GetGeoMean(ProbeCounts.Where(x => hkNames.Contains(x.Key)).Select(x => x.Value).ToArray()));
            }
        }

        #region Methods
        /// <summary>
        /// Gets indices for start and end of RCC sections
        /// </summary>
        /// <param name="lines">Lines from the RCC file</param>
        /// <returns>int[10] of section start/stop indices {Start and stop for Header, SampleAttributes, LaneAttributes, CodeSummary, and Messages</returns>
        private int[] GetSectionIndices(List<string> lines)
        {
            int[] indices = new int[10];
            int i = 0;
            int end = lines.Count - 1;
            while (i != end)
            {
                if (lines[i].StartsWith("<"))
                {
                    if (lines[i].StartsWith("</"))
                    {
                        if (lines[i].StartsWith("</H"))
                        {
                            indices[1] = i;
                        }
                        else if (lines[i].StartsWith("</S"))
                        {
                            indices[3] = i;
                        }
                        else if (lines[i].StartsWith("</L"))
                        {
                            indices[5] = i;
                        }
                        else if (lines[i].StartsWith("</C"))
                        {
                            indices[7] = i;
                        }
                        else if (lines[i].StartsWith("</M"))
                        {
                            indices[9] = i;
                        }
                    }
                    else
                    {
                        if (lines[i].StartsWith("<H"))
                        {
                            indices[0] = i;
                        }
                        else if (lines[i].StartsWith("<S"))
                        {
                            indices[2] = i;
                        }
                        else if (lines[i].StartsWith("<L"))
                        {
                            indices[4] = i;
                        }
                        else if (lines[i].StartsWith("<C"))
                        {
                            indices[6] = i;
                        }
                        else if (lines[i].StartsWith("<M"))
                        {
                            indices[8] = i;
                        }
                    }
                }
                i++;
            }

            return indices;
        }

        /// <summary>
        /// Used for setting IsSprint
        /// </summary>
        /// <param name="lines">Lines from Header section</param>
        /// <returns>bool indicating IfSprint</returns>
        private bool GetSystemType(List<string> lines)
        {
            string line = lines.Where(x => x.StartsWith("Sy")).FirstOrDefault();
            return line != null ? line.Split(',')[1].Equals("Gen3") : false;
        }

        /// <summary>
        /// Gets attributes from RCC SampleAttributes section
        /// </summary>
        /// <param name="lines">lines from the SampleAttributes section</param>
        /// <param name="rlfs">Form1's RLF collection</param>
        private void GetSampleAttributes(List<string> lines, Dictionary<string, Rlf> rlfs)
        {
            StringProperties.Add("SampleName", Util.GetValue(lines.Where(x => x.StartsWith("I")).FirstOrDefault()));
            StringProperties.Add("Owner", Util.GetValue(lines.Where(x => x.StartsWith("O")).FirstOrDefault()));
            StringProperties.Add("Comments", Util.GetValue(lines.Where(x => x.StartsWith("C")).FirstOrDefault()));
            StringProperties.Add("Date", Util.GetValue(lines.Where(x => x.StartsWith("D")).FirstOrDefault()));
            string rlfString = Util.GetValue(lines.Where(x => x.StartsWith("G")).FirstOrDefault());
            StringProperties.Add("Rlf", rlfString);
            if (rlfString != string.Empty)
            {
                if (rlfString.StartsWith("DSP_"))
                {
                    // Get Readers via UI
                    //ThisRLF = new Rlf(readers)
                }
                else
                {
                    bool found = rlfs.TryGetValue(rlfString, out Rlf thisRlf);
                    if (found)
                    {
                        ThisRlf = thisRlf;
                        RlfImported = false;
                    }
                    else
                    {
                        thisRlf = new Rlf(rlfString, found); // Add probe collection via public method in Rlf class after codesummary section deliniated below
                        RlfImported = true;
                    }
                    ThisRlf = thisRlf;
                }
            }
        }

        /// <summary>
        /// Gets attributes from RCC LaneAttributes section
        /// </summary>
        /// <param name="lines">Lines from the LaneAttributes section</param>
        private void GetLaneAttributes(List<string> lines)
        {
            // Get attributes
            DoubleProperties.Add("LaneID", Util.SafeParseDouble(Util.GetValue(lines.Where(x => x.StartsWith("I")).FirstOrDefault())));
            DoubleProperties.Add("FovCount", Util.SafeParseDouble(Util.GetValue(lines.Where(x => x.StartsWith("FovCount,")).FirstOrDefault())));
            DoubleProperties.Add("FovCounted", Util.SafeParseDouble(Util.GetValue(lines.Where(x => x.StartsWith("FovCounted")).FirstOrDefault())));
            StringProperties.Add("Instrument", Util.GetValue(lines.Where(x => x.StartsWith("Sc")).FirstOrDefault()));
            StringProperties.Add("StagePosition", Util.GetValue(lines.Where(x => x.StartsWith("St")).FirstOrDefault()));
            DoubleProperties.Add("BindingDensity", Util.SafeParseDouble(Util.GetValue(lines.Where(x => x.StartsWith("B")).FirstOrDefault())));
            StringProperties.Add("CartridgeID", Util.GetValue(lines.Where(x => x.StartsWith("CartridgeI")).FirstOrDefault()));
            StringProperties.Add("CartridgeBarcode", Util.GetValue(lines.Where(x => x.StartsWith("CartridgeB")).FirstOrDefault()));
        }

        /// <summary>
        /// Gets probe count diciontary from codesummary section; used when RLF was loaded from RLF file
        /// </summary>
        /// <param name="lines">Line from codesummary section</param>
        /// <returns>Dicionary of probe name/count pairs</string></returns>
        private Dictionary<string, int> GetProbeCounts(List<String> lines, RlfType type)
        {
            var probeCounts = new Dictionary<string, int>(lines.Count);
            if (type == RlfType.Gx || type == RlfType.CNV || type == RlfType.Generic)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        probeCounts.Add(bits[1], Util.SafeParseInt(bits[3]));
                    }
                }
            }
            else if (type == RlfType.miRNA || type == RlfType.miRGE)
            {
                int posAVal = GetPosAVal(lines);
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        if (bits[0].EndsWith("ous1"))
                        {
                            string name = bits[1].Split('|')[0];
                            int val = Convert.ToInt32(Util.SafeParseInt(bits[3]) 
                                - (ThisRlf.Probes[name].CorrectionCoefficient * posAVal));

                            probeCounts.Add(name, val);
                        }
                        else
                        {
                            probeCounts.Add(bits[1], Util.SafeParseInt(bits[3]));
                        }
                    }
                }
            }
            else if (type == RlfType.DSP)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        // Add only if probe was present in selected PKC(s)
                        ProbeItem item;
                        bool found = ThisRlf.Probes.TryGetValue(bits[1], out item);
                        if (found)
                        {
                            probeCounts.Add(item.TargetName, Util.SafeParseInt(bits[3]));
                        }
                    }
                }
            }
            else // For PlexSet
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        // Ensure target name is concatenated with row ID to provide unique key
                        string name;
                        if (bits[0].StartsWith("E") || bits[0].StartsWith("H"))
                        {
                            string row = bits[0].Substring(bits[0].Length - 2, 1);
                            name = $"{bits[1]}_{row}";
                        }
                        else
                        {
                            name = bits[1];
                        }
                        probeCounts.Add(name, Util.SafeParseInt(bits[3]));
                    }
                }
            }

            return probeCounts;
        }

        private int GetPosAVal(List<string> lines)
        {
            string posALine = lines.Where(x => x.StartsWith("Pos") 
                && x.Substring(9, 5).Equals("POS_A")).FirstOrDefault();
            if (posALine == null) { return -1; }
            return Util.SafeParseInt(posALine.Split(',')[3]);
        }

        /// <summary>
        /// creates probelist for RLF and Probe Count dictionary for RCC from codesummary section; for when RLF not pulled from file
        /// </summary>
        /// <param name="lines">Lines from the CodeSummary section</param>
        /// <returns>Tuple containing Probe dictionary and count dictionary</returns>
        private Tuple<Dictionary<string, ProbeItem>, Dictionary<string, int>> ProcessCodeSum(List<string> lines, RlfType type)
        {
            var item1 = new Dictionary<string, ProbeItem>(lines.Count);
            var item2 = new Dictionary<string, int>(lines.Count);
            for (int i = 0; i < lines.Count; i++)
            {
                string[] bits = lines[i].Split(',');
                if (bits.Length > 3)
                {
                    ProbeItem item = new ProbeItem(bits[0], bits[1], bits[2], ThisRlf.ThisType);
                    if (type != RlfType.PlexSet)
                    {
                        item1.Add(item.TargetName, item);
                        item2.Add(item.TargetName, Util.SafeParseInt(bits[3]));
                    }
                    else
                    {
                        string row = Rlf.PsTranslateRow[item.PlexRow];
                        item1.Add($"{item.TargetName}_{row}", item);
                        item2.Add($"{item.TargetName}_{row}", Util.SafeParseInt(bits[3]));
                    }
                }
            }

            return Tuple.Create(item1, item2);
        }

        /// <summary>
        /// Gets Pearson R^2 of log2 ERCC POS control counts vs. their ideal concentrations
        /// </summary>
        /// <param name="posNames">Targets names of the top 5 ERCC POS controls</param>
        /// <param name="counts">ProbeCount collection from RCC</param>
        /// <returns></returns>
        private double GetPosLinearity(IEnumerable<string> posNames, Dictionary<string, int> counts)
        {
            IEnumerable<int> vals0 = posNames.Select(x => counts[x]);
            double[] valsx = vals0.Select(x => x > 0 ? Math.Log(x, 2.0) : 0).ToArray();
            double[] valsy = new double[] { 7, 5, 3, 1, -1 };
            return valsx.Length > 5 ? 
                MathNet.Numerics.Statistics.Correlation.Pearson(valsx.Take(5), valsy) : -1.0;
        }

        /// <summary>
        /// Gets LOD value for LOD QC
        /// </summary>
        /// <param name="negNames">Target names of the ERCC NEG controls</param>
        /// <param name="counts">ProbeCount collection from RCC</param>
        /// <returns></returns>
        private double GetLod(IEnumerable<string> negNames, Dictionary<string, int> counts)
        {
            IEnumerable<double> logs = negNames.Select(x => Convert.ToDouble(counts[x]));
            return logs.Average() + 
                (Util.LODSDCoeff * MathNet.Numerics.Statistics.Statistics.StandardDeviation(logs));
        }
        #endregion
    }
}

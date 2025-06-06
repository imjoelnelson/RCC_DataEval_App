using System;
using System.Collections.Generic;
using NCounterCore;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class RccToDatabase
    {
        public bool ReadError { get; private set; }
        public string ReadErrorMessage { get; private set; }
        public string RccAddToDbDirective { get; private set; }

        private Rlf ThisRlf { get; set; }
        private bool RlfImported { get; set; }
        
        public RccToDatabase(string filePath, Dictionary<string, Rlf> rlfs, QcThresholds thresholds)
        {
            // Read in data
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            List<string> lines = new List<string>();
            try
            {
                string[] tempLines = File.ReadAllLines(filePath);
                lines.AddRange(tempLines);
                ReadError = false;
            }
            catch (Exception er)
            {
                ReadErrorMessage = $"{er.Message}\r\n{er.StackTrace}";
                ReadError = true;
                return;
            }

            // Identify section indices
            int[] indices = GetSectionIndices(lines);

            // Get SystemType
            bool isSprint = GetSystemType(lines.Skip(indices[0] + 1).Take(indices[1] - (indices[0] + 1)).ToList());

            // Get Sample Attributes from Sample_Attribute section
            //GetSampleAttributes(lines.Skip(indices[2] + 1).Take(indices[3] - (indices[2] + 1)).ToList(), rlfs);
            List<string> sampleLines = lines.Skip(indices[2] + 1).Take(indices[3] - (indices[2] + 1)).ToList();
            string sampleName = Util.GetValue(sampleLines.Where(x => x.StartsWith("I")).FirstOrDefault());
            string owner = Util.GetValue(sampleLines.Where(x => x.StartsWith("O")).FirstOrDefault());
            string comments = Util.GetValue(sampleLines.Where(x => x.StartsWith("C")).FirstOrDefault());
            string date = Util.GetValue(sampleLines.Where(x => x.StartsWith("D")).FirstOrDefault());
            string rlfString = Util.GetValue(sampleLines.Where(x => x.StartsWith("G")).FirstOrDefault());
            string rlfName;
            if (rlfString != string.Empty)
            {
                bool found = rlfs.TryGetValue(rlfString, out Rlf thisRlf);
                if (found)
                {
                    ThisRlf = thisRlf;
                    rlfName = thisRlf.Name;
                    RlfImported = false;
                }
                else
                {
                    thisRlf = new Rlf(rlfString, found); // Add probe collection via public method in Rlf class after codesummary section deliniated below
                    rlfName = thisRlf.Name;
                    RlfImported = true;
                }
                ThisRlf = thisRlf;
            }
            else
            {
                ReadError = true;
                ReadErrorMessage = $"RCC file, {fileName.Substring(fileName.LastIndexOf('\\'))}, does not specify an RLF name and cannot be loaded.";
                rlfName = string.Empty;
                return;
            }

            // Get Lane Attributes from Lane_Attribute section
            List<string> laneLines = lines.Skip(indices[4] + 1).Take(indices[5] - (indices[4] + 1)).ToList();
            int laneID = Util.SafeParseInt(Util.GetValue(laneLines.Where(x => x.StartsWith("I")).FirstOrDefault()));
            int fovCount = Util.SafeParseInt(Util.GetValue(laneLines.Where(x => x.StartsWith("FovCount,")).FirstOrDefault()));
            int fovCounted = Util.SafeParseInt(Util.GetValue(laneLines.Where(x => x.StartsWith("FovCounted")).FirstOrDefault()));
            string instrument = Util.GetValue(laneLines.Where(x => x.StartsWith("Sc")).FirstOrDefault());
            string stagePosition = Util.GetValue(laneLines.Where(x => x.StartsWith("St")).FirstOrDefault());
            double bindingDensity = Util.SafeParseDouble(Util.GetValue(laneLines.Where(x => x.StartsWith("B")).FirstOrDefault()));
            string cartridgeID = Util.GetValue(laneLines.Where(x => x.StartsWith("CartridgeI")).FirstOrDefault());
            string cartridgeBarcode = Util.GetValue(laneLines.Where(x => x.StartsWith("CartridgeB")).FirstOrDefault());

            // Get probe counts (and if RLF not yet added, probe info as well); this will add to probecount table, possibly probe table, and
            //      provide data for calculating QC values and flags
            Dictionary<int, int> probeCounts = GetProbeCounts(lines, ThisRlf);

            // Get QC values and flags
            // Imaging QC
            double pctFovCounted = fovCount > 0 ? Math.Round(Convert.ToDouble(fovCounted) / fovCount, 2) : -1;
            bool pctFovPass = pctFovCounted > thresholds.ImagingThreshold;
            // Density QC
            bool bindingDensityPass = isSprint ? bindingDensity <= thresholds.SprintDensityThreshold :
                bindingDensity <= thresholds.SprintDensityThreshold;

            // Probe count-based values
            double posLinearity;
            bool posLinearityPass;
            double lod;
            bool lodPass;
            int pctAboveThresh;
            double geoMeanOfPos;
            double geoMeanOfHKs;

            if (ThisRlf.ThisType != RlfType.Generic && ThisRlf.ThisType != RlfType.DSP
                && ThisRlf.ThisType != RlfType.PlexSet) // Controls processed differently for these assays
            {
                // Linearity QC
                IEnumerable<int> posIDs = ThisRlf.Probes.Values
                    .Where(x => x.CodeClass.Equals("Positive")).OrderBy(x => x.TargetName)
                    .Select(y => y.PrimaryKey);
                posLinearity = GetPosLinearity(posIDs, probeCounts);
                posLinearityPass = posLinearity >= thresholds.LinearityThreshold;
                // LOD QC
                IEnumerable<int> negIDs = ThisRlf.Probes.Values.Where(x => x.CodeClass.Equals("Negative"))
                    .Select(x => x.PrimaryKey);
                lod = GetLod(negIDs, probeCounts, thresholds.LodSdCoefficient);
                lodPass = probeCounts[ThisRlf.ProbeTranslator["POS_E(0.5)"]] > lod;

                // Percent above threshold
                if (ThisRlf.ThisType != RlfType.Generic && ThisRlf.ThisType != RlfType.DSP && ThisRlf.ThisType != RlfType.PlexSet)
                {
                    if (thresholds.CountThreshold > -1)
                    {
                        pctAboveThresh = GetPctAboveThresh(probeCounts, thresholds.CountThreshold);
                    }
                    else
                    {
                        pctAboveThresh = GetPctAboveThresh(probeCounts, Convert.ToInt32(lod));
                    }
                }
                else
                {
                    pctAboveThresh = -1;
                }

                geoMeanOfPos = GetPosGeoMean(posIDs, probeCounts);
                IEnumerable<int> hks = ThisRlf.Probes.Values.Where(x => x.CodeClass.Equals("Housekeeping"))
                                              .OrderBy(x => x.TargetName)
                                              .Select(y => y.PrimaryKey);
                geoMeanOfHKs = GetHkGeoMean(hks, probeCounts);
            }
            else
            {
                posLinearity = -1;
                posLinearityPass = false;
                lod = -1;
                lodPass = false;
                pctAboveThresh = -1;
                geoMeanOfPos = -1;
                geoMeanOfHKs = -1;
            }

           RccAddToDbDirective = $"INSERT INTO RccTable (FileName,IsSprint,SampleName,Owner,Comments,Date,Rlf,IsRlfImported,LaneID,FovCount,FovCounted,Instrument,StagePosition,BindingDensity,CartridgeID,CartridgeBarcode,PctFovCounted,PctFovPass,BindingDensityPass,PosLinearity,PosLinearityPass,LOD,LodPass,PctAboveThresh,GeoMeanOfPos) VALUES({fileName},{isSprint},{sampleName},{owner},{comments},{date},{rlfName},{RlfImported},{laneID},{fovCount},{fovCounted},{instrument},{stagePosition},{bindingDensity},{cartridgeID},{cartridgeBarcode},{pctFovCounted},{pctFovPass},{bindingDensityPass},{posLinearity},{posLinearityPass},{lod},{lodPass},{pctAboveThresh},{geoMeanOfPos})";
        }

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
        /// Gets probe count diciontary from codesummary section; used when RLF was loaded from RLF file
        /// </summary>
        /// <param name="lines">Line from codesummary section</param>
        /// <returns>Dicionary of probe name/count pairs</string></returns>
        private Dictionary<int, int> GetProbeCounts(List<String> lines, Rlf rlf)
        {
            var probeCounts = new Dictionary<int, int>(lines.Count);
            if (rlf.ThisType == RlfType.Gx || rlf.ThisType == RlfType.CNV || rlf.ThisType == RlfType.Generic)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        probeCounts.Add(rlf.ProbeTranslator[bits[1]], Util.SafeParseInt(bits[3]));
                    }
                }
            }
            else if (rlf.ThisType == RlfType.miRNA || rlf.ThisType == RlfType.miRGE)
            {
                int posAVal = GetPosAVal(lines);
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        if ((rlf.ThisType == RlfType.miRNA && bits[0].EndsWith("ous1")) || (rlf.ThisType == RlfType.miRGE && bits[0].EndsWith("ous2")))
                        {
                            string name = bits[1].Split('|')[0];
                            int val = Convert.ToInt32(Util.SafeParseInt(bits[3]) - (ThisRlf.Probes[name].CorrectionCoefficient * posAVal));

                            probeCounts.Add(rlf.ProbeTranslator[name], val);
                        }
                        else
                        {
                            probeCounts.Add(rlf.ProbeTranslator[bits[1]], Util.SafeParseInt(bits[3]));
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
                        probeCounts.Add(rlf.ProbeTranslator[name], Util.SafeParseInt(bits[3]));
                    }
                }
            }

            return probeCounts;
        }

        private int GetPosAVal(List<string> lines)
        {
            string posALine = lines.Where(x => x.StartsWith("Pos") && x.Substring(9, 5).Equals("POS_A")).FirstOrDefault();
            if (posALine == null) { return -1; }
            return Util.SafeParseInt(posALine.Split(',')[3]);
        }

        /// <summary>
        /// Gets Pearson R^2 of log2 ERCC POS control counts vs. their ideal concentrations
        /// </summary>
        /// <param name="posNames">Targets names of the top 5 ERCC POS controls</param>
        /// <param name="counts">ProbeCount collection from RCC</param>
        /// <returns>Double indicating pearson r^2 of correlation of log2 POS control counts vs. log2 of their concentrations</returns>
        private double GetPosLinearity(IEnumerable<int> posNames, Dictionary<int, int> counts)
        {
            IEnumerable<int> vals0 = posNames.Select(x => counts[x]);
            double[] valsx = vals0.Select(x => x > 0 ? Math.Log(x, 2.0) : 0).ToArray();
            double[] valsy = new double[] { 7, 5, 3, 1, -1 };
            double retVal = valsx.Length > 5 ? MathNet.Numerics.Statistics.Correlation.Pearson(valsx.Take(5), valsy) : -1.0;
            return Math.Round(retVal, 2);
        }

        /// <summary>
        /// Calculates % of genes above the user set or calculated background threshold
        /// </summary>
        /// <param name="countThreshold">Threshold for determining background</param>
        private int GetPctAboveThresh(Dictionary<int, int> probeCounts, int countThreshold)
        {
            int aboveThresh = probeCounts.Where(x => x.Value >= countThreshold).Count();
            double fractionAboveThresh = Convert.ToDouble(aboveThresh) / probeCounts.Count;
            return Convert.ToInt32(100 * fractionAboveThresh);
        }

        private double GetPosGeoMean(IEnumerable<int> posNames, Dictionary<int, int> counts)
        {
            IEnumerable<int> posCounts = posNames.Select(x => counts[x]);
            double retVal = Util.GetGeoMean(posCounts.ToArray());
            return Math.Round(retVal, 1);
        }

        /// <summary>
        /// Gets LOD value for LOD QC using NEG probes
        /// </summary>
        /// <param name="negNames">Target names of the ERCC NEG controls</param>
        /// <param name="counts">ProbeCount collection from RCC</param>
        /// <returns>Double indicating background threshold</returns>
        private double GetLod(IEnumerable<int> negNames, Dictionary<int, int> counts, int lodSdCoeff)
        {
            IEnumerable<double> logs = negNames.Select(x => Convert.ToDouble(counts[x]));
            double retVal = logs.Average() + (lodSdCoeff * MathNet.Numerics.Statistics.Statistics.StandardDeviation(logs));
            return Math.Round(retVal, 1);
        }

        /// <summary>
        /// Gets geomean of counts for targets with "Housekeeping" codeclass
        /// </summary>
        /// <param name="hkNames"></param>
        /// <param name="counts"></param>
        public double GetHkGeoMean(IEnumerable<int> hkNames, Dictionary<int, int> counts)
        { 
            if (hkNames.Count() < 1)
            {
                return 0;
            }
            IEnumerable<int> hkCounts = hkNames.Select(x => counts[x]);
            double retVal = Util.GetGeoMean(hkCounts.ToArray());
            return Math.Round(retVal, 1);
        }
    }
}

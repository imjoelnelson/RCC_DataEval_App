using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NCounterCore
{
    public class Rcc : INotifyPropertyChanged
    {
        #region Display property specification collection
        /// <summary>
        /// Dictionary of class display properties and boolean indicating whether value is bool or not
        /// </summary>
        public static Dictionary<string, Tuple<bool, string, int>> Properties = new Dictionary<string, Tuple<bool, string, int>>
        {
            { "FileName", Tuple.Create(false, "File Name", 250) },
            { "SampleName", Tuple.Create(false, "Sample Name", 150) },
            { "LaneID", Tuple.Create(false, "Lane ID", 47) },
            { "CartridgeID", Tuple.Create(false, "Cartridge ID", 150) },
            { "CartridgeBarcode", Tuple.Create(false, "Cartridge Barcode", 100) },
            { "RlfName", Tuple.Create(false, "RLF", 180) },
            { "Date", Tuple.Create(false, "Date", 60) },
            { "Instrument", Tuple.Create(false, "Instrument", 90) },
            { "StagePostion", Tuple.Create(false, "Stage Postion", 100) },
            { "Owner", Tuple.Create(false, "Owner", 100) },
            { "Comments", Tuple.Create(false, "Comments", 150) },
            { "FovCount", Tuple.Create(false, "FovCount", 70) },
            { "FovCounted", Tuple.Create(false, "FovCounted", 70) },
            { "PctFovCounted", Tuple.Create(false, "Imaging QC", 110) },
            { "PctFovPass", Tuple.Create(true, "Imaging Flag", 110) },
            { "BindingDensity", Tuple.Create(false, "Binding Density", 130) },
            { "BindingDensityPass", Tuple.Create(true, "Density Flag", 120) },
            { "PosLinearity", Tuple.Create(false, "POS Linearity", 110) },
            { "PosLinearityPass", Tuple.Create(true, "Linearity Flag", 110) },
            { "Lod", Tuple.Create(false, "LOD", 60) },
            { "LodPass", Tuple.Create(true, "LOD Flag", 90) },
            { "PctAboveThresh", Tuple.Create(false, "% Above Threshold", 145) },
            { "GeoMeanOfPos", Tuple.Create(false, "POS Geomean", 125) },
            { "GeoMeanOfHks", Tuple.Create(false, "Housekeeping Geomean", 175) }
        };
        #endregion

        #region Misc Properties
        public int ID { get; private set; }
        public string RccReadErrorMessage { get; private set; }
        /// <summary>
        /// Indicates to data model class that ThisRlf should be added to RLF list
        /// </summary>
        public bool RlfImported { get; private set; }
        /// <summary>
        /// Filename without .RCC extension; If not altered should have format: yyyyMMdd_[CartridgeID]_[SampleID]_[LaneID]
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            private set
            {
                if (_FileName != value)
                {
                    _FileName = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _FileName;
        /// <summary>
        /// Store of raw file lines for when parts of RCC must be processed after initialization
        /// </summary>
        private List<string> Lines { get; set; }
        /// <summary>
        /// Storage of section indices for when parts of RCC must be processed after initialization
        /// </summary>
        private int[] Indices { get; set; }
        #endregion

        #region Header Attribute Properties

        /// <summary>
        /// Indicates if instrument is a Sprint (Gen3) rather than Digital Analyzer (Gen2 or Gen2.5)
        /// </summary>
        public bool IsSprint { get; private set; }
        #endregion

        #region Sample Attribute Properties
        /// <summary>
        /// User set value; not necessarily unique between samples (alphanumeric, spaces, and hyphens); 60 char limit (if memory serves)
        /// </summary>
        public string SampleName
        {
            get { return _SampleName; }
            private set
            {
                if (_SampleName != value)
                {
                    _SampleName = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _SampleName;

        /// <summary>
        /// User set value (alphanumeric, spaces, and hyphens); 60 char limit (if memory serves)
        /// </summary>
        public string Owner
        {
            get { return _Owner; }
            private set
            {
                if (_Owner != value)
                {
                    _Owner = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _Owner;

        /// <summary>
        /// User set value (alphanumeric, spaces, and hyphens); 256 char limit (if memory serves)
        /// </summary>
        public string Comments
        {
            get { return _Comments; }
            private set
            {
                if (_Comments != value)
                {
                    _Comments = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _Comments;

        /// <summary>
        /// Should be an instrument set value with format yyyyMMdd, though historically this has not always been the case
        /// </summary>
        public string Date
        {
            get { return _Date; }
            private set
            {
                if (_Date != value)
                {
                    _Date = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _Date;

        /// <summary>
        /// Rlf class for this RCC; holds assay type and probe information 
        /// </summary>
        public Rlf ThisRLF { get; set; }
        public string RlfName
        {
            get { return _RlfName; }
            set
            {
                if(_RlfName != value)
                {
                    _RlfName = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _RlfName;
        #endregion

        #region Lane Attribute Properties
        /// <summary>
        /// Lane number (1-12) the RCC is from
        /// </summary>
        public int LaneID
        {
            get { return _LaneID; }
            private set
            {
                if (_LaneID != value)
                {
                    _LaneID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _LaneID;

        /// <summary>
        /// Total fields of view the analyzer was set to record barcodes from
        /// </summary>
        public int FovCount
        {
            get { return _FovCount; }
            private set
            {
                if (_FovCount != value)
                {
                    _FovCount = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _FovCount;

        /// <summary>
        /// Number of fields of view the analyzer actually recorded barcodes from
        /// </summary>
        public int FovCounted
        {
            get { return _FovCounted; }
            private set
            {
                if (_FovCounted != value)
                {
                    _FovCounted = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _FovCounted;

        /// <summary>
        /// Instrument serial number (\d\d\d\d[B,C,F,P]\d\d\d\d; B or C for Gen2, F, for Gen2.5, or P for Sprint/Gen3)
        /// </summary>
        public string Instrument
        {
            get { return _Instrument; }
            private set
            {
                if (_Instrument != value)
                {
                    _Instrument = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _Instrument;

        /// <summary>
        /// Invariably 1 for Sprint or 1-6 for digital analyzers
        /// </summary>
        public string StagePosition
        {
            get { return _StagePosition; }
            private set
            {
                if (_StagePosition != value)
                {
                    _StagePosition = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _StagePosition;

        /// <summary>
        /// Number of barcodes (valid or not) per square micrometer of registered (though not necessarily counted) FOVs
        /// </summary>
        public double BindingDensity
        {
            get { return _BindingDensity; }
            private set
            {
                if (_BindingDensity != value)
                {
                    _BindingDensity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _BindingDensity;

        /// <summary>
        /// User set value (alphanumeric, spaces, and hyphens); (don't remember length; less than 60 chars for sure)
        /// </summary>
        public string CartridgeID
        {
            get { return _CartridgeID; }
            private set
            {
                if (_CartridgeID != value)
                {
                    _CartridgeID = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _CartridgeID;

        /// <summary>
        /// Instrument-recorded cartridge barcode; may be empty if scanner not functioning or barcode obscured; 12 characters
        /// </summary>
        public string CartridgeBarcode
        {
            get { return _CartridgeBarcode; }
            private set
            {
                if (_CartridgeBarcode != value)
                {
                    _CartridgeBarcode = value ?? string.Empty;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _CartridgeBarcode;
        #endregion

        #region Code Summary Properties
        /// <summary>
        /// Dictionary of key=TargetName, Value=ProbeItem for all probes in the codeset
        /// </summary>
        public Dictionary<string, int> ProbeCounts { get; private set; }
        #endregion

        #region QC Properties
        /// <summary>
        /// Equals FovCounted/FovCount
        /// </summary>
        public double PctFovCounted
        {
            get { return _PctFovCounted; }
            private set
            {
                if (_PctFovCounted != value)
                {
                    _PctFovCounted = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _PctFovCounted;

        /// <summary>
        /// Imaging QC flag; true/pass if PctFovCounted > 0.75
        /// </summary>
        public bool PctFovPass
        {
            get { return _PctFovPass; }
            private set
            {
                if (_PctFovPass != value)
                {
                    _PctFovPass = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _PctFovPass;

        /// <summary>
        /// Saturation QC flag; true/pass if BindingDensity < 1.8 for Sprint or < 2.25 for Gen2 or Gen2.5
        /// </summary>
        public bool BindingDensityPass
        {
            get { return _BindingDensityPass; }
            private set
            {
                if (_BindingDensityPass != value)
                {
                    _BindingDensityPass = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _BindingDensityPass;

        /// <summary>
        /// Pearson R^2 for log2 counts of POS probes vs. their ideal concentrations (128, 32, 8, 2, 0.5fM for POS_A through E)
        /// </summary>
        public double PosLinearity
        {
            get { return _PosLinearity; }
            private set
            {
                if (_PosLinearity != value)
                {
                    _PosLinearity = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _PosLinearity;

        /// <summary>
        /// Linearity QC flag; true/pass if PosLinearity >= 0.95
        /// </summary>
        public bool PosLinearityPass
        {
            get { return _PosLinearityPass; }
            private set
            {
                if (_PosLinearityPass != value)
                {
                    _PosLinearityPass = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _PosLinearityPass;

        /// <summary>
        /// Limit of detection mean plus two standard deviations of all NEG control counts
        /// </summary>
        public double Lod
        {
            get { return _Lod; }
            private set
            {
                if (_Lod != value)
                {
                    _Lod = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _Lod;

        /// <summary>
        /// Limit of dection QC flag; true/pass if LOD < POS_E counts
        /// </summary>
        public bool LodPass
        {
            get { return _LodPass; }
            private set
            {
                if (_LodPass != value)
                {
                    _LodPass = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _LodPass;

        /// <summary>
        /// Percent of genes with counts above the default or user-set background threshold
        /// </summary>
        public int PctAboveThresh
        {
            get { return _PctAboveThresh; }
            set
            {
                if(_PctAboveThresh != value)
                {
                    _PctAboveThresh = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _PctAboveThresh;
        #endregion

        #region Normalization Properties
        // For Normalization and Norm flags
        /// <summary>
        /// Value potentially used for POS normalization flagging (i.e. indentifying samples with poor assay performance)
        /// </summary>
        public double GeoMeanOfPos
        {
            get { return _GeoMeanOfPos; }
            private set
            {
                if (_GeoMeanOfPos != value)
                {
                    _GeoMeanOfPos = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _GeoMeanOfPos;
        /// <summary>
        /// Value used for endogenous content-based normalization; based on user specified targets
        /// </summary>
        public double GeoMeanOfHKs
        {
            get { return _GeomeanOfHKs; }
            set
            {
                if (_GeomeanOfHKs != value)
                {
                    _GeomeanOfHKs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _GeomeanOfHKs;
        #endregion

        /// <summary>
        /// CONSTRUCTOR for all RCCs
        /// </summary>
        /// <param name="filePath">Path to the RCC file</param>
        /// <param name="rlfs">List of already loaded RLFs in the Model being loaded into</param>
        public Rcc(string filePath, int id, Dictionary<string, Rlf> rlfs, QcThresholds thresholds)
        {
            // Read in data
            FileName = Path.GetFileNameWithoutExtension(filePath);
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

            // Get Sample Attributes from Sample_Attribute section
            GetSampleAttributes(lines.Skip(indices[2] + 1).Take(indices[3] - (indices[2] + 1)).ToList(), rlfs);

            // Get Lane Attributes from Lane_Attribute section
            GetLaneAttributes(lines.Skip(indices[4] + 1).Take(indices[5] - (indices[4] + 1)).ToList());

            if(ThisRLF.ThisType == RlfType.DSP)
            {
                // Skip setting codesummary section (to be processed later after RLF object created from PkcReaders)
                SetQcValuesAndFlags(thresholds);
                Lines = lines;
                Indices = indices;
                return; // Control for getting RLF and CodeSummary passed back to view/presenters so user can first enter PKCs
            }

            // Get codesummary
            if (ThisRLF.FromRlfFile) // When RLF loaded first
            {
                ProbeCounts = GetProbeCounts(lines.Skip(indices[6] + 1).Take(indices[7] - (indices[6] + 1)).ToList(),
                    ThisRLF.ThisType);
            }
            else // When no relevant RLF loaded
            {
                var result = ProcessCodeSum(lines.Skip(indices[6] + 2).Take(indices[7] - (indices[6] + 1)).ToList(),
                    ThisRLF.ThisType);
                ProbeCounts = result.Item2;
                ThisRLF.AddProbesFromRcc(result.Item1);
            }

            // Check nProbes
            if (ThisRLF.Probes.Count != ProbeCounts.Count)
            {
                throw new Exception("RLF vs. RCC count of probes do not match");
            }

            // *** Get QC flags ***
            SetQcValuesAndFlags(thresholds);

            SetPosGeoMean(ThisRLF.Probes.Values.Where(x => x.CodeClass.Equals("Positive"))
                                               .OrderBy(x => x.TargetName)
                                               .Select(y => y.TargetName)
                                                          , ProbeCounts);
            IEnumerable<string> hks = ThisRLF.Probes.Values.Where(x => x.CodeClass.Equals("Housekeeping"))
                                              .OrderBy(x => x.TargetName)
                                              .Select(y => y.TargetName);
            SetHkGeoMean(hks, ProbeCounts);
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
        /// Gets attributes from RCC SampleAttributes section
        /// </summary>
        /// <param name="lines">lines from the SampleAttributes section</param>
        /// <param name="rlfs">Form1's RLF collection</param>
        private void GetSampleAttributes(List<string> lines, Dictionary<string, Rlf> rlfs)
        {
            SampleName = Util.GetValue(lines.Where(x => x.StartsWith("I")).FirstOrDefault());
            Owner = Util.GetValue(lines.Where(x => x.StartsWith("O")).FirstOrDefault());
            Comments = Util.GetValue(lines.Where(x => x.StartsWith("C")).FirstOrDefault());
            Date = Util.GetValue(lines.Where(x => x.StartsWith("D")).FirstOrDefault());
            string rlfString = Util.GetValue(lines.Where(x => x.StartsWith("G")).FirstOrDefault());
            if (rlfString != string.Empty)
            {
                if (rlfString.StartsWith("DSP_"))
                {
                    RlfName = "Dsp_v1.0";
                    ThisRLF = new Rlf(true);
                    return;
                }
                else
                {
                    bool found = rlfs.TryGetValue(rlfString, out Rlf thisRlf);
                    if (found)
                    {
                        ThisRLF = thisRlf;
                        RlfName = thisRlf.Name;
                        RlfImported = false;
                    }
                    else
                    {
                        thisRlf = new Rlf(rlfString, found); // Add probe collection via public method in Rlf class after codesummary section deliniated below
                        RlfName = thisRlf.Name;
                        RlfImported = true;
                    }
                    ThisRLF = thisRlf;
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
            LaneID = Util.SafeParseInt(Util.GetValue(lines.Where(x => x.StartsWith("I")).FirstOrDefault()));
            FovCount = Util.SafeParseInt(Util.GetValue(lines.Where(x => x.StartsWith("FovCount,")).FirstOrDefault()));
            FovCounted = Util.SafeParseInt(Util.GetValue(lines.Where(x => x.StartsWith("FovCounted")).FirstOrDefault()));
            Instrument = Util.GetValue(lines.Where(x => x.StartsWith("Sc")).FirstOrDefault());
            StagePosition = Util.GetValue(lines.Where(x => x.StartsWith("St")).FirstOrDefault());
            BindingDensity = Util.SafeParseDouble(Util.GetValue(lines.Where(x => x.StartsWith("B")).FirstOrDefault()));
            CartridgeID = Util.GetValue(lines.Where(x => x.StartsWith("CartridgeI")).FirstOrDefault());
            CartridgeBarcode = Util.GetValue(lines.Where(x => x.StartsWith("CartridgeB")).FirstOrDefault());
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
                            int val = Convert.ToInt32(Util.SafeParseInt(bits[3]) - (ThisRLF.Probes[name].CorrectionCoefficient * posAVal));

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
                var test0 = this.FileName;
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        // Add only if probe was present in selected PKC(s)
                        ProbeItem item;
                        bool found = ThisRLF.Probes.TryGetValue(bits[1], out item);
                        if (found)
                        {
                            probeCounts.Add(item.ProbeID, Util.SafeParseInt(bits[3]));
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
            string posALine = lines.Where(x => x.StartsWith("Pos") && x.Substring(9, 5).Equals("POS_A")).FirstOrDefault();
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
                    ProbeItem item = new ProbeItem(bits[0], bits[1], bits[2], ThisRLF.ThisType);
                    if (type != RlfType.PlexSet)
                    {
                        item1.Add(item.TargetName, item);
                        item2.Add(item.TargetName, Util.SafeParseInt(bits[3]));
                    }
                    else
                    {
                        item1.Add($"{item.TargetName}_{item.PlexRow}", item);
                        item2.Add($"{item.TargetName}_{item.PlexRow}", Util.SafeParseInt(bits[3]));
                    }
                }
            }

            return Tuple.Create(item1, item2);
        }

        /// <summary>
        /// Calculates QC values and sets flags based on provided thresholds
        /// </summary>
        /// <param name="thresholds">QC thresholds obtained from settings (editable from dialog from main view)</param>
        public void SetQcValuesAndFlags(QcThresholds thresholds)
        {
            // Imaging QC
            PctFovCounted = FovCount > 0 ? Math.Round(Convert.ToDouble(FovCounted) / FovCount, 2) : -1;
            PctFovPass = PctFovCounted > thresholds.ImagingThreshold;
            // Density QC
            BindingDensityPass = IsSprint ? BindingDensity <= thresholds.SprintDensityThreshold :
                BindingDensity <= thresholds.SprintDensityThreshold;
            if (ThisRLF.ThisType != RlfType.Generic && ThisRLF.ThisType != RlfType.DSP
                && ThisRLF.ThisType != RlfType.PlexSet) // Controls processed differently for these assays
            {
                // Linearity QC
                PosLinearity = GetPosLinearity(ThisRLF.Probes.Values
                    .Where(x => x.CodeClass.Equals("Positive")).OrderBy(x => x.TargetName)
                    .Select(y => y.TargetName), ProbeCounts);
                PosLinearityPass = PosLinearity >= thresholds.LinearityThreshold;
                // LOD QC
                Lod = GetLod(ThisRLF.Probes.Values.Where(x => x.CodeClass.Equals("Negative"))
                    .Select(x => x.TargetName), ProbeCounts, thresholds.LodSdCoefficient);
                LodPass = ProbeCounts["POS_E(0.5)"] > Lod;
            }
            // Percent above threshold
            if (ThisRLF.ThisType != RlfType.Generic && ThisRLF.ThisType != RlfType.DSP && ThisRLF.ThisType != RlfType.PlexSet)
            {
                if (thresholds.CountThreshold > -1)
                {
                    GetPctAboveThresh(thresholds.CountThreshold);
                }
                else
                {
                    GetPctAboveThresh(Convert.ToInt32(Lod));
                }
            }
        }

        /// <summary>
        /// Gets Pearson R^2 of log2 ERCC POS control counts vs. their ideal concentrations
        /// </summary>
        /// <param name="posNames">Targets names of the top 5 ERCC POS controls</param>
        /// <param name="counts">ProbeCount collection from RCC</param>
        /// <returns>Double indicating pearson r^2 of correlation of log2 POS control counts vs. log2 of their concentrations</returns>
        private double GetPosLinearity(IEnumerable<string> posNames, Dictionary<string, int> counts)
        {
            IEnumerable<int> vals0 = posNames.Select(x => counts[x]);
            double[] valsx = vals0.Select(x => x > 0 ? Math.Log(x, 2.0) : 0).ToArray();
            double[] valsy = new double[] { 7, 5, 3, 1, -1 };
            double retVal = valsx.Length > 5 ? MathNet.Numerics.Statistics.Correlation.Pearson(valsx.Take(5), valsy) : -1.0;
            return  Math.Round(retVal, 2);
        }

        /// <summary>
        /// Calculates % of genes above the user set or calculated background threshold
        /// </summary>
        /// <param name="countThreshold">Threshold for determining background</param>
        private void GetPctAboveThresh(int countThreshold)
        {
            int aboveThresh = ProbeCounts.Where(x => x.Value >= countThreshold).Count();
            double fractionAboveThresh = Convert.ToDouble(aboveThresh) / ProbeCounts.Count;
            PctAboveThresh = Convert.ToInt32(100 * fractionAboveThresh);
        }
        
        private void SetPosGeoMean(IEnumerable<string> posNames, Dictionary<string, int> counts)
        {
            IEnumerable<int> posCounts = posNames.Select(x => counts[x]);
            double retVal = Util.GetGeoMean(posCounts.ToArray());
            GeoMeanOfPos = Math.Round(retVal, 1);
        }

        /// <summary>
        /// Gets LOD value for LOD QC using NEG probes
        /// </summary>
        /// <param name="negNames">Target names of the ERCC NEG controls</param>
        /// <param name="counts">ProbeCount collection from RCC</param>
        /// <returns>Double indicating background threshold</returns>
        private double GetLod(IEnumerable<string> negNames, Dictionary<string, int> counts, int lodSdCoeff)
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
        public void SetHkGeoMean(IEnumerable<string> hkNames, Dictionary<string, int> counts)
        {
            if(hkNames.Count() < 1)
            {
                GeoMeanOfHKs = 0;
                return;
            }
            IEnumerable<int> hkCounts = hkNames.Select(x => counts[x]);
            double retVal = Util.GetGeoMean(hkCounts.ToArray());
            GeoMeanOfHKs = Math.Round(retVal, 1);
        }

        public void ApplyRlfandProcessDsp(string rlfName, Dictionary<string, ProbeItem> translator)
        {
            ThisRLF.AddTranslatorAndNameForDsp(rlfName, translator);
            ProbeCounts = GetProbeCounts(Lines.Skip(Indices[6] + 1).Take(Indices[7] - (Indices[6] + 1)).ToList(), ThisRLF.ThisType);
        }

        // NotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

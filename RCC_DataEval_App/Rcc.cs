using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;


namespace RCC_DataEval_App
{
    public class Rcc : INotifyPropertyChanged
    {
        public string RccReadErrorMessage { get; private set; }
        /// <summary>
        /// Filename without .RCC extension; If not altered should have format: yyyyMMdd_[CartridgeID]_[SampleID]_[LaneID]
        /// </summary>
        public string FileName
        {
            get { return _FileName; }
            private set
            {
                if(_FileName != value)
                {
                    _FileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _FileName;
        

        // Header attribues
        /// <summary>
        /// Indicates if instrument is a Sprint (Gen3) rather than Digital Analyzer (Gen2 or Gen2.5)
        /// </summary>
        public bool IsSprint { get; private set; }


        // SampleAttributes
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
                    _SampleName = value;
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
                    _Owner = value;
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
                    _Comments = value;
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
                    _Date = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _Date;

        /// <summary>
        /// Rlf class for this RCC; holds assay type and probe information 
        /// </summary>
        public Rlf ThisRLF
        {
            get { return _ThisRlf; }
            set
            {
                if (_ThisRlf != value)
                {
                    _ThisRlf = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Rlf _ThisRlf;


        // LaneAttributes
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
                    _Instrument = value;
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
                    _StagePosition = value;
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
                    _CartridgeID = value;
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
                if(_CartridgeBarcode != value)
                {
                    _CartridgeBarcode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _CartridgeBarcode;


        // CodeSummary (i.e. probe counts)
        /// <summary>
        /// Dictionary of key=TargetName, Value=ProbeItem for all probes in the codeset
        /// </summary>
        public Dictionary<string, int> ProbeCounts { get; private set; }


        // Messages
        /// <summary>
        /// Instrument set message starting with a ';' Usually empty
        /// </summary>
        public string Message { get; private set; }


        // Flags
        /// <summary>
        /// Equals FovCounted/FovCount
        /// </summary>
        public double PctFovCounted 
        {
            get { return _PctFovCounted; } 
            private set
            {
                if(_PctFovCounted != value)
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
                if(_PctFovPass != value)
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
                if(_BindingDensityPass != value)
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
                if(_PosLinearityPass != value)
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
                if(_Lod != value)
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
                if(_LodPass != value)
                {
                    _LodPass = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _LodPass;


        // For Normalization and Norm flags
        /// <summary>
        /// Value potentially used for POS normalization flagging (i.e. indentifying samples with poor assay performance)
        /// </summary>
        public double GeoMeanOfPos
        {
            get { return _GeoMeanOfPos; }
            private set
            {
                if(_GeoMeanOfPos != value)
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
                if(_GeomeanOfHKs != value)
                {
                    _GeomeanOfHKs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _GeomeanOfHKs;

        /// <summary>
        /// Constructor for all RCCs
        /// </summary>
        /// <param name="filePath">Path to the RCC file</param>
        /// <param name="rlfs">List of already loaded RLFs in Form1</param>
        public Rcc(string filePath, Dictionary<string, Rlf> rlfs)
        {
            // Read in data
            FileName = Path.GetFileNameWithoutExtension(filePath);
            List<string> lines = new List<string>();
            try
            {
                string[] tempLines = File.ReadAllLines(filePath);
                lines.AddRange(tempLines);
            }
            catch(Exception er)
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

            // Get codesummary
            if(ThisRLF.Probes.Any(x => x.Value.ProbeID != null)) // When RLF loaded first
            {
                ProbeCounts = GetProbeCounts(lines.Skip(indices[6] + 1).Take(indices[7] - (indices[6] + 1)).ToList(), ThisRLF.ThisType);
            }
            else // When no relevant RLF loaded
            {
                var result = ProcessCodeSum(lines.Skip(indices[6] + 2).Take(indices[7] - (indices[6] + 1)).ToList());
                ProbeCounts = result.Item2;
                ThisRLF.AddProbesFromRcc(result.Item1);
            }

            // Check nProbes
            if(ThisRLF.Probes.Count != ProbeCounts.Count)
            {
                throw new Exception("RLF vs. RCC count of probes do not match");
            }

            // Get QC flags
            PctFovCounted = FovCount > 0 ? FovCounted / FovCount : -1;
            PctFovPass = PctFovCounted > Form1.ImagingPassThresh;
            BindingDensityPass = IsSprint ? BindingDensity <= Form1.DensityPassThreshS : BindingDensity <= Form1.DensityPassThreshDA;
            if(ThisRLF.ThisType != RlfType.Generic || ThisRLF.ThisType != RlfType.DSP || ThisRLF.ThisType != RlfType.PlexSet) // Controls processed differently for these assays
            {
                PosLinearity = GetPosLinearity(ThisRLF.Probes.Values.Where(x => x.CodeClass.Equals("Positive")).OrderBy(x => x.TargetName)
                .Select(y => y.TargetName), ProbeCounts);
                PosLinearityPass = PosLinearity >= Form1.PosLinearityPassThresh;
                Lod = GetLod(ThisRLF.Probes.Values.Where(x => x.CodeClass.Equals("Negative"))
                    .Select(x => x.TargetName), ProbeCounts);
                LodPass = ProbeCounts["POS_E(0.5)"] > Lod;
            }
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
                        if(lines[i].StartsWith("</H"))
                        {
                            indices[1] = i;
                        }
                        else if(lines[i].StartsWith("</S"))
                        {
                            indices[3] = i;
                        }
                        else if(lines[i].StartsWith("</L"))
                        {
                            indices[5] = i; 
                        }
                        else if(lines[i].StartsWith("</C"))
                        {
                            indices[7] = i;
                        }
                        else if(lines[i].StartsWith("</M"))
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
            if(rlfString != string.Empty)
            {
                if(rlfString.StartsWith("DSP_"))
                {
                    // GetPKCReaders()

                }
                else
                {
                    Rlf thisRlf;
                    bool found = rlfs.TryGetValue(rlfString, out thisRlf);
                    if (found)
                    {
                        ThisRLF = thisRlf;
                    }
                    else
                    {
                        thisRlf = new Rlf(rlfString, found); // Add probe collection via public method in Rlf class after codesummary section deliniated below
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
            var returnVal = new Dictionary<string, int>(lines.Count);
            if (type != RlfType.DSP)
            {
                for (int i = 0; i < lines.Count; i++)
                {
                    string[] bits = lines[i].Split(',');
                    if (bits.Length > 3)
                    {
                        returnVal.Add(bits[1], Util.SafeParseInt(bits[3]));
                    }
                }
            }
            else
            {
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
                            returnVal.Add(item.TargetName, Util.SafeParseInt(bits[3]));
                        }
                    }
                }
            }

            return returnVal;
        }

        private Dictionary<string, int> GetProbeCountsMiRNA(List<string> lines, RlfType type)
        {
            // Get POS_A counts for ligation-background correction
            int posAVal = GetPosAVal(lines);
            var counts = new Dictionary<string, int>(lines.Count);
            for (int i = 0; i < lines.Count; i++)
            {
                string[] bits = lines[i].Split(',');
                if (bits.Length > 3)
                {
                    if(bits[0].EndsWith("ous1"))
                    {
                        string name = bits[1].Split('|')[0];
                        int val = Convert.ToInt32(Util.SafeParseInt(bits[3]) - (ThisRLF.Probes[name].CorrectionCoefficient * posAVal));

                        counts.Add(name, val);
                    }
                    else
                    {
                        counts.Add(bits[1], Util.SafeParseInt(bits[3]));
                    }
                }
            }

            return counts;
        }

        private int GetPosAVal(List<string> lines)
        {
            string posALine = lines.Where(x => x.StartsWith("Pos") && x.Substring(9,5).Equals("POS_A")).FirstOrDefault();
            if(posALine == null) { return -1; }
            return Util.SafeParseInt(posALine.Split(',')[3]);
        }

        /// <summary>
        /// creates probelist for RLF and Probe Count dictionary for RCC from codesummary section; for when RLF not pulled from file
        /// </summary>
        /// <param name="lines">Lines from the CodeSummary section</param>
        /// <returns>Tuple containing Probe dictionary and count dictionary</returns>
        private Tuple<Dictionary<string, ProbeItem>, Dictionary<string, int>> ProcessCodeSum(List<string> lines)
        {
            var item1 = new Dictionary<string, ProbeItem>(lines.Count);
            var item2 = new Dictionary<string, int>(lines.Count);
            for(int i = 0; i < lines.Count; i++)
            {
                string[] bits = lines[i].Split(',');
                if(bits.Length > 3)
                {
                    item1.Add(bits[1], new ProbeItem(bits[0], bits[1], bits[2], ThisRLF.ThisType));
                    item2.Add(bits[1], Util.SafeParseInt(bits[3]));
                }
            }

            return Tuple.Create(item1, item2);
        }

        /// <summary>
        /// Transfers probe collection from PKC to RLF and creates count dictionary for RCC
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="translator"></param>
        /// <returns></returns>
        private Tuple<Dictionary<string, ProbeItem>, Dictionary<string, int>> ProcessCodeSum(List<string> lines, Dictionary<string, ProbeItem> translator)
        {
            var item1 = new Dictionary<string, ProbeItem>(lines.Count);
            var item2 = new Dictionary<string, int>(lines.Count);
            for (int i = 0; i < lines.Count; i++)
            {
                string[] bits = lines[i].Split(',');
                if (bits.Length > 3)
                {
                    ProbeItem probe;
                    bool found = translator.TryGetValue(bits[1], out probe);
                    if(found)
                    {
                        item1.Add(bits[1], probe);
                        item2.Add(bits[1], Util.SafeParseInt(bits[3]));
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
            return valsx.Length > 5 ? MathNet.Numerics.Statistics.Correlation.Pearson(valsx.Take(5), valsy) : -1.0;
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
            return logs.Average() + (Form1.LODSDCoeff * MathNet.Numerics.Statistics.Statistics.StandardDeviation(logs));
        }

        public void GetHkGeoMean(List<string> hkNames, Dictionary<string, int> counts)
        {
            IEnumerable<int> hkCounts = hkNames.Select(x => counts[x]);
            GeoMeanOfHKs = Util.GetGeoMean(hkCounts.ToArray());
        }

        // NotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

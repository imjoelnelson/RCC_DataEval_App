using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCounterCore
{
    public enum RlfType { Gx, miRNA, miRGE, DSP, PlexSet, CNV, Generic }
    public class Rlf
    {
        public string ErrorMessage { get; private set; }
        /// <summary>
        /// Usually follows the format {a-zA-Z0-9}_C\d\d\d\d or {a-zA-Z0-9}_PS\d\d\d\d) unless a special internal (to Nanostring) RLF
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Enum indicating assay type; used to determine some assay-specific values and processing routes
        /// </summary>
        public RlfType ThisType { get; set; }
        /// <summary>
        /// Collection of distinct probe codeclasses in the codeset
        /// </summary>
        public List<string> CodeClasses => Probes != null ? Probes.Select(x => x.Value.CodeClass).Distinct().ToList() : null;
        /// <summary>
        /// Collection of ProbeItems in the codeset, searchable by TargetName
        /// </summary>
        public Dictionary<string, ProbeItem> Probes { get; private set; }
        /// <summary>
        /// Bool indicating if RLF was initiated from rlf file (vs. from RCC)
        /// </summary>
        public bool FromRlfFile { get; private set; }

        public static Dictionary<string, string> PsRowTranslate = new Dictionary<string, string>()
        {
            { "1", "A" }, { "2", "B" }, { "3", "C" }, { "4", "D" }, { "5", "E" }, { "6", "F" }, { "7", "G" }, { "8", "H" }
        };
        public static Dictionary<string, string> PsTranslateRow = new Dictionary<string, string>()
        {
            { "A", "1" }, { "B", "2" }, { "C", "3" }, { "D", "4" }, { "E", "5" }, { "F", "6" }, { "G", "7" }, { "H", "8" }
        };

        /// <summary>
        /// Constructor when generating RLF objects from an RLF file
        /// </summary>
        /// <param name="filePath">Path of RLF file</param>
        public Rlf(string filePath)
        {
            // Get RLF name from file path and RlfType from Name
            Name = Path.GetFileNameWithoutExtension(filePath);
            ThisType = GetRlfType(Name);

            // Get file text
            List<string> lines = new List<string>();
            try
            {
                string[] tempLines = File.ReadAllLines(filePath);
                lines.AddRange(tempLines);
            }
            catch (Exception er)
            {
                MessageBox.Show($"{er.Message}\r\n{er.StackTrace}", "RLF Read Error", MessageBoxButtons.OK);
            }

            // Build codeclass translator from header section
            Dictionary<string, string> codeClassTranslator = GetCodeClassTranslator(lines);

            // Get probe collection
            Probes = GetProbes(lines, codeClassTranslator);

            // Set from RLF file to true
            FromRlfFile = true;
        }

        /// <summary>
        /// Constructor for RLF when RCC loaded first
        /// </summary>
        /// <param name="name">RLF name from file name</param>
        /// <param name="fromRlf">Only used to overload constructor</param>
        public Rlf(string name, bool fromRlf)
        {
            Name = name;
            ThisType = GetRlfType(Name);
            FromRlfFile = false;
        }

        /// <summary>
        /// Constructor for RLF when DSP RLF or RCC loaded
        /// </summary>
        /// <param name="readers"></param>
        public Rlf(bool fromFile)
        {
            Name = "DSP_v1.0";
            ThisType = RlfType.DSP;
            FromRlfFile = fromFile;
        }


        /// <summary>
        /// Gets the assay type to determine how to process the data
        /// </summary>
        /// <returns>RlfType indicating codeset is PlexSet, DSP, CNV, miRNA, miRGE, or Gx</returns>
        private RlfType GetRlfType(string name)
        {
            RlfType[] temp = new RlfType[1];
            Match match1 = Regex.Match(name.ToLower(), @"_ps\d\d\d\d");
            if (match1.Success)
            {
                temp[0] = RlfType.PlexSet;
            }
            else if (name.StartsWith("DSP_"))
            {
                temp[0] = RlfType.DSP;
            }
            else if (name.StartsWith("miR"))
            {
                temp[0] = RlfType.miRNA;
            }
            else if (name.StartsWith("miX"))
            {
                temp[0] = RlfType.miRGE;
            }
            else if (name.StartsWith("CNV"))
            {
                temp[0] = RlfType.CNV;
            }
            else if (name.StartsWith("n6_DV1-pBBs-972c"))
            {
                temp[0] = RlfType.Generic;
            }
            else
            {
                temp[0] = RlfType.Gx;
            }

            return temp[0];
        }

        /// <summary>
        /// Translates the numeric codeclass keys in the RLF to codeclass names
        /// </summary>
        /// <param name="lines">Lines from the RLF header section</param>
        /// <returns>Dictionary translating codeclass key to codeclass name</returns>
        private Dictionary<string, string> GetCodeClassTranslator(List<string> lines)
        {
            int i = 0;
            Dictionary<string, string> dict = new Dictionary<string, string>(20);
            while (i < lines.Count)
            {
                if (lines[i].StartsWith("[con")) // Read up to start of the "Content" section
                {
                    break;
                }
                if (lines[i].StartsWith("ClassN")) // ClassName lines provide definition of a codeclass
                {
                    string[] classActiveBits = lines[i + 1].Split('='); // Line directly following gives classactive value which determines if class is in RCCs
                    if (classActiveBits.Length > 1)
                    {
                        if (classActiveBits[1].Equals("2") || classActiveBits[1].Equals("3")) //filters for class active value of 2 or 3 (i.e. routed to RCC)
                        {
                            string[] codeKeyBits = lines[i].Split('=');
                            if (codeKeyBits.Length > 1)
                            {
                                if (codeKeyBits[0].Length > 9)
                                {
                                    dict.Add(codeKeyBits[0].Substring(9), codeKeyBits[1]);  //Add class key and class name to dictionary
                                }
                            }
                        }
                    }
                }
                i++;
            }

            return dict;
        }

        /// <summary>
        /// Gets ProbeItems from the content rowss of the RLF file
        /// </summary>
        /// <param name="lines">Content rows from the RLF file</param>
        /// <param name="translator">Dictionary translating codeclass key to codeclass name</param>
        /// <returns>Probe collection Dictionary searchable by TargetName</returns>
        private Dictionary<string, ProbeItem> GetProbes(List<string> lines, Dictionary<string, string> translator)
        {
            Dictionary<string, ProbeItem> dict = new Dictionary<string, ProbeItem>(800);
            int i = 0;
            int recordCount = 0;
            while (i < lines.Count)  //Advance to record count line
            {
                if (lines[i].StartsWith("Re"))
                {
                    string[] bits = lines[i].Split('=');
                    if (bits.Length > 1)
                    {
                        Util.SafeParseInt(bits[1]);    // Get record count
                    }
                }
                if (lines[i].StartsWith("Co")) // Advance to start of records
                {
                    i++;
                    break;
                }
                i++;
            }
            while (i < lines.Count)
            {
                if (lines[i].StartsWith("Re"))
                {
                    ProbeItem item = new ProbeItem(lines[i], translator, ThisType);
                    if (ThisType == RlfType.PlexSet && (item.CodeClass.StartsWith("E") || item.CodeClass.StartsWith("H")))
                    {
                        // Concatenate row indicator with target name
                        dict.Add($"{item.TargetName}_{PsTranslateRow[item.PlexRow]}", item);
                    }
                    else
                    {
                        dict.Add(item.TargetName, item);
                    }
                }
            }

            if (recordCount > 0)
            {
                if (recordCount == dict.Count)
                {
                    return dict;
                }
                else
                {
                    ErrorMessage = "Some probes from RLF may not have been parsed and added to the probe list.";
                }
            }

            return dict;
        }

        // Public method for privately setting Probes dictionary from RCC content (i.e. when RCC loaded first)
        public void AddProbesFromRcc(Dictionary<string, ProbeItem> input)
        {
            Probes = input;
        }

        public void AddTranslatorForDsp(Dictionary<string, ProbeItem> translator)
        {
            Probes = translator;
        }
    }
}

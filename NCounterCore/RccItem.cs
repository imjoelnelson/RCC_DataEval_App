using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class RccItem : ISampleData
    {
        public int ID { get; set; }
        public List<KeyValuePair<string, object>> AnnotationInfo { get; set; }
        public List<Tuple<string, double, bool>> QcInfo { get; set; }
        public string ErrorMessage { get; set; }

        public RlfType ThisRlfType { get; set; }

        public RccItem(string filePath, int id)
        {
            AnnotationInfo = new List<KeyValuePair<string, object>>(16);
            QcInfo = new List<Tuple<string, double, bool>>(8);

            // Set initial properties
            ID = id;
            AnnotationInfo.Add(new KeyValuePair<string, object>("FileName", System.IO.Path.GetFileNameWithoutExtension(filePath)));

            // Read file
            List<string> lines = new List<string>();
            try
            {
                string[] tempLines = System.IO.File.ReadAllLines(filePath);
                lines.AddRange(tempLines);
            }
            catch (Exception er)
            {
                ErrorMessage = $"{er.Message}\r\n{er.StackTrace}";
                return;
            }

            // Identify section indices
            int[] indices = GetSectionIndices(lines);

            // Enter System Type
            AnnotationInfo.Add(new KeyValuePair<string, object>("IsSprint", 
                GetSystemType(lines.Skip(indices[0] + 1).Take(indices[1] - (indices[0] + 1)).ToList())));

            // Enter Sample Attributes
            GetSampleAttributes(lines.Skip(indices[2] + 1).Take(indices[3] - (indices[2] + 1)).ToList());

            // Enter Lane Attributes
            GetLaneAttributes(lines.Skip(indices[4] + 1).Take(indices[5] - (indices[4] + 1)).ToList());

            // Get RLF type
            ThisRlfType = GetRlfType(GetStringAnnot("RlfName"));

            // Read count data and RLF info in publics methods, returning to lists in the MainModel, not the Rcc object
        }

        public int? GetIntAnnot(string key)
        {
            var kvp = AnnotationInfo.Where(x => x.Key == key);
            if (kvp.Count() > 0)
            {
                if (kvp.ElementAt(0).Value.GetType() == typeof(int))
                {
                    return (int)kvp.ElementAt(0).Value;
                }
            }
            return null;
        }

        public double? GetDoubleAnnot(string key)
        {
            var kvp = AnnotationInfo.Where(x => x.Key == key);
            if (kvp.Count() > 0)
            {
                if (kvp.ElementAt(0).Value.GetType() == typeof(int))
                {
                    return (double)kvp.ElementAt(0).Value;
                }
            }
            return null;
        }

        public string GetStringAnnot(string key)
        {
            var kvp = AnnotationInfo.Where(x => x.Key == key);

            if (kvp.Count() > 0)
            {
                return (string)kvp.ElementAt(0).Value;
            }
            else
            {
                return null;
            }
        }

        public Tuple<List<ProbeItem>, List<ProbeCountItem>> ProcessRccCodeSum(List<string> codeSumLines) // WHEN SHOULD THIS BE PROCESSED TO AVOID DOUBLE READING FILE LINES
        {
            List<ProbeItem> retVal1 = new List<ProbeItem>(codeSumLines.Count);
            List<ProbeCountItem> retVal2 = new List<ProbeCountItem>(codeSumLines.Count);
            if (this.ThisRlfType == RlfType.miRGE || this.ThisRlfType == RlfType.miRNA)
            {

            }
            else
            {
                for (int i = 0; i < codeSumLines.Count; i++)
                {
                    string[] bits = codeSumLines[i].Split(',');
                    if(this.ThisRlfType == RlfType.PlexSet)
                    {

                    }
                    else
                    {
                        ProbeItem item = new ProbeItem(bits[0], bits[1], bits[2], ThisRlfType);// Add ID to constructor
                        retVal1.Add(item);
                        retVal2.Add(new ProbeCountItem(this.ID, item.ProbeMainKeyId, Util.SafeParseInt(bits[3])));
                    }
                }
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
        private void GetSampleAttributes(List<string> lines)
        {
            AnnotationInfo.Add(new KeyValuePair<string, object>("Name", Util.GetValue(lines.Where(x => x.StartsWith("I")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("Owner", Util.GetValue(lines.Where(x => x.StartsWith("O")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("Comments", Util.GetValue(lines.Where(x => x.StartsWith("C")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("Date", Util.GetValue(lines.Where(x => x.StartsWith("D")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("RlfName", Util.GetValue(lines.Where(x => x.StartsWith("G")).FirstOrDefault())));
        }

        /// <summary>
        /// Gets attributes from RCC LaneAttributes section
        /// </summary>
        /// <param name="lines">Lines from the LaneAttributes section</param>
        private void GetLaneAttributes(List<string> lines)
        {
            // Get attributes
            AnnotationInfo.Add(new KeyValuePair<string, object>("LaneID", Util.SafeParseInt(Util.GetValue(lines.Where(x => x.StartsWith("I")).FirstOrDefault()))));
            AnnotationInfo.Add(new KeyValuePair<string, object>("FovCount", Util.SafeParseInt(Util.GetValue(lines.Where(x => x.StartsWith("FovCount,")).FirstOrDefault()))));
            AnnotationInfo.Add(new KeyValuePair<string, object>("FovCounted", Util.SafeParseInt(Util.GetValue(lines.Where(x => x.StartsWith("FovCounted")).FirstOrDefault()))));
            AnnotationInfo.Add(new KeyValuePair<string, object>("Instrument", Util.GetValue(lines.Where(x => x.StartsWith("Sc")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("StagePosition", Util.GetValue(lines.Where(x => x.StartsWith("St")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("BindingDensity", Util.SafeParseDouble(Util.GetValue(lines.Where(x => x.StartsWith("B")).FirstOrDefault()))));
            AnnotationInfo.Add(new KeyValuePair<string, object>("CartridgeID", Util.GetValue(lines.Where(x => x.StartsWith("CartridgeI")).FirstOrDefault())));
            AnnotationInfo.Add(new KeyValuePair<string, object>("CartridgeBarcode", Util.GetValue(lines.Where(x => x.StartsWith("CartridgeB")).FirstOrDefault())));
        }

        /// <summary>
        /// Gets the assay type to determine how to process the data
        /// </summary>
        /// <param name="name">The RLF file name w/o extension</param>
        /// <returns>RlfType indicating codeset is PlexSet, DSP, CNV, miRNA, miRGE, or Gx</returns>
        private RlfType GetRlfType(string name)
        {
            System.Text.RegularExpressions.Match match1 = System.Text.RegularExpressions.Regex.Match(name.ToLower(), @"_ps\d\d\d\d");
            if (match1.Success)
            {
                return RlfType.PlexSet;
            }
            else if (name.StartsWith("DSP_"))
            {
                return RlfType.DSP;
            }
            else if (name.StartsWith("miR"))
            {
                return RlfType .miRNA;
            }
            else if (name.StartsWith("miX"))
            {
                return RlfType .miRGE;
            }
            else if (name.StartsWith("CNV"))
            {
                return RlfType .CNV;
            }
            else if (name.StartsWith("n6_DV1-pBBs-972c"))
            {
                return RlfType .Generic;
            }
            else
            {
                return RlfType.Gx;
            }
        }
    }
}

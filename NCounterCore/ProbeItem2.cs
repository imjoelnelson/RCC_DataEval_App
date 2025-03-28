using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class ProbeItem2 : IProbeInfo
    {
        // The main key for probe items
        public int MainKeyID { get; set; }
        /// <summary>
        /// A name,value list of probe attributes from either RCCs, RLF, or both
        /// </summary>
        public List<KeyValuePair<string, object>> AnnotationInfo { get; set; }
        /// <summary>
        /// A message for reporting errors in item instantiation
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Constructor for ProbeItems derived from RCC info rather than RLF info
        /// </summary>
        /// <param name="codeClass">The text from the CodeClass field of the probe row</param>
        /// <param name="name">The text from the Name field of the probe row</param>
        /// <param name="accession">The text from the Accession field of the probe row</param>
        /// <param name="type">The RLF type of the associated RLF</param>
        public ProbeItem2(string codeClass, string name, string accession, RlfType type, int RlfId, int mainKeyID)
        {
            MainKeyID = mainKeyID;
            AnnotationInfo = new List<KeyValuePair<string, object>>(9);

            // Add RLF association
            AnnotationInfo.Add(new KeyValuePair<string, object>("RlfId", 
                                                                RlfId));
            // Add RCC info based on RlfType
            if (type == RlfType.Gx || type == RlfType.CNV)
            {
                AnnotationInfo.Add(new KeyValuePair<string, object>("CodeClass", 
                                                                    codeClass));
                AnnotationInfo.Add(new KeyValuePair<string, object>("TargetName", 
                                                                    name));
                AnnotationInfo.Add(new KeyValuePair<string, object>("Accession", 
                                                                    accession));
            }
            else if (type == RlfType.miRNA || type == RlfType.miRGE)
            {
                AnnotationInfo.Add(new KeyValuePair<string, object>("CodeClass", 
                                                                    codeClass));
                AnnotationInfo.Add(new KeyValuePair<string, object>("Accession", 
                                                                    accession));
                string[] bits = name.Split('|');
                AnnotationInfo.Add(new KeyValuePair<string, object>("TargetName", 
                                                                    bits[0]));
                AnnotationInfo.Add(new KeyValuePair<string, object>("CorrectionCoefficient", 
                                                                    Util.SafeParseDouble(bits[1])));
            }
            else if (type == RlfType.PlexSet)
            {
                if (codeClass.StartsWith("P") || codeClass.StartsWith("N")) // For POS and NEG probes with row specified in Name field
                {
                    AnnotationInfo.Add(new KeyValuePair<string, object>("CodeClass", 
                                                                        codeClass));
                    AnnotationInfo.Add(new KeyValuePair<string, object>("Accession", 
                                                                        accession));
                    string[] bits = name.Split('_'); // Concatenated probe name and row specifier (e.g. POS_1)
                    AnnotationInfo.Add(new KeyValuePair<string, object>("TargetName", 
                                                                        bits[0]));
                    AnnotationInfo.Add(new KeyValuePair<string, object>("PlexRow", 
                                                                        Int32.Parse(RlfItem.PsTranslateRow[bits[1]]) - 1)); // Convert row letter to index
                }
                else // For Endogenous and HK probes with row designator concatenated with codeclass
                {
                    AnnotationInfo.Add(new KeyValuePair<string, object>("CodeClass", 
                                                                        codeClass.Substring(0, codeClass.Length - 2)));
                    AnnotationInfo.Add(new KeyValuePair<string, object>("PlexRow", 
                                                                        Int32.Parse(codeClass.Substring(codeClass.Length - 2, 1)) - 1));
                    AnnotationInfo.Add(new KeyValuePair<string, object>("CodeClass", 
                                                                        codeClass));
                    AnnotationInfo.Add(new KeyValuePair<string, object>("TargetName", 
                                                                        name));
                }
            }
            else
            {
                AnnotationInfo.Add(new KeyValuePair<string, object>("TargetName", name));
            }
        }

        /// <summary>
        /// Add RLF field information after loading RLF
        /// </summary>
        /// <param name="probeID">The text from the ProbeID field of the probe row in the RLF</param>
        /// <param name="seq">The text from the Sequence field of the probe row in the RLF</param>
        /// <param name="barcode">The text from the Barcode Field of the probe row in the RLF</param>
        public void AddRlfInfo(string probeID, string seq, string barcode)
        {
            if(probeID == null || seq == null || barcode == null)
            {
                throw new ArgumentException("AddRlfInfo argument exception: Arguments cannot be null");
            }
            AnnotationInfo.Add(new KeyValuePair<string, object>("ProbeID", probeID));
            AnnotationInfo.Add(new KeyValuePair<string, object>("Sequence", seq));
            AnnotationInfo.Add(new KeyValuePair<string, object>("Barcode", barcode));
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
    }
}

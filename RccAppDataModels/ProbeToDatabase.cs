using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class ProbeToDatabase
    {
        public string ProbeAddToDBDirective { get; private set; }
        public bool ReadError { get; set; }
        public string ReadErrorMessage { get; set; }

        /// <summary>
        /// Contructor for creating ProbeItems from RCCs (i.e. when RCCs loaded before or without RLF)
        /// </summary>
        /// <param name="dataLine">Probe line from RCC</param>
        public ProbeToDatabase(string classString, string name, string accessionString, Rlf rlf)
        {
            try
            {
                // Parent RLF info
                int rlfID = rlf.ID;
                // Probe line info
                string codeClass;
                string targetName;
                double correctionCoefficient;
                int plexRow;
                if (rlf.ThisType == RlfType.Gx || rlf.ThisType == RlfType.CNV)
                {
                    codeClass = classString;
                    targetName = name;
                    correctionCoefficient = 0; //Placeholder value
                    plexRow = -1; // Placeholder vlaue
                }
                else if (rlf.ThisType == RlfType.miRNA || rlf.ThisType == RlfType.miRGE)
                {
                    codeClass = classString;
                    string[] bits = name.Split('|');
                    if (bits.Length == 2) // Is an miRNA target
                    {
                        targetName = bits[0];
                        correctionCoefficient = Util.SafeParseDouble(bits[1]);
                        plexRow = -1; //Placeholder
                    }
                    else // Is not an miRNA target
                    {
                        targetName = bits[0];
                        correctionCoefficient = 0;
                        plexRow = -1; //Placeholder
                        return;
                    }
                }
                else if (rlf.ThisType == RlfType.PlexSet)
                {
                    if (classString.StartsWith("P") || classString.StartsWith("N")) // For POS and NEG probes with row specified in Name field
                    {
                        codeClass = classString;
                        string[] bits = name.Split('_'); // Concatenated probe name and row specifier (e.g. POS_1)
                        targetName = bits[0];
                        plexRow = Int32.Parse(Rlf.PsTranslateRow[bits[1]]) - 1; // Convert row letter to index
                        correctionCoefficient = -1.0; //Placeholder value
                    }
                    else // For Endogenous and HK probes with row designator concatenated with codeclass
                    {
                        codeClass = classString.Substring(0, classString.Length - 2);
                        plexRow = Int32.Parse(classString.Substring(classString.Length - 2, 1)) - 1;
                        targetName = name;
                        correctionCoefficient = -1.0; //Placeholder value
                    }
                }
                else
                {
                    targetName = name;
                    codeClass = string.Empty;
                    correctionCoefficient = -1.0;
                    plexRow = -1;
                }

                // Un-set, RLF file-specific fields
                string probeID = string.Empty;
                string barcode = string.Empty;
                string sequence = string.Empty;

                ProbeAddToDBDirective = $"INSERT INTO ProbeTable (RlfID,ThisType,ProbeID,TargetName,Barcode,Sequence,CodeClass,Accession,CorrectionCoefficient,PlexRow) VALUES({rlf.ID},{(int)rlf.ThisType},{probeID},{targetName},{barcode},{sequence},{codeClass},{accessionString},{correctionCoefficient},{plexRow})";
                ReadError = false;
            }
            catch(Exception er)
            {
                ReadError = true;
                ReadErrorMessage = $"The probe, {name}, could not be imported due to an exception:\r\n{er.Message}";
            }
        }

        public ProbeToDatabase(string dataLine, Dictionary<string, string> codeClassTranslator, Rlf rlf)
        {
            try
            {
                // Parent RLF info
                int rlfID = rlf.ID;
                // Probe line info
                if (string.IsNullOrEmpty(dataLine))
                {
                    return;
                }

                RlfType thisType = rlf.ThisType;
                string[] bits = dataLine.Split(',');
                if (bits.Length < 7)
                {
                    ReadError = true;
                    ReadErrorMessage = $"Probe line, {dataLine}, from RLF, {rlf.Name},could not be parsed;";
                }
                string[] classBits = bits[0].Split('=');
                if (classBits.Length < 2)
                {
                    ReadError = true;
                    ReadErrorMessage = $"A probe could not be parsed from line: \"{dataLine}\"";
                    return;
                }
                string probeID = bits[4];
                string barcode = bits[2];
                string sequence = bits[1];
                string accession = bits[6];
                // Codeset type-specific output
                string targetName;
                string codeClass;
                double correctionCoefficient;
                int plexRow;
                string tempClass;
                bool parsed = codeClassTranslator.TryGetValue(classBits[1], out tempClass);
                ReadErrorMessage = parsed ? null : $"Codeclass could not be parsed from \"{dataLine}\"";
                if (rlf.ThisType == RlfType.Gx || rlf.ThisType == RlfType.CNV)
                {
                    targetName = bits[3];
                    codeClass = parsed ? tempClass : "Unparsed";
                    correctionCoefficient = 0; // Placeholder value
                    plexRow = -1; // Placeholder value
                }
                else if (rlf.ThisType == RlfType.miRNA || rlf.ThisType == RlfType.miRGE)
                {
                    string[] nameBits = bits[3].Split('|');
                    if (nameBits.Length > 1) // For miRNA targets
                    {
                        targetName = nameBits[0];
                        correctionCoefficient = Util.SafeParseDouble(nameBits[1]);
                        codeClass = parsed ? tempClass : "Unparsed";
                        plexRow = -1; // Placeholder value
                    }
                    else // For non-miRNA targets
                    {
                        targetName = nameBits[0];
                        correctionCoefficient = 0;
                        codeClass = parsed ? tempClass : "Unparsed";
                        plexRow = -1; // Placeholder value
                    }
                }
                else if (rlf.ThisType == RlfType.PlexSet)
                {
                    string classString = parsed ? tempClass : "Unparsed";
                    if (classString.Equals("Unparsed"))
                    {
                        // Placeholders
                        targetName = null;
                        correctionCoefficient = 0;
                        codeClass = null;
                        plexRow = -1;
                        return;
                    }
                    else
                    {
                        if (classString.StartsWith("P") || classString.StartsWith("N")) // For POS and NEG probes with row specified in Name field
                        {
                            codeClass = classString;
                            string[] nameBits = bits[3].Split('_'); // Concatenated probe name and row specifier (e.g. POS_1)
                            targetName = nameBits[0];
                            plexRow = Int32.Parse(nameBits[1]) - 1; // Convert POS or NEG control number to zero-based int
                            correctionCoefficient = -1.0; //Placeholder value
                        }
                        else // For all other targets
                        {
                            codeClass = classString.Substring(0, classString.Length - 2); // Remover plex row number from codeclass
                            plexRow = Int32.Parse(classString.Substring(classString.Length - 2, 1)) - 1; // Extract plex row number from codeclass string
                            targetName = bits[3];
                            correctionCoefficient = -1.0; //Placeholder value
                        }
                    }
                }
                else
                {
                    targetName = bits[3];
                    codeClass = string.Empty; // Placeholder value
                    correctionCoefficient = 0; // Placeholder value
                    plexRow = -1; // Placeholder value
                }

                ProbeAddToDBDirective = $"INSERT INTO ProbeTable (RlfID,ThisType,ProbeID,TargetName,Barcode,Sequence,CodeClass,Accession,CorrectionCoefficient,PlexRow) VALUES({rlf.ID},{(int)rlf.ThisType},{probeID},{targetName},{barcode},{sequence},{codeClass},{accession},{correctionCoefficient},{plexRow})";
                ReadError = false;
            }
            catch(Exception er)
            {
                ReadError = true;
                ReadErrorMessage = $"The probe information could not be imported from RLF dataline, \"{dataLine}\", due to an exception:\r\n\r\n{er.Message}";
            }
        }
    }
}

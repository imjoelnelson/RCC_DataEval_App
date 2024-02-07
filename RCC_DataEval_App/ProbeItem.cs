using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public class ProbeItem
    {
        public string ParsingErrorMessage { get; private set; }
        public RlfType ThisType { get; private set; }

        // ********  Properties from RLF  *********
        /// <summary>
        /// Primary key for probes if RLF loaded; otherwise name is primary key
        /// </summary>
        public string ProbeID { get; private set; }
        public string Barcode { get; private set; }
        public string Sequence { get; private set; }

        // ********  Properties from RLF or RCC  *********
        /// <summary>
        /// Primary key if RLF NOT loaded; otherwise ProbeID is primary key
        /// </summary>
        public string TargetName { get; private set; }
        /// <summary>
        /// Indicator or analyte or how probe will be used; dependent on context of codeset (i.e. associated RLF)
        /// </summary>
        public string CodeClass { get; private set; }
        /// <summary>
        /// Acession number from relevant database
        /// </summary>
        public string Accession { get; private set; }

        // ********  Properties from RCC  *********
        /// <summary>
        /// miRNA-specific; Pulled from data line after '|'; multiplied by ERCC POS_A count gives ligation background correction factor
        /// </summary>
        public double CorrectionCoefficient { get; private set; }

        // ********  GeoMx or PlexSet-specific  *********
        /// <summary>
        /// Indicator of the well on 96 well plate where hybridization took place; in format [A-G][1-12]
        /// </summary>
        public string PlexRow { get; set; }


        /// <summary>
        /// Contructor for when RCC loaded first; NOT DSP/GeoMx
        /// </summary>
        /// <param name="dataLine">Probe line from RCC</param>
        public ProbeItem(string codeClass, string name, string accession, RlfType type)
        {
            ThisType = type;
            CodeClass = codeClass;
            
            if (type == RlfType.miRNA || type == RlfType.miRGE)
            {
                Accession = accession;
                string[] bits = name.Split('|');
                if(bits.Length == 2)
                {
                    TargetName = bits[0];
                    CorrectionCoefficient = Util.SafeParseDouble(bits[1]);
                }
            }
            else if(type == RlfType.DSP)
            {
                TargetName = name;
                PlexRow = accession; // Shoehorned in to use same constructor
                Accession = string.Empty;
            }
            else
            {
                TargetName = name;
                Accession = accession;
            }
        }


        /// <summary>
        /// Constructor for when RLF loaded first
        /// </summary>
        /// <param name="dataLine">Probe line from RLF</param>
        /// <param name="codeClassTranslator">Dictionary for translating from classID to codeclass</param>
        public ProbeItem(string dataLine, Dictionary<string, string> codeClassTranslator, RlfType type)
        {
            if(string.IsNullOrEmpty(dataLine))
            {
                return;
            }
            ThisType = type;
            string[] bits = dataLine.Split(',');
            string[] classBits = bits[0].Split('=');
            if(classBits.Length < 2)
            {
                ParsingErrorMessage = $"A probe could not be parsed from line: \"{dataLine}\"";
                return;
            }
            string tempClass;
            bool parsed = codeClassTranslator.TryGetValue(classBits[1], out tempClass);
            ParsingErrorMessage = $"Codeclass could not be parsed from \"{dataLine}\"";
            CodeClass = parsed ? tempClass : "Unparsed";
            ProbeID = bits[4];
            TargetName = bits[3];
            Barcode = bits[2];
            Sequence = bits[1];
            Accession = bits[6];
        }

        /// <summary>
        /// Set RLF-specific properties if RLF loaded after item created by RCC
        /// </summary>
        /// <param name="dataBits"></param>
        public void SetRlfProperties(string[] dataBits)
        {
            ProbeID = dataBits[4];
            Barcode = dataBits[2];
            Sequence = dataBits[1];
        }
    }
}

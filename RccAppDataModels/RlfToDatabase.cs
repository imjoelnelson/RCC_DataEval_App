using NCounterCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class RlfToDatabase
    {
        public string RlfAddToDBDirective { get; private set; }
        public bool ReadError { get; private set; }
        public string ReadErrorMessage { get; private set; }

        /// <summary>
        /// Creating an RLF entry in RLF table from RLF file
        /// </summary>
        /// <param name="filePath">The path to the RLF file</param>
        public RlfToDatabase(string filePath)
        {
            // Get RLF name from file path and RlfType from Name
            string name = Path.GetFileNameWithoutExtension(filePath);
            int thisType = GetRlfType(name);

            // Get file text
            List<string> lines = new List<string>();
            try
            {
                string[] tempLines = File.ReadAllLines(filePath);
                lines.AddRange(tempLines);
            }
            catch (Exception er)
            {
                ReadError = true;
                ReadErrorMessage = $"RLF, {name} could not be loaded due to an exception:\r\n\r\n{er.Message}";
            }
        }

        /// <summary>
        /// Creating an RLF entry in RLF table from an RCC (when RCCs loaded prior to any RLF loaded for the codeset)
        /// </summary>
        /// <param name="name">The RLF name</param>
        /// <param name="isFromRlf">Bool indicating if is from RLF file; should always be false and essentially is just for creating an overload</param>
        public RlfToDatabase(string name, bool isFromRlf)
        {
            try
            {
                int thisType = GetRlfType(name);
                string codeClasses = string.Empty;
                int rlfFileLoaded = isFromRlf ? 1 : 0;

                RlfAddToDBDirective = $"INSERT INTO RlfTable (Name,ThisType,CodeClasses,RlfFileLoaded) VALUES({name},{thisType},{codeClasses},{rlfFileLoaded})";
                ReadError = false;
            }
            catch(Exception er)
            {
                ReadError = true;
                ReadErrorMessage = $"RLF, {name} could not be loaded due to an exception:\r\n\r\n{er.Message}";
            }
        }


        /// <summary>
        /// Gets the assay type to determine how to process the data
        /// </summary>
        /// <returns>RlfType indicating codeset is PlexSet, DSP, CNV, miRNA, miRGE, or Gx</returns>
        private int GetRlfType(string name)
        {
            int retVal;
            Match match1 = Regex.Match(name.ToLower(), @"_ps\d\d\d\d");
            if (match1.Success)
            {
                var type = RlfType.PlexSet;
                retVal = (int)type;
            }
            else if (name.StartsWith("DSP_"))
            {
                var type = RlfType.DSP;
                retVal = (int)type;
            }
            else if (name.StartsWith("miR"))
            {
                var type = RlfType.miRNA;
                retVal = (int)type;
            }
            else if (name.StartsWith("miX"))
            {
                var type = RlfType.miRGE;
                retVal = (int)type;
            }
            else if (name.StartsWith("CNV"))
            {
                var type = RlfType.CNV;
                retVal = (int)type;
            }
            else if (name.StartsWith("n6_DV1-pBBs-972c"))
            {
                var type = RlfType.Generic;
                retVal = (int)type;
            }
            else
            {
                var type = RlfType.Gx;
                retVal = (int)type;
            }

            return retVal;
        }
    }
}

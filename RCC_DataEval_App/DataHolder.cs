
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public class DataHolder
    {
        public BindingList<Rcc> Rccs { get; set; }
        public Dictionary<string, Rlf> Rlfs { get; set; }
        public Dictionary<string, PkcReader> Pkcs { get; set; }

        public DataHolder() { }

        public void CreateObjectsFromFiles(List<string> fileNames, int fileTypeIndex)
        {
            List<string> filesToLoad = new List<string>();
            List<string> zipsToLoad = new List<string>();

            string[] fileTypes = new string[] { ".RCC", ".RLF", ".PKC" };

            filesToLoad.AddRange(fileNames.Where(x => x.EndsWith(fileTypes[fileTypeIndex], StringComparison.OrdinalIgnoreCase)));
            zipsToLoad.AddRange(fileNames.Where(x => x.EndsWith(".ZIP", StringComparison.OrdinalIgnoreCase)));

            if(zipsToLoad.Count > 0)
            {
                foreach (string s in zipsToLoad)
                {
                    IEnumerable<string> newFiles = RecursiveUnzip.RecursivelyGetFilesFromZip(s, fileTypes[fileTypeIndex]);
                    filesToLoad.AddRange(newFiles);
                }
            }

            if(fileTypeIndex == 0)
            {
                IEnumerable<string> rccsToAdd = filesToLoad.Where(x => x.EndsWith("RCC", StringComparison.OrdinalIgnoreCase));
            }
            else if(fileTypeIndex == 1)
            {
                // Make RLFs from files
            }
            else
            {
                // Make PKCs from files
            }
        }

        
    }
}
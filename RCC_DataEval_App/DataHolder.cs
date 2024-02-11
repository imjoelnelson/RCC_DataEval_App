using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                filesToLoad.AddRange(zipsToLoad.Select(x => filesToLoad.AddRange(RecursivelyGetFilesFromZip(x, fileTypes[fileTypeIndex]))));
            }

            if(fileTypeIndex == 0)
            {
                IEnumerable<string> rccsToAdd = filesToLoad.Select(x => new Rcc(x, Rlfs));
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

        public IEnumerable<string> RecursivelyGetFilesFromZip(string zipPath, string searchPattern)
        {

        }
    }
}

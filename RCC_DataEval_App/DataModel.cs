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
    public class DataModel : IRawDataModel
    {
        public BindingList<Rcc> Rccs { get; set; }
        public Dictionary<string, Rlf> Rlfs { get; set; }
        public Dictionary<string, PkcReader> Pkcs { get; set; }
        public List<RccTreeNode> Tree { get; set; }

        public DataModel()
        {
            Rccs = new BindingList<Rcc>();
            Rlfs = new Dictionary<string, Rlf>();
            Pkcs = new Dictionary<string, PkcReader>();
        }

        public void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex)
        {
            List<string> filesToLoad = new List<string>();
            List<string> zipsToLoad = new List<string>();

            string[] fileTypes = new string[] { ".RCC", ".RLF", ".PKC" };

            filesToLoad.AddRange(fileNames.Where(x => x.EndsWith(fileTypes[fileTypeIndex], StringComparison.OrdinalIgnoreCase)));
            zipsToLoad.AddRange(fileNames.Where(x => x.EndsWith(".ZIP", StringComparison.OrdinalIgnoreCase)));

            if (zipsToLoad.Count > 0)
            {
                foreach (string s in zipsToLoad)
                {
                    IEnumerable<string> newFiles = RecursiveUnzip.RecursivelyGetFilesFromZip(s, fileTypes[fileTypeIndex]);
                    filesToLoad.AddRange(newFiles);
                }
            }

            if (fileTypeIndex == 0)
            {
                for (int i = 0; i < filesToLoad.Count; i++)
                {
                    Rcc temp = new Rcc(filesToLoad[i], Rlfs);
                    Rccs.Add(temp);
                    if (temp.RlfImported)
                    {
                        Rlfs.Add(temp.ThisRLF.Name, temp.ThisRLF);
                    }
                }
            }
            else if (fileTypeIndex == 1)
            {
                for (int i = 0; i < filesToLoad.Count; i++)
                {
                    Rlf temp = new Rlf(filesToLoad[i]);
                    // Validate temp and then:
                    Rlfs.Add(temp.Name, temp);
                }
            }
            else
            {
                // Add PkcReader
            }
        }
    }
}

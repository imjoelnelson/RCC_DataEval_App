
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
                filesToLoad.AddRange(zipsToLoad.Select(x => filesToLoad.AddRange(RecursivelyGetFilesFromZip(x, fileTypes[fileTypeIndex]))));
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

        public List<string> RecursivelyGetFilesFromZip(string zipPath, string searchPattern, List<string> passwords)
        {
            // List to return
            List<string> returnList = new List<string>();
            // Top level zip to extract
            ZipFile topZip = new ZipFile(zipPath);
            // Queue to contain zips for unzipping
            Queue<ZipFile> archiveQueue = new Queue<ZipFile>();
            archiveQueue.Enqueue(topZip);

            // Loop through zips from queue, extracting one at a time. Add files with seach pattern to return list 
            //  and enqueueing any zip files found for later extraction. Any directories are recuresively opened and 
            //  any files matching searchPattern are added to return list. This continues till no more zips in queue
            while(archiveQueue.Count > 0)
            {
                // Dequeue to get a zip to extract
                ZipFile current = archiveQueue.Dequeue();
                // Get temp folder to extract to
                string extractPath = $"{Path.GetTempFileName()}";
                try
                {
                    // Exract to temp path then enumerate files(zips and matching extension) 
                    current.ExtractAll(extractPath, ExtractExistingFileAction.OverwriteSilently);
                    IEnumerable<string> files = Directory.EnumerateFiles(extractPath, "*", SearchOption.TopDirectoryOnly);
                    // Add files with matching extension to return list
                    returnList.AddRange(files.Where(x => x.EndsWith(searchPattern, StringComparison.OrdinalIgnoreCase)));
                    // Enqueue zips for later extraction
                    files.Where(x => x.EndsWith("ZIP", StringComparison.OrdinalIgnoreCase))
                        .ToList().ForEach(x => archiveQueue.Enqueue(new ZipFile(x)));
                    // Get any directories
                    IEnumerable<string> dirs = Directory.EnumerateDirectories(extractPath)
                        .Where(x => !x.Equals("_MACOSX"));
                }
                catch(Exception er)
                {

                }
            }
        }

        public Tuple<List<string>, List<string>> RecursivelyEnumerateByExtention(string path, string extension)
        {
            List<string> out1 = new List<string>();
            List<string> zips = new List<string>();
            Queue<string> dirsToSearch = new Queue<string>();

            dirsToSearch.Enqueue(path);

            while(dirsToSearch.Count > 0)
            {
                string current = dirsToSearch.Dequeue();
                var files = Directory.EnumerateFiles(current, "*", SearchOption.TopDirectoryOnly);
                out1.AddRange(files.Where(x => x.EndsWith(extension, StringComparison.OrdinalIgnoreCase)));
                zips.AddRange(files.Where(x => x.EndsWith("ZIP", StringComparison.OrdinalIgnoreCase)));
                Directory.EnumerateDirectories(current, "*", SearchOption.TopDirectoryOnly).
                    Where(x => !x.Equals("_MACOSX")).ToList()
                    .ForEach(x => dirsToSearch.Enqueue(x));
            }

            return Tuple.Create(out1, zips);
        }
    }
}
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RccAppDataModels
{
    class RecursiveUnzip
    {
        public event EventHandler RequestPassword;
        /// <summary>
        /// Recursively extract a zip (and any zips contained), enumerating all files with matching extension
        /// </summary>
        /// <param name="zipPath">Path to the zip to be extracted</param>
        /// <param name="searchPattern">Extension to search for</param>
        /// <returns>All files with matching extension</returns>
        public static List<string> RecursivelyGetFilesFromZip(string zipPath, string searchPattern)
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
            while (archiveQueue.Count > 0)
            {
                // Dequeue to get a zip to extract
                ZipFile current = archiveQueue.Dequeue();
                // Get temp folder to extract to
                string extractPath = $"{Path.GetTempFileName()}";
                // Extract and get files
                Tuple<List<string>, List<string>> extractedFiles = TryExtract(current, extractPath, searchPattern, null);
                if (extractedFiles != null)
                {
                    // Add files with selected extension to returnlist
                    returnList.AddRange(extractedFiles.Item1);
                    // Enqueue any zips found for later exraction
                    foreach (string s in extractedFiles.Item2)
                    {
                        archiveQueue.Enqueue(new ZipFile(s));
                    }
                }
                else
                {
                    // If extraction fails, assume due to missing password; have user enter password and re-try extraction
                    int i = 0;
                    while (i < 3) // Repeat three times then exit if all attempts failed
                    {
                        string newPassword = GetPassword(Path.GetFileName(current.Name));
                        if (newPassword == null) { break; }
                        Tuple<List<string>, List<string>> extractedFilesAgain = TryExtract(current, extractPath, searchPattern, newPassword);
                        if (extractedFilesAgain != null)
                        {
                            returnList.AddRange(extractedFilesAgain.Item1);
                            foreach (string s in extractedFilesAgain.Item2)
                            {
                                archiveQueue.Enqueue(new ZipFile(s));
                            }
                            break;
                        }
                        // If the above is null, assume wrong password again
                        var result = MessageBox.Show("The password is incorrect. Do you want to re-enter it?",
                                                      string.Empty,
                                                      MessageBoxButtons.YesNo);
                        if (result == DialogResult.No)
                        {
                            break;
                        }
                        i++;
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Extract a zipfile and return a list of all files with given extension and a second list with any zips found
        /// </summary>
        /// <param name="current">ZipFile to be extracted</param>
        /// <param name="extractPath">Temp path to extract to</param>
        /// <param name="searchPattern">Extension of files to be returned</param>
        /// <param name="password">Password for any password-locked zips</param>
        /// <returns></returns>
        private static Tuple<List<string>, List<string>> TryExtract(ZipFile current, string extractPath, string searchPattern, string password)
        {
            try
            {
                List<string> out1 = new List<string>();
                List<string> zips = new List<string>();

                // Exract to temp path then enumerate files(zips and matching extension) 
                current.ExtractAll(extractPath, ExtractExistingFileAction.OverwriteSilently);
                IEnumerable<string> files = Directory.EnumerateFiles(extractPath, "*", SearchOption.TopDirectoryOnly);
                // Add files with matching extension to return list
                out1.AddRange(files.Where(x => x.EndsWith(searchPattern, StringComparison.OrdinalIgnoreCase)));
                // Add zips for later extraction
                zips.AddRange(files.Where(x => x.EndsWith("ZIP", StringComparison.OrdinalIgnoreCase)));
                // Get any directories
                IEnumerable<string> dirs = Directory.EnumerateDirectories(extractPath)
                    .Where(x => !x.Equals("_MACOSX"));
                foreach (string d in dirs)
                {
                    Tuple<List<string>, List<string>> temp = GetFilesByExtention(d, searchPattern);
                    out1.AddRange(temp.Item1);
                    zips.AddRange(temp.Item2);
                }

                return Tuple.Create(out1, zips);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Recursively enumerate files from a directory  while skipping any subdirs named _MACOSX
        /// </summary>
        /// <param name="path">path to directorty to enumerate files from</param>
        /// <param name="extension">search pattern/extension for files to collect</param>
        /// <returns>Tuple containing a collection of files with named extension and a collection of any zips</returns>
        public static Tuple<List<string>, List<string>> GetFilesByExtention(string path, string extension)
        {
            List<string> out1 = new List<string>();
            List<string> zips = new List<string>();
            Queue<string> dirsToSearch = new Queue<string>();

            dirsToSearch.Enqueue(path);

            while (dirsToSearch.Count > 0)
            {
                string current = dirsToSearch.Dequeue();
                var files = Directory.EnumerateFiles(current, "*", SearchOption.TopDirectoryOnly);
                out1.AddRange(files.Where(x => x.EndsWith(extension, StringComparison.OrdinalIgnoreCase)));
                zips.AddRange(files.Where(x => x.EndsWith("ZIP", StringComparison.OrdinalIgnoreCase)));
                Directory.EnumerateDirectories(current, "*", SearchOption.TopDirectoryOnly).
                    Where(x => !x.Equals("_MACOSX")).ToList().ForEach(x => dirsToSearch.Enqueue(x));
            }

            return Tuple.Create(out1, zips);
        }
    }
}

using RccExtensions;
using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;

namespace RccAppDataModels
{
    public class RccDataModel : IDataModel
    {
        public BindingList<Rcc> Rccs { get; set; }
        public System.Windows.Forms.BindingSource  RccSource{ get; set; }
        public BindingList<Rcc> SelectedRccs { get; set; }
        public Dictionary<string, Rlf> Rlfs { get; set; }
        public Dictionary<string, string> Pkcs { get; set; } // <-- Keep this collection here rather than in Pkc MVP to handle mainform PKCs adds (holds the info in case PKC dir cannot be located)

        private static string AppDataFolderName = "RccEvalAppData"; // <-- MODIFY NAME OF APPLICATION FOLDER IN ROAMING HERE
        private int IdCounter { get; set; }
        private string RlfRootPath { get; set; }
        private string PkcRootPath { get; set; }

        public event EventHandler RccListChanged;
        public event EventHandler AppFolderCreationFailed;
        public event EventHandler DspRccsLoaded;

        public RccDataModel()
        {
            Rccs = new BindingList<Rcc>();
            RccSource = new System.Windows.Forms.BindingSource();
            RccSource.DataSource = Rccs;
            SelectedRccs = new BindingList<Rcc>();
            IdCounter = 0;

            // Check if application folder and subfolders present in AppData\Roaming
            var appFolderPath = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}\\{AppDataFolderName}";
            CheckSetAppDataFolder(appFolderPath);
            // Get dictionaries of RLFs and PKCs previously saved
            Rlfs = new Dictionary<string, Rlf>();
            Pkcs = GetSavedPkcs(PkcRootPath);
        }

        private void CheckSetAppDataFolder(string appFolderPath)
        {
            if (!Directory.Exists(appFolderPath))
            {
                try
                {
                    Directory.CreateDirectory(appFolderPath);
                }
                catch(Exception er)
                {
                    System.Windows.Forms.MessageBox.Show($"The application's main folder could not be created due to the following error and the app must close:\r\n\r\n{er.Message}",
                                                          "App Folder Creation Error",
                                                          System.Windows.Forms.MessageBoxButtons.OK);
                    AppFolderCreationFailed.Invoke(this, EventArgs.Empty);
                }
            }
            // Check on subfolders
            RlfRootPath = $"{appFolderPath}\\RCC";
            if (!Directory.Exists(RlfRootPath))
            {
                try
                {
                    Directory.CreateDirectory(RlfRootPath);
                }
                catch { }
            }
            PkcRootPath = $"{appFolderPath}\\PKC";
            if(!Directory.Exists(PkcRootPath))
            {
                try
                {
                    Directory.CreateDirectory(PkcRootPath);
                }
                catch { }
            }
        }

        private Dictionary<string, string> GetSavedPkcs(string pkcPath)
        {
            IEnumerable<string> pkcFilePaths = Directory.EnumerateFiles(pkcPath, "*.pkc", SearchOption.TopDirectoryOnly);
            if(pkcFilePaths.Count() > 0)
            {
                return pkcFilePaths.ToDictionary(x => Path.GetFileNameWithoutExtension(x), x => x);
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }
        
        public void CreateObjectsFromFiles(string[] fileNames, int fileTypeIndex, QcThresholds thresholds)
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
                    List<string> newFiles = new RecursiveUnzip(s, fileTypes[fileTypeIndex]).ExtractedFiles;
                    filesToLoad.AddRange(newFiles);
                }
            }
            // Add any RCC files to Rcc binding list
            if (fileTypeIndex == 0)
            {
                List<Rcc> tempList = new List<Rcc>(filesToLoad.Count);
                // Use WaitCursor here because lots of files can cause some delay
                GuiCursor.WaitCursor(() => {
                    for (int i = 0; i < filesToLoad.Count; i++)
                    {
                        Rcc temp = new Rcc(filesToLoad[i], IdCounter, Rlfs, thresholds);
                        tempList.Add(temp);
                        IdCounter++;
                        // Add ThisRlf to Rlf collection if not present in collection
                        if (temp.RlfImported)
                        {
                            Rlfs.Add(temp.ThisRLF.Name, temp.ThisRLF); // This step is the only reason for not putting this in a lamda
                        }
                    }
                });

                // Order by RLF, then date, then cartridge ID
                List<Rcc> tempList2 = tempList.OrderBy(x => x.ThisRLF.Name)
                                              .ThenBy(x => x.Date)
                                              .ThenBy(x => x.CartridgeID)
                                              .ThenBy(x => x.LaneID).ToList();
                // Add ordered Rccs to BindingList
                for(int i = 0; i < tempList2.Count; i++)
                {
                    Rccs.Add(tempList2[i]);
                }
                ListChanged(); // Send update signal out to View through Presenter
                if(Rccs.Any(x => x.ThisRLF.ThisType == RlfType.DSP))
                {
                    DspRccsLoaded.Invoke(this, EventArgs.Empty);
                }
            }
            // Add any RLFs to Rlf collection
            else if (fileTypeIndex == 1)
            {
                for (int i = 0; i < filesToLoad.Count; i++)
                {
                    // Copy file to RLF folder in app folder
                    string savePath = $"{RlfRootPath}\\{Path.GetFileName(filesToLoad[i])}";
                    try
                    {
                        File.Copy(filesToLoad[i], savePath);
                    }
                    catch { }
                    // Add to RLF dictionary
                    Rlf temp = new Rlf(filesToLoad[i]);
                    // Validate temp and then:
                    Rlfs.Add(temp.Name, temp);
                }
            }
            // Add any PKCs to Pkc collection
            else
            {
                for(int i = 0; i < filesToLoad.Count; i++)
                {
                    AddPkc(filesToLoad[i]);
                }
            }
        }

        /// <summary>
        /// Method run when RCCs added or removed from Rccs BindingList
        /// </summary>
        public void ListChanged()
        {
            RccSource.DataSource = Rccs;
            RccSource.ResetBindings(false);

            RccListChanged.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Clears the RCC list (in response to clear RCCs button or MenuStripItem click)
        /// </summary>
        public void ClearRccs()
        {
            Rccs.Clear();
            ListChanged();
        }

        /// <summary>
        /// Updates QC values and flags in all RCCs based on updated threshold values
        /// </summary>
        /// <param name="thresholds">QCThresholds object with update threshold values</param>
        public void UpdateThresholds(QcThresholds thresholds)
        {
            for(int i = 0; i < Rccs.Count; i++)
            {
                Rccs[i].SetQcValuesAndFlags(thresholds);
            }
            RccSource.DataSource = Rccs;
            RccSource.ResetBindings(false);
        }

        /// <summary>
        /// Sorts the RCC list by the selected columns, ascending or descending based on the sortList
        /// </summary>
        /// <param name="sortList">Provides columns to be sorted on as well as indication of whether ascending or descending</param>
        public void SortTable(Dictionary<string, bool> sortList)
        {
            // Convert sort list to ISort collection to be used in sorting method
            Dictionary<string, ISort> columnsToSortOn = new Dictionary<string, ISort>(4);
            int i = 0;
            foreach(KeyValuePair<string, bool> p in sortList)
            { 
                columnsToSortOn.Add(p.Key, new SortableColumn(i, p.Value));
                i++;
            }
            // Sort the Rcc collection
            Rccs = new BindingList<Rcc>(Rccs.AsQueryable<Rcc>().OrderByColumns<Rcc>(columnsToSortOn).ToList());
            RccSource.DataSource = Rccs;
            RccSource.ResetBindings(false);
        }

        public void AddPkc(string thisPkcPath)
        {
            // Attempt to save PKC
            string savePath = $"{PkcRootPath}\\{Path.GetFileName(thisPkcPath)}";
            bool saved;
            try
            {
                File.Copy(thisPkcPath, savePath);
                saved = true;
            }
            catch { saved = false; }

            // Update Pkc list
            string name = Path.GetFileNameWithoutExtension(thisPkcPath);
            if (saved)
            {
                Pkcs[name] = savePath;
            }
            else
            {
                Pkcs[name] = thisPkcPath; // Should be set by ref in PKC model but ... just in case
            }
        }

        public void RemovePkc(string pkcPath)
        {
            try
            {
                File.Delete(pkcPath);
            }
            catch { }
            var name = Path.GetFileNameWithoutExtension(pkcPath);
            if(Pkcs.ContainsKey(name)) // Again, just in case
            {
                Pkcs.Remove(name);
            }
        }

        public void ApplyRlfToDspRccs(List<Rcc> rccs, string cartridgeID, string rlfName, Dictionary<string, ProbeItem> translator)
        {
            IEnumerable<Rcc> cartRccs = rccs.Where(x => x.CartridgeID == cartridgeID);
            foreach(Rcc rcc in cartRccs)
            {
                rcc.ApplyRlfandProcessDsp(rlfName, translator);
            }
        }

        public string[][] BuildRawDataTable(List<int> ids, string[] selectedProperties)
        {
            if(ids == null)
            {
                throw new Exception("RCC list cannot be null.");
            }
            if(ids.Count < 1)
            {
                throw new Exception("RCC list must have 1 or more RCCs");
            }
            List<Rcc> rccs = Rccs.Where(x => ids.Contains(x.ID)).ToList();
            IEnumerable<Rlf> rlfs = rccs.Select(x => x.ThisRLF);
            int rlfCount = rlfs.Select(x => x.Name).Distinct().Count();

            if (rlfs.Any(x => x.ThisType == RlfType.PlexSet || x.ThisType == RlfType.DSP))
            {
                if((rlfs.Any(x => x.ThisType == RlfType.DSP) && rlfCount > 1)
                    || (rlfs.Any(x => x.ThisType == RlfType.PlexSet) && rlfCount > 1))
                {
                    // Triggers error message via presenter as this should be the only condition where this method returns null
                    return null;
                }
                else
                {
                    // Build multiplex raw data table or give option for table vs. plate view
                    var table = new RawProbeCountsTable(rccs, rccs[0].ThisRLF, selectedProperties, rccs[0].ThisRLF.ThisType == RlfType.DSP);
                    if (table != null)
                    {
                        return table.TableLines;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                // Single RLF table
                if (rlfCount < 2)
                {
                    
                    // Easiest given Rlf count == 1; no need to change
                    Rlf rlf = rccs[0].ThisRLF;
                    // Table using all probes
                    var table = new RawProbeCountsTable(rccs, rlf, selectedProperties);
                    if (table != null)
                    {
                        return table.TableLines;
                    }
                    else
                    {
                        return null;
                    }
                }
                // Multi-RLF table
                else
                {
                    if(!rlfs.All(x => x.FromRlfFile))
                    {
                        // load all RLFs from files with dialog
                        // Check if all RLFs now loaded; if not return null and message
                        return null;
                    }

                    // Get targets present in all RLFs
                    List<List<string>> listOfLists = rlfs.Select(x => x.Probes.Select(y => y.Value.ProbeID).ToList()).ToList();
                    var intersection = listOfLists.Skip(1).Aggregate(new HashSet<string>(listOfLists.First()),
                        (h, e) => { h.IntersectWith(e); return h; });
                    // <<<REPLACE LATER WITH LIST FROM PREFERENCES>>>
                    var table = new RawProbeCountsTable(rccs, rccs.Select(x => x.ThisRLF).Distinct().ToList(), selectedProperties);
                    if (table != null)
                    {
                        return table.TableLines;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public string[][] TransformTable(string[][] lines)
        {
            if(lines.Any(x => x.Length != lines[0].Length))
            {
                throw new Exception("TransformTable cannot operate on jagged array");
            }
            List<List<string>> transformed = new List<List<string>>(lines[0].Length);
            for(int r = 0; r < lines[0].Length; r++)
            {
                List<string> temp = new List<string>(lines.Length);
                for(int c = 0; c < lines.Length; c++)
                {
                    temp.Add(lines[c][r]);
                }
                transformed.Add(temp);
            }
            return transformed.Select(x => x.ToArray()).ToArray();
        }

        public List<RlfType> GetRlfTypes(List<int> ids)
        {
            if (ids == null) return null;
            return Rccs.Where(x => ids.Contains(x.ID))
                       .Select(x => x.ThisRLF.ThisType)
                       .Distinct()
                       .ToList();
        }
    }
}

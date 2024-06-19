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

        /// <summary>
        /// Checks for presence and correct folder structure of the App directory in AppData (local)
        /// </summary>
        /// <param name="appFolderPath">The path where the app folder should be saved</param>
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

        /// <summary>
        /// Gets paths of the previously save PKCs in the PKC directory in AppData to put into the PKC list in this model
        /// </summary>
        /// <param name="pkcPath">Path to the PKC directory</param>
        /// <returns>A list of paths as strings</returns>
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
        
        /// <summary>
        /// Created RCC, Rlf, and PKC objects based on the files selected
        /// </summary>
        /// <param name="fileNames">Files selected in the main view</param>
        /// <param name="fileTypeIndex">File type filter index from the OpenFileDialog in the main view</param>
        /// <param name="thresholds">For any RCC files selected, provides threshold to use for '% above threshold' QC metric calculation</param>
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

        /// <summary>
        /// Adds a PKC file to the directory of saved PKCs in AppData, as well as to the Pkcs list in this main model
        /// </summary>
        /// <param name="thisPkcPath"></param>
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

        /// <summary>
        /// Removes a PKC file from the Pkc directory in AppData (and from the Pkc list in this model if not already removed)
        /// </summary>
        /// <param name="pkcPath">Path of the PKC to be removed</param>
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

        /// <summary>
        /// Applies RLF (actually just RLF name and probe translator) to DSP RCCs, based on PKCs selected for the RCCs
        /// </summary>
        /// <param name="rccs">RCCs selected in the main form</param>
        /// <param name="cartridgeID">Cartride ID of the RCCs that are to have the RLF applied</param>
        /// <param name="rlfName">The name of the RLF to be applied (concatenation of all PKCs selected for the RCCs)</param>
        /// <param name="translator">The probe name translator for converting DSP_IDs to probe name, codeclass, etc.</param>
        public void ApplyRlfToDspRccs(List<Rcc> rccs, string cartridgeID, string rlfName, Dictionary<string, ProbeItem> translator)
        {
            IEnumerable<Rcc> cartRccs = rccs.Where(x => x.CartridgeID == cartridgeID);
            foreach(Rcc rcc in cartRccs)
            {
                rcc.ApplyRlfandProcessDsp(rlfName, translator);
            }
        }

        /// <summary>
        /// Returns a list of tuples of DSP cartIDs with any PKCs that are associated for the PKC select view when MainView "Edit Pkc Association" menu item is clicked 
        /// </summary>
        /// <param name="ids">Selected row indices from the MainView DGV corresponding to selected RCCs</param>
        /// <returns>A list of tuples containing cartridge ID and any associated PKCs</returns>
        public List<Tuple<string, string[]>> GetDspCartIDs(List<int> ids)
        {
            // Get selected DSP RCCs
            var dspRcss = Rccs.Where(x => ids.Contains(x.ID) && x.ThisRLF.ThisType == RlfType.DSP);
            // Get unique cartridge IDs of the selected RCCs
            var cartIDs = dspRcss.Count() > 0 ? dspRcss.Select(x => x.CartridgeID).Distinct().ToList() : null;
            if(cartIDs == null)
            {
                return null;
            }
            // Get associated PKCs for each cartridge ID
            List<Tuple<string, string[]>> retVal = new List<Tuple<string, string[]>>();
            for(int i = 0; i < cartIDs.Count; i++)
            {
                // Get RLF name from an RCC from the cartridge with cartID[i], then split by '_' to get PKC names
                var cartRccs = dspRcss.Where(x => x.CartridgeID.Equals(cartIDs[i]));
                if(cartRccs.All(x => !x.ThisRLF.Name.Equals("DSP_v1.0")))
                {
                    // Split RLF name from one of the cartridge RCCs using '$' to get PKC names
                    //   (RLF name for RCCs with associated PKCs is concatenation of PKC names in this app)
                    string[] pkcs = cartRccs.ElementAt(0).ThisRLF.Name.Split('$');
                    retVal.Add(new Tuple<string, string[]>(cartIDs[i], pkcs));
                }
                else
                {
                    retVal.Add(null);
                }
            }
            return retVal;
        }

        /// <summary>
        /// Builds a string[][] representing raw counts for the probes in the selected RCCs; This output is essentially an array of columns to be converted to array of rows with another function
        /// </summary>
        /// <param name="ids">int IDs of the selected RCCs</param>
        /// <param name="selectedProperties">RCC metadata property rows to display in the header portion of the table</param>
        /// <returns>Raw count table with metadata header section, QC flag section, and probe count section, formatted as an array of columns</returns>
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

            // WARNING: UGLY BRANCHING HELL AHEAD; SHOULD REVISIT AND SEE IF REFACTOR COULD HELP OR AT LEAST RELOCATE TO RAWPROBECOUNTTABLE CLASS
            
            if (rlfs.Any(x => x.ThisType == RlfType.PlexSet || x.ThisType == RlfType.DSP)) // Sample multiplexed assays
            {
                // Incompatible Multiplex RCCs selected
                if((rlfs.Any(x => x.ThisType == RlfType.DSP) && rlfCount > 1)
                    || (rlfs.Any(x => x.ThisType == RlfType.PlexSet) && rlfCount > 1))
                {
                    // DSP and PlexSet selected
                    if(rlfs.Any(x => x.ThisType == RlfType.DSP && rlfs.Any(z => z.ThisType == RlfType.PlexSet)))
                    {
                        System.Windows.Forms.MessageBox.Show("PlexSet and DSP RCCs cannot be displayed in the same raw counts table together. Select RCCs with one or the other assay type and try again.",
                                                             "Incompatible Assay Types",
                                                             System.Windows.Forms.MessageBoxButtons.OK);
                        return null;
                    }
                    // DSP-only selected but some have no PKC or different collections of PKCs
                    else if(rlfs.Any(x => x.ThisType == RlfType.DSP))
                    {
                        // Some RCCs have no PKCs selected
                        if(rlfs.Any(x => x.Name.Equals("DSP_v1.0")))
                        {
                            System.Windows.Forms.MessageBox.Show("Some RCCs had no PKCs associated and were excluded from the table.",
                                                                 "Undefined Probes Detected",
                                                                 System.Windows.Forms.MessageBoxButtons.OK);
                            rccs.RemoveAll(x => x.ThisRLF.Name.Equals("DSP_v1.0"));
                            // Build multiplex raw data table
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
                        // RCCs with different collections of PKCs
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("DSP RCCs were selected that were associated with different collections of PKCs. These cannot be shown in a table together. Select RCCs only using the same collection of PKCs and try again.",
                                                                 "Multiple Probe Collections Detected",
                                                                 System.Windows.Forms.MessageBoxButtons.OK);
                            return null;
                        }
                    }
                    // PlexSet RCCs with different RLFs selected
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("PlexSet RCCs with different RLFs were selected together. These cannot be shown in the same table. Select RCCs from only one RLF and try again.",
                                                             "Multiple RLFs Detected",
                                                             System.Windows.Forms.MessageBoxButtons.OK);
                        return null;
                    }
                }
                else
                {
                    // No incompatibility => Build multiplex raw data table
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
            // Non-sample-multiplexed assays only
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

        /// <summary>
        /// Converts array of string[] which represent columns to array of string[] which represent rows
        /// </summary>
        /// <param name="lines">Array of string[] which represent columns of the table</param>
        /// <returns>Array of string[] which represent rows of the table</returns>
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

        /// <summary>
        /// Gets a collection of the RlfTypes (assay types) represented in the selected RCCs; for branching purposes in building the table
        /// </summary>
        /// <param name="ids">int IDs of the selected RCCs</param>
        /// <returns>Collection of RlfTypes represented in the selected RCCs</returns>
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

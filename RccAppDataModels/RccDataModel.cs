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
        public Dictionary<string, string> Pkcs { get; set; }

        private static string AppDataFolderName = "RccEvalAppData"; // <-- MODIFY NAME OF APPLICATION FOLDER IN ROAMING HERE
        private string RlfPath { get; set; }
        private string PkcPath { get; set; }

        public event EventHandler RccListChanged;
        public event EventHandler AppFolderCreationFailed;
        public event EventHandler DspRccsLoaded;

        public RccDataModel()
        {
            Rccs = new BindingList<Rcc>();
            RccSource = new System.Windows.Forms.BindingSource();
            RccSource.DataSource = Rccs;
            SelectedRccs = new BindingList<Rcc>();
            Rlfs = new Dictionary<string, Rlf>();
            Pkcs = new Dictionary<string, string>();

            // Check if application folder present in AppData\Roaming
            var appFolderPath = $"{System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData)}\\{AppDataFolderName}";
            CheckSetAppDataFolder(appFolderPath);
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
            var RlfPath = $"{appFolderPath}\\RCC";
            if (!Directory.Exists(RlfPath))
            {
                try
                {
                    Directory.CreateDirectory(RlfPath);
                }
                catch { }
            }
            var PkcPath = $"{appFolderPath}\\PKC";
            if(!Directory.Exists(PkcPath))
            {
                try
                {
                    Directory.CreateDirectory(PkcPath);
                }
                catch { }
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
                        Rcc temp = new Rcc(filesToLoad[i], Rlfs, thresholds);
                        tempList.Add(temp);
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
                if(Rccs.Any(x => x.IsDsp))
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
                    string savePath = $"{RlfPath}\\{Path.GetFileName(filesToLoad[i])}";
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
                    // Copy file to PKC folder in app folder
                    string savePath = $"{PkcPath}\\{Path.GetFileName(filesToLoad[i])}";
                    try
                    {
                        File.Copy(filesToLoad[i], savePath);
                    }
                    catch { }
                    // Add to PKC list
                    Pkcs.Add(Path.GetFileNameWithoutExtension(savePath), savePath);
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
    }
}

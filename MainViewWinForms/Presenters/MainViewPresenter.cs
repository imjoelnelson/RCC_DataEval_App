using MessageCenter;
using NCounterCore;
using RccAppDataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinyMessenger;

namespace MainViewWinForms
{
    public class MainViewPresenter
    {
        private IMainView MainView { get; set; }
        private IDataModel MainModel { get; set; }
        private List<string> DirectoriesToDelete { get; set; }

        public MainViewPresenter(IMainView mainView, IDataModel mainModel)
        {
            // Initializing
            DirectoriesToDelete = new List<string>();
            // get model and view refs
            MainView = mainView;
            MainModel = mainModel;
            // View events
            MainView.FilesLoading += new EventHandler(View_FilesLoading);
            MainView.FormLoaded += new EventHandler(View_FormLoaded);
            MainView.RccListCleared += new EventHandler(View_RccListCleared);
            MainView.ThresholdsUpdated += new EventHandler(View_ThresholdsUpdated);
            MainView.SelectingColumns += new EventHandler(View_SelectingColumns);
            MainView.ColumnsSelected += new EventHandler(View_ColumnsSelected);
            MainView.SortClick += new EventHandler(View_SortingColumns);
            MainView.ThisFormClosed += new EventHandler(View_FormClosing);
            MainView.BuildRawCountsTable += new EventHandler<Views.RccSelectEventArgs>(View_BuildRawCountsTable);
            MainView.OpenRawCountTablePreferences += new EventHandler(View_OpenRawCountTablePreferences);
            MainView.DgvSelectionChanged += new EventHandler<Views.RccSelectEventArgs>(View_RccSelectionChanged);
            MainView.BuildPlateViewTable += new EventHandler<Views.RccSelectEventArgs>(View_BuildPlateViewTable);
            MainView.OpenSampleVSampleScatterDialog += new EventHandler<Views.RccSelectEventArgs>(View_OpenSampleVSampleScatterDialog);
            MainView.AssociatePkcsMenuItemClicked += new EventHandler<Views.RccSelectEventArgs>(View_AssociatedPkcsMenuItemClicked);
            MainView.CountBinsMenuItemClicked += new EventHandler<Views.RccSelectEventArgs>(View_CountBinsMenuItemClicked);
            MainView.PcaOverviewMenuItemClicked += new EventHandler<Views.RccSelectEventArgs>(View_PcaOverviewMenuItemClicked);
            MainView.EvaluateHousekeepersMenuItemClicked += new EventHandler<Views.RccSelectEventArgs>(View_EvaluateHousekeepersMenuItemClicked);
            // Model events
            MainModel.RccListChanged += new EventHandler(Model_RccListChanged);
            MainModel.AppFolderCreationFailed += new EventHandler(Model_AppFolderFailed);
            MainModel.DspRccsLoaded += new EventHandler(Model_DspRccsLoaded);
            // Subscribe to messages
            PresenterHub.MessageHub.Subscribe<PasswordRequestMessage>((m) => HandlePasswordRequest(m.Content));
            PresenterHub.MessageHub.Subscribe<DirectoryToDeleteMessage>((m) => HandleNewDirectoryToDelete(m.Content));
            PresenterHub.MessageHub.Subscribe<PkcAddMessage>((m) => HandlePkcAdd(m.Content));
            PresenterHub.MessageHub.Subscribe<PkcRemoveMessage>((m) => HandlePkcRemove(m.Content));
            PresenterHub.MessageHub.Subscribe<TranslatorSendMessage>((m) => HandleTranslatorSend(m.Content));
        }

        private void HandlePasswordRequest(string fileName)
        {
            _ = MVPFactory.PasswordEnterView(fileName);
        }

        private void HandleNewDirectoryToDelete(string dirToDelete)
        {
            DirectoriesToDelete.Add(dirToDelete);
        }

        private void HandlePkcAdd(string pkcPath)
        {
            string name = Path.GetFileNameWithoutExtension(pkcPath);
            MainModel.AddPkc(pkcPath);
        }

        private void HandlePkcRemove(string pkcPath)
        {
            MainModel.RemovePkc(pkcPath);
        }

        /// <summary>
        /// Complete RCC processing (add RLF and finish codesum) after PKCs have been selected
        /// </summary>
        /// <param name="translator">Tuple containing the cartridge ID in question and the cognate DSP_ID-to-target translator</param>
        private void HandleTranslatorSend(Tuple<string, string, Dictionary<string, ProbeItem>> translator)
        {
            MainModel.ApplyRlfToDspRccs(MainModel.Rccs.ToList(), translator.Item1, translator.Item2, translator.Item3);
        }

        /// <summary>
        /// Event called from View when any of the three file import menu item dialogs are confirmed
        /// </summary>
        /// <param name="sender">View window</param>
        /// <param name="e">Event args containing OpenFileDialog filter index value</param>
        private void View_FilesLoading(object sender, EventArgs e)
        {
            var args = (FilesLoadingEventArgs)e;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {

                ofd.Filter = "RCC;ZIP|*.RCC;*.ZIP|RLF;ZIP|*.RLF;*.ZIP|PKC;ZIP|*.PKC;*.ZIP";
                ofd.FilterIndex = args.Index;
                string[] fileTypes = new string[] { "RCCs", "RLFs", "PKCs" };
                ofd.Title = $"Select {fileTypes[args.Index]} and/or ZIPs to load";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    MainModel.CreateObjectsFromFiles(ofd.FileNames, args.Index, MainView.CollectThresholds());
                }
                else
                {
                    return;
                }
            }
        }

        private void View_FormLoaded(object sender, EventArgs e)
        {
            Dictionary<string, bool> proptypes = new Dictionary<string, bool>(MainView.SelectedProperties.Count);
            Dictionary<string, string> propNames = new Dictionary<string, string>(MainView.SelectedProperties.Count);
            for (int i = 0; i < MainView.SelectedProperties.Count; i++)
            {
                proptypes.Add(MainView.SelectedProperties[i], Rcc.Properties[MainView.SelectedProperties[i]].Item1);
                propNames.Add(MainView.SelectedProperties[i], Rcc.Properties[MainView.SelectedProperties[i]].Item2);
            }
            MainView.SetDgv(Rcc.Properties, MainView.SelectedProperties, MainModel.RccSource);
        }

        private void Model_RccListChanged(object sender, EventArgs e)
        {
            MainView.DgvSourceChanged(MainModel.Rccs.Count);
        }

        private void View_RccListCleared(object sender, EventArgs e)
        {
            MainModel.ClearRccs();
        }

        private void View_ThresholdsUpdated(object sender, EventArgs e)
        {
            MainModel.UpdateThresholds(MainView.CollectThresholds());
        }

        private void View_SelectingColumns(object sender, EventArgs e)
        {
            // Call this from Presenter to avoid referencing Rcc.Properties directly from View
            MainView.ShowSelectColumnsDialog(Rcc.Properties.Select(x => Tuple.Create(x.Key, x.Value.Item2)).ToList(), MainView.SelectedProperties);
        }

        private void View_ColumnsSelected(object sender, EventArgs e)
        {
            MainView.SetDgv(Rcc.Properties, MainView.SelectedProperties, MainModel.RccSource);
            MainModel.ListChanged();
        }

        private void View_SortingColumns(object sender, EventArgs e)
        {
            foreach(KeyValuePair<string, bool> t in MainView.SortList)
            {
                Console.WriteLine($"{t.Key},{t.Value.ToString()}");
            }
            Console.Write("\r\n\r\n");
            MainModel.SortTable(MainView.SortList);
            MainView.DgvSourceChanged(MainModel.Rccs.Count);
        }

        private void View_FormClosing(object sender, EventArgs e)
        {
            // Delete temp directories and all files therein
            if(DirectoriesToDelete.Count > 0)
            {
                List<string> filesToDelete = new List<string>();

                for(int i = 0; i < DirectoriesToDelete.Count; i++)
                {
                    IEnumerable<string> files = Directory.EnumerateFiles(DirectoriesToDelete[i]);
                    foreach(string s in files)
                    {
                        try
                        {
                            File.Delete(s);
                        }
                        catch { }
                    }
                    int filesAfter = Directory.EnumerateDirectories(DirectoriesToDelete[i]).Count();
                    if(filesAfter < 1)
                    {
                        Directory.Delete(DirectoriesToDelete[i]);
                    }
                }
            }
        }

        private void Model_AppFolderFailed(object sender, EventArgs e)
        {
            MainView.FormClose();
        }

        private void Model_DspRccsLoaded(object sender, EventArgs e)
        {
            IEnumerable<Rcc> dspRccs = MainModel.Rccs.Where(x => x.ThisRLF.ThisType == RlfType.DSP);
            List<string> cartIds = dspRccs.Count() > 0 ? dspRccs.Select(x => x.CartridgeID).Distinct().ToList()
                                                       : new List<string>();
            if(cartIds.Count < 1)
            {
                return;
            }
            List<Tuple<string, string[]>> cartPkcs = new List<Tuple<string, string[]>>(cartIds.Count);
            for(int i = 0; i < cartIds.Count; i++)
            {
                cartPkcs.Add(new Tuple<string, string[]>(cartIds[i], null));
            }
            // LOOP TO CHECK IF RLF != "DSP_v1.0" in which case split RLF name to provide list of already associated PKCs
            Views.IPkcSelectView view = cartIds.Count > 0 ? MVPFactory.PkcView(cartPkcs, MainModel.Pkcs) : null;
            if(view != null)
            {
                view.ShowForm();
            }
        }

        private void View_BuildRawCountsTable(object sender, Views.RccSelectEventArgs e)
        {
            if(MainView.SelectedRlfTypes.Contains(RlfType.DSP) || MainView.SelectedRlfTypes.Contains(RlfType.PlexSet))
            {
                if(MainView.SelectedRlfTypes.Count > 1)
                {
                    string message = "Selected RCCs include sample-multiplexed (DSP and/or PlexSet) and non-multiplexed assays, which are incompatible with each other. Select only sample-multiplexed or non-multiplexed assays and try again.";
                    string caption = "Warning";
                    MainView.ShowErrorMessage(message, caption);
                    return;
                }
            }

            string[] rccProperties = Properties.Settings.Default.SelectedProperties.Split(',');
            string[][] lines = MainModel.BuildRawDataTable(e.IDs, rccProperties);
            if(lines != null)
            {
                string[][] transformed = Util.TransformTable(lines);
                MainView.SaveTable(transformed);
            }
        }

        private void View_OpenRawCountTablePreferences(object sender, EventArgs e)
        {
            Views.IRccPropertyConfigView view = MVPFactory.RccPropertyView();
            if(view != null)
            {
                view.ShowForm();
            }
        }

        private void View_RccSelectionChanged(object sender, Views.RccSelectEventArgs e)
        {
            List<RlfType> rlfTypes = MainModel.GetRlfTypes(e.IDs);
            MainView.UpdateTypesPresent(rlfTypes);
        }

        private void View_BuildPlateViewTable(object sender, Views.RccSelectEventArgs e)
        {
            // Check if selected 
            if ((MainView.SelectedRlfTypes.Contains(RlfType.DSP) || MainView.SelectedRlfTypes.Contains(RlfType.PlexSet)) 
                && MainView.SelectedRlfTypes.Count > 1)
            {
                if(MainView.SelectedRlfTypes.Contains(RlfType.DSP) && MainView.SelectedRlfTypes.Contains(RlfType.PlexSet))
                {
                    string message = "The selected RCCs are from both DSP and PlexSet RLFs which cannot be displayed in the same figure. Select RCCs from one or the other RLF and then try again.";
                    string caption = "Incompatible RLFs";
                    MainView.ShowErrorMessage(message, caption);
                    return;
                }
                if(MainView.SelectedRlfTypes.Any(x => x != RlfType.DSP && x != RlfType.PlexSet))
                {
                    string message = "Only Sample multiplexed RCCs (DSP readout and PlexSet) can be displayed in PlateView. Selected RCCs which are not sample multiplexed will be excluded in the figure.";
                    string caption = "Non-Sample-Multiplexed RCCs Detected";
                    MainView.ShowErrorMessage(message, caption);
                }
            }
            // Check RCCs to ensure PKCs are associated
            List<Rcc> rccs = MainModel.Rccs.Where(x => e.IDs.Contains(x.ID)).ToList();
            if(rccs.Select(x => x.ThisRLF).Distinct().Any(y => y.Name == "DSP_v1.0"))
            {
                string message = "At least one cartridge's RCCs do not have PKCs selected. QC values based on probe counts will not be displayed for this cartridge's RCCs. To see these QC metrics in PlateView, Edit PKCs via the 'Edit' menu at the top of the main window and try again.";
                string caption = "Some PKCs Not Loaded";
                MainView.ShowErrorMessage(message, caption);
            }
            // Create plate view triad with RCCs that have PKCs associated
            Views.IRawCountsPlateView view = MVPFactory.RawCountPlateView(rccs.Where(x => x.ThisRLF.ThisType == RlfType.DSP 
                                                                                       || x.ThisRLF.ThisType == RlfType.PlexSet).ToList());
            if(view != null)
            {
                view.ShowThisDialog();
            }
        }

        private void View_OpenSampleVSampleScatterDialog(object sender, Views.RccSelectEventArgs e)
        {
            Views.SampleVsScatterplotView view = MVPFactory.SampleVsScatterView(MainModel.Rccs
                .Where(x => e.IDs.Contains(x.ID)).ToList());
            if(view != null)
            {
                view.ShowThisDialog();
            }
        }

        private void View_AssociatedPkcsMenuItemClicked(object sender, Views.RccSelectEventArgs e)
        {
            // Call Model method, passing selected IDs
            List<Tuple<string, string[]>> cartsAndPkcs = MainModel.GetDspCartIDs(e.IDs);
            Views.PkcSelectView view = MVPFactory.PkcView(cartsAndPkcs, MainModel.Pkcs);
            if(view != null)
            {
                view.ShowDialog();
            }
        }

        private void View_CountBinsMenuItemClicked(object sender, Views.RccSelectEventArgs e)
        {
            List<Rcc> rccs = MainModel.Rccs.Where(x => e.IDs.Contains(x.ID)).ToList();
            Views.BinnedCountsView view = MVPFactory.CountBinsView(rccs, new int[] { 0, 20, 50, 100, 500, 150000 });
            view.ShowDialog();
        }

        private void View_PcaOverviewMenuItemClicked(object sender, Views.RccSelectEventArgs e)
        {
            List<Rcc> rccs = MainModel.Rccs.Where(x => e.IDs.Contains(x.ID)).ToList();
            Views.PcaQcView view = MVPFactory.ThisPcaQcView(rccs);
            view.ShowForm();
        }

        private void View_EvaluateHousekeepersMenuItemClicked(object sender, Views.RccSelectEventArgs e)
        {
            List<Rcc> rccs = MainModel.Rccs.Where(x => e.IDs.Contains(x.ID)).ToList();
            Views.SelectHKsView view = MVPFactory.ThisSelectHKsView(rccs);
            view.ShowForm();
        }
    }
}

using MessageCenter;
using NCounterCore;
using RccAppDataModels;
using System;
using System.Collections.Generic;
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


        public MainViewPresenter(IMainView mainView, IDataModel mainModel)
        {
            // get model and view refs
            MainView = mainView;
            MainModel = mainModel;
            // hook events
            MainView.FilesLoading += new EventHandler(View_FilesLoading);
            MainView.FormLoaded += new EventHandler(View_FormLoaded);
            MainView.RccListCleared += new EventHandler(View_RccListCleared);
            MainView.ThresholdsUpdated += new EventHandler(View_ThresholdsUpdated);
            MainView.SelectingColumns += new EventHandler(View_SelectingColumns);
            MainView.ColumnsSelected += new EventHandler(View_ColumnsSelected);
            MainView.SortClick += new EventHandler(View_SortingColumns);
            MainModel.RccListChanged += new EventHandler(Model_RccListChanged);
            // Subscribe to password request message
            PresenterHub.MessageHub.Subscribe<PasswordRequestMessage>((m) => HandlePasswordRequest(m.Content));
        }

        private void HandlePasswordRequest(string fileName)
        {
            IPasswordEnterView view = MVPFactory.PasswordEnterView(fileName);
            if(view.ShowAsDialog() == DialogResult.OK)
            {
                PresenterHub.MessageHub.Publish<PasswordSendMessage>(new PasswordSendMessage(this, Tuple.Create(fileName, view.Password)));
            }
            // else if cancel or skip button clicked, do not continue to try extracting
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
    }
}

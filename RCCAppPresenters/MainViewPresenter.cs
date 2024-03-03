using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCCAppPresenters
{
    public class MainViewPresenter
    {
        private IMainView MainView { get; set; }
        private IDataModel Holder { get; set; }

        public MainViewPresenter(IMainView mainView, IDataModel holder)
        {
            MainView = mainView;
            Holder = holder;
            mainView.FilesLoading += new EventHandler(View_FilesLoading);
            mainView.ThresholdsSet += new EventHandler(View_ThresholdSet);
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
                    Holder.CreateObjectsFromFiles(ofd.FileNames, args.Index);
                    // Update binding for mainview Gv
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Handler for event called from View when thresholds are set in the threshold set diaglog
        /// </summary>
        /// <param name="sender">view window</param>
        /// <param name="e">empty</param>
        private void View_ThresholdSet(object sender, EventArgs e)
        {
            Holder.SetThresholds(MainView.CollectThresholds());
        }
    }
}

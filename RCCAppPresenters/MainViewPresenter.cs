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
        private IMainView RawDataView { get; set; }
        private IDataModel Holder { get; set; }

        public MainViewPresenter(IMainView rawDataView, IDataModel holder)
        {
            RawDataView = rawDataView;
            Holder = holder;
            rawDataView.FilesLoading += new EventHandler(View_FilesLoading);
            Holder.RCC_ListChanged += new EventHandler(Rccs_ListChanged);
        }

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
                    RawDataView.SetViewDataSource(Holder.Rccs);
                }
                else
                {
                    return;
                }
            }
        }

        private void Rccs_ListChanged(object sender, EventArgs e)
        {
            
        }
    }
}

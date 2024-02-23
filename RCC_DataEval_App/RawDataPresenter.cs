using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    public class RawDataPresenter
    {
        private IRawDataView RawDataView { get; set; }
        private IRawDataModel Holder { get; set; }
       
        public RawDataPresenter(IRawDataView rawDataView, IRawDataModel holder)
        {
            RawDataView = rawDataView;
            Holder = holder;
            rawDataView.FilesLoading += new EventHandler(View_FilesLoading);
        }

        public void View_FilesLoading(object sender, EventArgs e)
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
    }
}

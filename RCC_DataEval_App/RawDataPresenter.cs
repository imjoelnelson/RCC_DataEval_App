using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    internal class RawDataPresenter
    {
        private IRawDataView RawDataView { get; set; }
        private DataHolder Holder { get; set; }
       
        
        internal RawDataPresenter(IRawDataView rawDataView, DataHolder holder)
        {
            RawDataView = rawDataView;
            rawDataView.FilesLoaded += new EventHandler(Files_Loaded);
        }

        private void Files_Loaded(object sender, EventArgs e)
        {
            Holder.CreateObjectsFromFiles(RawDataView.FileNames, RawDataView.FileTypeIndex);
        }
    }
}

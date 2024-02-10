using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    public partial class RawDataView : Form, IRawDataView
    {
        // File loading
        public List<string> FileNames { get; set; }

        DBDataGridView Gv { get; set; }

        // Implementing IRawDataView events
        public event EventHandler FilesLoaded;
        private void loadRCCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "RCC;ZIP|*.RCC;*.ZIP";
                ofd.Title = "Select RCCs and/or ZIPs to load";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if(FileNames == null)
                    {
                        FileNames = new List<string>();
                    }
                    else
                    {
                        FileNames.Clear();
                    }
                    FileNames.AddRange(ofd.FileNames);
                    FilesLoaded.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    return;
                }
            }
        }

        public event EventHandler RccListCleared;
        public event EventHandler SentToQueue;
        public event EventHandler ExportToCsv;
        public event EventHandler CreateQCPlot;
        public event EventHandler ReorderRows;
        public event EventHandler Filter;
        public RawDataView(DataHolder holder)
        {
            InitializeComponent();

        }

        
    }
}

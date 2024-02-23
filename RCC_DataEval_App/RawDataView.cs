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
        // Presenter
        public RawDataPresenter Presenter { get; set; }
        // File loading
        public int FileTypeIndex { get; set; }

        // Data grid control
        private BindingSource Source { get; set; }
        private DBDataGridView Gv { get; set; }

        

        // Implementing IRawDataView events
        public event EventHandler FilesLoading;
        public event EventHandler RccListCleared;
        public event EventHandler SentToQueue;
        public event EventHandler ExportToCsv;
        public event EventHandler CreateQCPlot;
        public event EventHandler ReorderRows;
        public event EventHandler Filter;
        public RawDataView()
        {
            InitializeComponent();

            Source = new BindingSource();
            Gv = new DBDataGridView(true)
            {
                DataSource = Source
            };
            Gv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
        }

        // *****    Event Handling    *****
        private void LoadRCCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTypeIndex = 0;
            FilesLoadingEventArgs args = new FilesLoadingEventArgs(FileTypeIndex);
            FilesLoading.Invoke(this, args);
        }

        private void LoadRLFsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTypeIndex = 1;
            FilesLoadingEventArgs args = new FilesLoadingEventArgs(FileTypeIndex);
            FilesLoading.Invoke(this, args);
        }
        private void LoadPKCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileTypeIndex = 2;
            FilesLoadingEventArgs args = new FilesLoadingEventArgs(FileTypeIndex);
            FilesLoading.Invoke(this, args);
        }

        // *****    Methods    ******
        /// <summary>
        /// Public method for setting GridView binding source
        /// </summary>
        /// <param name="list">DataSource for the BindingSource</param>
        public void SetViewDataSource(BindingList<Rcc> list)
        {
            Source.DataSource = list;
            Source.ResetBindings(false);
        }

        public void ShowThis()
        {
            this.Show();
        }
    }
}

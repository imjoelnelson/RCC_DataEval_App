using NCounterCore;
using RCCAppPresenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreeViewMS;

namespace MainViewWinForms
{
    public partial class MainView : Form, IMainView
    {
        // Presenter
        public MainViewPresenter Presenter { get; set; }
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
        public MainView()
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

        /// <summary>
        /// Builds a tree view representing RCCs organized by common RLF; RLFs are parent nodes and RCCs are child nodes
        /// </summary>
        /// <param name="rcc">List of RCCs</param>
        /// <returns>A Treeview representing RCCs organized by common RLF</returns>
        private TreeViewMS.TreeViewMS GetRccTree(List<Rcc> rcc)
        {
            if (rcc == null)
            {
                throw new ArgumentException("The 'rcc' argument for GetRccTree's  cannot be null");
            }
            TreeViewMS.TreeViewMS returnObject = new TreeViewMS.TreeViewMS();
            IEnumerable<string> rlfNames = rcc.Select(x => x.ThisRLF.Name).Distinct();
            foreach (string s in rlfNames)
            {
                RccTreeNode node = new RccTreeNode(false, null, s);
                IEnumerable<Rcc> theseRccs = rcc.Where(x => x.ThisRLF.Name.Equals(s));
                foreach (Rcc r in theseRccs)
                {
                    node.Nodes.Add(new RccTreeNode(true, r, r.FileName));
                }

                returnObject.Nodes.Add(node);
            }

            return returnObject;
        }

        public void RccListChanged(List<Rcc> rccs)
        {
            TreeViewMS.TreeViewMS rccTree = GetRccTree(rccs);
            rccTree.Dock = DockStyle.Fill;
            rccTree.ShowPlusMinus = true;
            rccTree.ShowNodeToolTips = true;
            rccTree.Scrollable = true;
            splitContainer1.Panel1.Controls.Clear();
            splitContainer1.Panel1.Controls.Add(rccTree);
        }

    }
}


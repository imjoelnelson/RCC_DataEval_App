using NCounterCore;
using RCCAppPresenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public partial class MainView : Form, IMainView
    {
        // Presenter
        public MainViewPresenter Presenter { get; set; }
        // File loading
        public int FileTypeIndex { get; set; }

        // TreeView Conrol
        private List<string> SelectedProperties { get; set; }

        // Implementing IRawDataView events
        public event EventHandler FilesLoading;
        public event EventHandler ThresholdsSet;
        public event EventHandler RccListCleared;
        public event EventHandler SentToQueue;
        public event EventHandler ExportToCsv;
        public event EventHandler CreateQCPlot;
        public event EventHandler ReorderRows;
        public event EventHandler Filter;
        public MainView()
        {
            InitializeComponent();
            SelectedProperties = Properties.Settings.Default.SelectedProperties.Split(',').ToList();
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

        public void ShowThis()
        {
            this.Show();
        }

        /// <summary>
        /// Implementing method from IMainView; Method called by presenter when the RCC list changes
        /// </summary>
        /// <param name="rccs"></param>
        public void RccListChanged(List<Rcc2> rccs)
        {
            panel1.Controls.Clear();
            var treeView = McTreeViewFromProperties.GetTreeViewFromProperties(rccs, SelectedProperties);
            panel1.Controls.Add(treeView);
        }

        public QcThresholds CollectThresholds() // Too much connection with Model here; need to make more generic
        {
            QcThresholds retVal = new QcThresholds();
            retVal.ImagingThreshold = Properties.Settings.Default.ImagingThreshold;
            retVal.SprintDensityThreshold = Properties.Settings.Default.SprintDensityThreshold;
            retVal.DaDensityThreshold = Properties.Settings.Default.DaDensityThreshold;
            retVal.LinearityThreshold = Properties.Settings.Default.LinearityThreshold;
            retVal.LodSdCoefficient = Properties.Settings.Default.LodSdCoefficient;
            retVal.CountThreshold = Properties.Settings.Default.CountThreshold;

            return retVal;
        }

        private void setThresholdsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ThresholdSetFormcs form = new ThresholdSetFormcs())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ThresholdsSet.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}


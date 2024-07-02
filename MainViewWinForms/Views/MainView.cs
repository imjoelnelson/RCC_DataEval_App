using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public partial class MainView : Form, IMainView
    {
        #region properties
        /// <summary>
        /// This View's presenter
        /// </summary>
        public MainViewPresenter Presenter { get; set; }
        /// <summary>
        /// Index indicating which of the import toolstrip menu items has been clicked 
        /// </summary>
        public int FileTypeIndex { get; set; }

        // *****     DataGridView properties     *****
        /// <summary>
        /// Properties selected to be displayed as columns in main datagridview
        /// </summary>
        public List<string> SelectedProperties { get; set; }
        /// <summary>
        /// List of properties to sort datagridview by (passed to model by presenter to sort the bound list)
        /// </summary>
        public Dictionary<string, bool> SortList { get; set; }
        /// <summary>
        /// List of RlfTypes among selected RCCs
        /// </summary>
        public List<RlfType> SelectedRlfTypes { get; set; }
        /// <summary>
        /// Main display control for this form
        /// </summary>
        private DBDataGridView Dgv { get; set; }
        /// <summary>
        /// Font for main datagridview headers
        /// </summary>
        private static Font gvHeaderFont = new Font(DefaultFont, FontStyle.Bold);
        #endregion

        #region Events from interface
        // *****     Implementing IRawDataView events     *****
        /// <summary>
        /// Tells presenter there are files to pass to model for loading
        /// </summary>
        public event EventHandler FilesLoading;
        /// <summary>
        /// Triggers presenter to pass selected properties (i.e. columns to include) for building main datagridview
        /// </summary>
        public event EventHandler FormLoaded;
        /// <summary>
        /// Triggers presenter to tell model to clear the RCC binding list when Clear Button is clicked
        /// </summary>
        public event EventHandler RccListCleared;
        /// <summary>
        /// After thresholds dialog closed, triggers Presenter to tell Model to update flags in the RCC objects in the BindingList
        /// </summary>
        public event EventHandler ThresholdsUpdated;
        /// <summary>
        /// Tells presenter to grab RCC properties from static dictionary in Rcc to pass back to view for property selection Checked List Box
        /// </summary>
        public event EventHandler SelectingColumns;
        /// <summary>
        /// When view's select columns dialog is closed, triggers presenter to update the datagridview with selected columns as well as update Dgv width
        /// </summary>
        public event EventHandler ColumnsSelected;
        /// <summary>
        /// Indicates a column has been selected for sorting on
        /// </summary>
        public event EventHandler SortClick;
        /// <summary>
        /// Triggers presenter to clear any temp folders/files created when extracting zips (plus any future form closing actions)
        /// </summary>
        public event EventHandler ThisFormClosed;
        /// <summary>
        /// Triggers presenter to build csv from this forms main dgv
        /// </summary>
        public event EventHandler<Views.RccSelectEventArgs> BuildRawCountsTable;
        /// <summary>
        /// Triggers presenter to call for RawCountTablePrefs dialog from MVPFactory
        /// </summary>
        public event EventHandler OpenRawCountTablePreferences;
        /// <summary>
        /// Triggers presenter to have model search for types of selected RCCs to send back which options should be available
        /// </summary>
        public event EventHandler<Views.RccSelectEventArgs> DgvSelectionChanged;
        /// <summary>
        /// Triggers presenter to have model build a plate view raw counts table
        /// </summary>
        public event EventHandler<Views.RccSelectEventArgs> BuildPlateViewTable;
        /// <summary>
        /// Triggers Presenter to create a SampleVSample Scatter plot MVP triad and open form
        /// </summary>
        public event EventHandler<Views.RccSelectEventArgs> OpenSampleVSampleScatterDialog;
        /// <summary>
        /// Triggers Presenter to create an Associate PKC MVP triad
        /// </summary>
        public event EventHandler<Views.RccSelectEventArgs> AssociatePkcsMenuItemClicked;
        /// <summary>
        /// Triggers Presenter to launch a new Count Bins Stacked Bar Chart form
        /// </summary>
        public event EventHandler<Views.RccSelectEventArgs> CountBinsMenuItemClicked;
        #endregion

        public MainView()
        {
            InitializeComponent();
            SelectedProperties = Properties.Settings.Default.SelectedProperties.Split(',').ToList();
            SortList = new Dictionary<string, bool>(4);
            this.Load += new EventHandler(This_Load);
            panel1.Resize += new EventHandler(Panel1_Resize);
            this.WindowState = FormWindowState.Maximized;
        }

        #region methods
        public void ShowErrorMessage(string message, string caption)
        {
            MessageBox.Show(message, caption, MessageBoxButtons.OK);
        }

        /// <summary>
        /// Method for buidling the view's datagridview control
        /// </summary>
        /// <param name="properties">Collection of all properties that could be displayed as columns</param>
        /// <param name="selectedProperties">Sets which columns (properties) should be displayed</param>
        /// <param name="source">RCC binding source for the gridview, passed from model by presenter</param>
        public void SetDgv(Dictionary<string, Tuple<bool, string, int>> properties, 
            List<string> selectedProperties, BindingSource source)
        {
            panel1.Controls.Clear();
            Dgv = new DBDataGridView(true);
            Dgv.AllowUserToResizeColumns = true;
            Dgv.BackgroundColor = SystemColors.Window;
            Dgv.DataSource = source;
            Dgv.ScrollBars = ScrollBars.None;
            Dgv.Click += new EventHandler(Dgv_Click);
            Dgv.SelectionChanged += new EventHandler(Dgv_SelectionChanged);

            for (int i = 0; i < selectedProperties.Count; i++)
            {
                if(properties[selectedProperties[i]].Item1)
                {
                    DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
                    col.Name = selectedProperties[i];
                    col.HeaderText = properties[selectedProperties[i]].Item2;
                    col.HeaderCell.Style.Font = gvHeaderFont;
                    col.Width = properties[selectedProperties[i]].Item3;
                    col.DataPropertyName = selectedProperties[i];
                    col.TrueValue = false; // Because true/false based on pass but check based on fail
                    col.FalseValue = true; //     "                         "
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                    Dgv.Columns.Add(col);
                }
                else
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.Name = selectedProperties[i];
                    col.HeaderText = properties[selectedProperties[i]].Item2;
                    col.HeaderCell.Style.Font = gvHeaderFont;
                    col.Width = properties[selectedProperties[i]].Item3;
                    col.DataPropertyName = selectedProperties[i];
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                    Dgv.Columns.Add(col);
                }
            }

            Dgv.Visible = false; // Keep hidden until Rccs loaded
            SetDgvWidth(Dgv.Columns);
            panel1.Controls.Add(Dgv);
        }

        /// <summary>
        /// Sets the datagridview width based on selected columns
        /// </summary>
        /// <param name="cols">Collection of selected columns</param>
        private void SetDgvWidth(DataGridViewColumnCollection cols)
        {
            int retVal = 0;
            foreach(DataGridViewColumn col in cols)
            {
                retVal += col.Width;
            }

            Dgv.Width = retVal;
        }

        /// <summary>
        /// Collects thresholds from settings either on constructing the form or when ThresholdSet dialog is closed
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// For updating datagridview dimensions when columns/rows changed and updating sort glyphs when sorting on different columns
        /// </summary>
        /// <param name="count"></param>
        public void DgvSourceChanged(int count)
        {
            if(count > 0)
            {
                Dgv.Visible = true;
            }
            else
            {
                Dgv.Visible = false;
            }
            int h = 0;
            foreach(DataGridViewRow r in Dgv.Rows)
            {
                h += r.Height;
            }
            Dgv.Height = h + 24;
            if (panel1.AutoScrollMargin.Height < 20)
            {
                panel1.SetAutoScrollMargin(20, 20);
            }

            DgvSortGlyphHandling(SortList);
        }

        /// <summary>
        /// Gets mouseover row and column coordinates for right click event on main datagridview
        /// </summary>
        /// <param name="dgv">The main datagridview</param>
        /// <param name="X">e.X from mouseclick event</param>
        /// <param name="Y">e.Y from mouseclick event</param>
        /// <returns></returns>
        private Tuple<int, int> GetMouseOverCoordinates(DataGridView dgv, int X, int Y)
        {
            int currentMouseOverRow = dgv.HitTest(X, Y).RowIndex;
            int currentMouseOverCol = dgv.HitTest(X, Y).ColumnIndex;

            return Tuple.Create(currentMouseOverRow, currentMouseOverCol);
        }

        /// <summary>
        /// When SelectColumns item selected from column header right click context menu; displays SelectColumnsDialog
        /// </summary>
        /// <param name="columns">All RCC properties available</param>
        /// <param name="selectedProperties">RCC properties currently selected</param>
        public void ShowSelectColumnsDialog(List<Tuple<string, string>> columns, List<string> selectedProperties)
        {
            using(SelectColumnsDialog form = new SelectColumnsDialog(columns, selectedProperties.ToArray()))
            {
                if(form.ShowDialog() == DialogResult.OK)
                {
                    SelectedProperties = form.SelectedColumns;
                    ColumnsSelected.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Sets sort glyphs of Dgv columns based on columns selected for sorting and direction
        /// </summary>
        /// <param name="sortList">Dictionary indicating columns selected and bool for whether ascending</param>
        public void DgvSortGlyphHandling(Dictionary<string, bool> sortList)
        {
            // Clear previous sort glyphs
            foreach(DataGridViewColumn col in Dgv.Columns)
            {
                col.HeaderCell.SortGlyphDirection = SortOrder.None;
            }
            // Add sorty glyph for each column being sorted on
            foreach(KeyValuePair<string, bool> p in sortList)
            {
                Dgv.Columns[p.Key].HeaderCell.SortGlyphDirection = p.Value ? SortOrder.Ascending : SortOrder.Descending;
            }
        }

        /// <summary>
        /// Only needed if app has to close due to app folder not being able to be created
        /// </summary>
        public void FormClose()
        {
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// Saves matrix/table (string[][]) as a csv
        /// </summary>
        /// <param name="tableLines">the matrix/table (string[][]) to be saved</param>
        public void SaveTable(string[][] tableLines)
        {
            if (tableLines.Length > 0)
            {
                string path = string.Empty;
                using (System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog())
                {
                    sfd.Filter = "CSV|*.csv|TXT|*.txt";
                    sfd.RestoreDirectory = true;
                    sfd.ValidateNames = true;
                    if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        List<string> lines = new List<string>();
                        string csvSeparator;
                        if (sfd.FilterIndex == 1)
                        {
                            // <<< REPLACE SEPARATOR LATER WITH VALUE FROM PREFERENCES >>>
                            csvSeparator = ",";
                        }
                        else
                        {
                             csvSeparator = "\t";

                        }
                        lines.AddRange(tableLines.Select(x => string.Join(csvSeparator, x)));
                        try
                        {
                            File.WriteAllLines(sfd.FileName, lines);
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show($"{er.Message}\r\n\r\n{er.StackTrace}", "Table Error", MessageBoxButtons.OK);
                        }
                        path = sfd.FileName;
                    }
                }

                Util.OpenFileAfterSaved(path, 5000);
            }
        }

        /// <summary>
        /// Sets eval and analysis options available based on types present in selected RCCs
        /// </summary>
        /// <param name="types">The list of types present in the RCCs selected in the main Dgv</param>
        public void UpdateTypesPresent(List<RlfType> types)
        {
            SelectedRlfTypes = types;
            if(types.Contains(RlfType.DSP) || types.Contains(RlfType.PlexSet))
            {
                rawCountsPlateViewToolStripMenuItem.Enabled = true;
            }
            else
            {
                rawCountsPlateViewToolStripMenuItem.Enabled = false;
            }
        }

        /// <summary>
        /// Gets the IDs of selected rows to pass, via presenter, to methods in main model that require selected RCCs
        /// </summary>
        /// <returns>List of IDs of selected RCCs as a list of int</returns>
        private List<int> GetSelectedRows()
        {
            var selectedRows = Dgv.SelectedCells.Cast<DataGridViewCell>().Select(x => x.RowIndex).ToList();
            List<int> selectedIDs;
            if(selectedRows.Count > 0)
            {
                selectedIDs = new List<int>(selectedRows.Count);
                for (int i = 0; i < selectedRows.Count; i++)
                {
                    var temp = (Rcc)Dgv.Rows[selectedRows[i]].DataBoundItem;
                    selectedIDs.Add(temp.ID);
                }
            }
            else
            {
                selectedIDs = new List<int>(Dgv.Rows.Count);
                for (int i = 0; i < Dgv.Rows.Count; i++)
                {
                    var temp = (Rcc)Dgv.Rows[i].DataBoundItem;
                }
            }
            return selectedIDs;
        }
        #endregion

        #region Event Handling
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

        private void setThresholdsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (ThresholdSetFormcs form = new ThresholdSetFormcs())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ThresholdsUpdated.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void This_Load(object sender, EventArgs e)
        {
            FormLoaded.Invoke(this, EventArgs.Empty);
        }

        private void Panel1_Resize(object sender, EventArgs e)
        {
            if(Dgv != null)
            {
                if(Dgv.Columns != null)
                {
                    SetDgvWidth(Dgv.Columns);
                }
            }
        }

        private void clearRCCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RccListCleared.Invoke(this, EventArgs.Empty);
        }

        private void Dgv_Click(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;
            DataGridView temp = sender as DataGridView;
            Tuple<int, int> coords = GetMouseOverCoordinates(temp, args.X, args.Y);
            int row = coords.Item1;
            int col = coords.Item2;
            if (row == -1)
            {
                // Right click on column header
                if (args.Button == MouseButtons.Right)
                {
                    MenuItem[] items = new MenuItem[2];
                    items[0] = new MenuItem("Select Columns To Display", Item_SelectColumns);
                    items[1] = new MenuItem("Save Table As A CSV", Item_SaveAsCsv);

                    ContextMenu menu = new ContextMenu(items);
                    menu.Show(temp, new Point(args.X, args.Y));
                }
                // Left click on column header
                else if(args.Button == MouseButtons.Left)
                {
                    // Has the table been sorted on the clicked column already
                    bool clicked;
                    var found = SortList.TryGetValue(Dgv.Columns[col].Name, out clicked);

                    // If CTRL pressed when left mouse button clicked
                    if (ModifierKeys.HasFlag(Keys.Control))
                    {
                        // If this column not previously selected, add column to be sorted ascending
                        if (!found & SortList.Count < 4)
                        {
                            SortList.Add(Dgv.Columns[col].Name, true);
                        }
                        else
                        {
                            // If already sorted descending, remove the column from the sortlist
                            if (!clicked)
                            {
                                SortList.Remove(Dgv.Columns[col].Name);
                            }
                            // If not (i.e. sorted ascending), switch sorting to descending
                            else
                            {
                                SortList[Dgv.Columns[col].Name] = false;
                            }
                        }
                    }
                    // If CTRL not down when left mouse button clicked
                    else
                    {
                        // If this column not previously selected, clear list and then add column to be sorted ascending
                        if(!found)
                        {
                            SortList.Clear();
                            SortList.Add(Dgv.Columns[col].Name, true);
                        }
                        // If so check if already sorted descending
                        else
                        {
                            // If already sorted descending, set to default sorting
                            if (!clicked)
                            {
                                SortList.Clear();
                                SortList.Add("RlfName", true);
                                SortList.Add("Date", true);
                                SortList.Add("CartridgeID", true);
                                SortList.Add("LaneID", true);
                            }
                            // If not (i.e. sorted ascending), switch sorting to descending
                            else
                            {
                                SortList[Dgv.Columns[col].Name] = false;
                            }
                        }
                    }
                    // Send event to Presenter
                    SortClick.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void Item_SelectColumns(object sender, EventArgs e)
        {
            SelectingColumns.Invoke(this, EventArgs.Empty);
        }

        private void Item_SaveAsCsv(object sender, EventArgs e)
        {
            // Convert Dgv to string array
            string[] lines = Views.ViewUtils.ConvertDgvToStrings(Dgv);

            if(lines != null)
            {
                // Save lines to file
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = ".csv|*.csv";
                    sfd.RestoreDirectory = true;
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            File.WriteAllLines(sfd.FileName, lines);
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show($"{er.Message}\r\n\r\n{er.StackTrace}",
                                "File Write Error", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else
            {
                MessageBox.Show("Something went wrong converting the table to a CSV. Try again.",
                                   "CSV Conversion Failed", MessageBoxButtons.OK);
                return;
            }

        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ThisFormClosed.Invoke(this, EventArgs.Empty);
            this.Dispose();
        }

        private void rawCountsTableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Collect selected RCC IDs
            List<int> selectedIDs = GetSelectedRows();
            if(selectedIDs.Count > 0)
            {
                // Send message via presenter to model to build table
                BuildRawCountsTable.Invoke(this, new Views.RccSelectEventArgs(selectedIDs));
            }
            else
            {
                MessageBox.Show("Select RCCs (i.e. highlight rows) to include in the raw data table.", "No RCCs Selected", MessageBoxButtons.OK);
                return;
            }
        }

        private void rawCountTablePreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenRawCountTablePreferences.Invoke(this, EventArgs.Empty);
        }

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            Views.RccSelectEventArgs args = new Views.RccSelectEventArgs(GetSelectedRows());
            DgvSelectionChanged.Invoke(this, args);
        }

        private void rawCountsPlateViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Views.RccSelectEventArgs args = new Views.RccSelectEventArgs(GetSelectedRows());
            BuildPlateViewTable.Invoke(this, args);
        }

        private void sampleVsSampleCorrelationScatterplotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Views.RccSelectEventArgs args = new Views.RccSelectEventArgs(GetSelectedRows());
            OpenSampleVSampleScatterDialog.Invoke(this, args);
        }

        private void associatePKCsWithRCCsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var args = new Views.RccSelectEventArgs(GetSelectedRows());
            AssociatePkcsMenuItemClicked?.Invoke(this, args);
        }

        private void geneCountBinsStackedBarChartToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var args = new Views.RccSelectEventArgs(GetSelectedRows());
            CountBinsMenuItemClicked?.Invoke(this, args);
        }

        #endregion
    }
}


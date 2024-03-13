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
        // Presenter
        public MainViewPresenter Presenter { get; set; }
        // File loading
        public int FileTypeIndex { get; set; }

        // DataGridView properties
        private DBDataGridView Dgv { get; set; }
        public List<string> SelectedProperties { get; set; }

        private static Font gvHeaderFont = new Font(DefaultFont, FontStyle.Bold);
        public Dictionary<string, bool> SortList { get; set; }

        // Implementing IRawDataView events
        public event EventHandler FilesLoading;
        public event EventHandler FormLoaded;
        public event EventHandler RccListCleared;
        public event EventHandler ThresholdsUpdated;
        public event EventHandler SelectingColumns;
        public event EventHandler ColumnsSelected;
        public event EventHandler SortClick;
        public event EventHandler SentToQueue;
        public event EventHandler ExportToCsv;
        public event EventHandler CreateQCPlot;
        public event EventHandler ReorderRows;
        public event EventHandler Filter;
        public MainView()
        {
            InitializeComponent();
            SelectedProperties = Properties.Settings.Default.SelectedProperties.Split(',').ToList();
            SortList = new Dictionary<string, bool>(4);
            this.Load += new EventHandler(This_Load);
            panel1.Resize += new EventHandler(Panel1_Resize);
            this.WindowState = FormWindowState.Maximized;
        }

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

        private void SetDgvWidth(DataGridViewColumnCollection cols)
        {
            int retVal = 0;
            foreach(DataGridViewColumn col in cols)
            {
                retVal += col.Width;
            }

            Dgv.Width = retVal;
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
        /// Gets mouseover row and column coordinates for right click event
        /// </summary>
        /// <param name="X">e.X from mouseclick event</param>
        /// <param name="Y">e.Y from mouseclick event</param>
        /// <returns></returns>
        private Tuple<int, int> GetMouseOverCoordinates(DataGridView dgv, int X, int Y)
        {
            int currentMouseOverRow = dgv.HitTest(X, Y).RowIndex;
            int currentMouseOverCol = dgv.HitTest(X, Y).ColumnIndex;

            return Tuple.Create(currentMouseOverRow, currentMouseOverCol);
        }

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
                    MenuItem[] items = new MenuItem[4];
                    items[0] = new MenuItem("Select Columns To Display", Item_SelectColumns);
                    items[2] = new MenuItem("Save Table As A CSV", Item_SaveAsCsv);

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
            string[] lines = Util.ConvertDgvToStrings(Dgv);

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
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms.Views
{
    // The purpose of this figure is to allow users to easily see QC measures from their GeoMx protein readout in the context/position in the plate
    //  where the samples were hybridized; The data is displayed in a data grid view with one of 7 variables being displayed, the variable specified 
    //  by a combobox
    public partial class RawCountsPlateViewcs : Form, Views.IRawCountsPlateView
    {
        /// <summary>
        /// The display name and key for the callback to calculate the displayed QC values
        /// </summary>
        public string SelectedQcProperty { get; set; }
        /// <summary>
        /// The count threshold to be used for calculating the "Percent of Targets Above Threshold" QC metric
        /// </summary>
        public int Threshold { get; set; }
        /// <summary>
        /// Tab pages for each of the cartridges represented in the selected RCCs
        /// </summary>
        private List<RawCountPlatePage> Pages { get; set; }

        public event EventHandler<SelectedPlateViewEventArgs> CalculateQcMetricForSelectedPlateviewPage;
        public event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        public RawCountsPlateViewcs(List<string> cartridgeIDs)
        {
            InitializeComponent();

            plateTabControl.TabPages.Clear();
            Pages = new List<RawCountPlatePage>();
            for(int i = 0; i < cartridgeIDs.Count; i++)
            {
                var temp = new RawCountPlatePage(cartridgeIDs[i], i);
                plateTabControl.TabPages.Add(temp);
                Pages.Add(temp); // For holding an easier reference than the object boxed as a TabPage
            }

            Threshold = Properties.Settings.Default.CountThreshold;

            // Get display size based on screen size/res
            Screen screen = Screen.FromControl(this);
            int maxWidth = screen.Bounds.Width;
            int maxHeight = screen.WorkingArea.Bottom;
            plateTabControl.Size = new Size(Math.Min(1315, maxWidth - 20), Math.Min(390, maxHeight - 72));
            plateTabControl.Location = new Point(12, 12);
            this.Size = new Size(plateTabControl.Width + 20, plateTabControl.Height + 72);
            label1.Location = new Point(15, plateTabControl.Height + 23);
            label1.Size = new Size(302, 39);
            qcSelectorCombo.Location = new Point(label1.Location.X + label1.Width + 6, label1.Location.Y);
            qcSelectorCombo.Size = new Size(461, 39);

            // Events
            qcSelectorCombo.SelectedIndexChanged += new EventHandler(qcSelectorCombo_SelectedIndexChanged);
            plateTabControl.SelectedIndexChanged += new EventHandler(plateTabControl_SelectedIndexChanged);

            // Combobox items and selected index as well as dgv1 & 2 data set by presenter after View created
        }

        public void SetQcPropertySelectorComboItems(string[] items)
        {
            qcSelectorCombo.Items.AddRange(items);
            qcSelectorCombo.SelectedItem = items[0];
        }

        public void SetDgv1Values(string[][] mat, int index)
        {
            RawCountPlatePage page = Pages.Where(x => x.PageIndex == index).FirstOrDefault();
            if(page != null)
            {
                page.LaneDgv.SuspendLayout();
                page.LaneDgv.Rows.Clear();
                page.LaneDgv.Rows.Add(mat[0]);
                page.LaneDgv.Rows[0].HeaderCell.Value = "% FOV Counted";
                page.LaneDgv.Rows.Add(mat[1]);
                page.LaneDgv.Rows[1].HeaderCell.Value = "Binding Density";
                page.LaneDgv.ResumeLayout();
            }
        }

        public void SetDgv2Values(string[][] mat, int index)
        {
            RawCountPlatePage page = Pages.Where(x => x.PageIndex == index).FirstOrDefault();
            if(page != null)
            {
                page.WellDgv.SuspendLayout();
                page.WellDgv.Rows.Clear();
                for (int i = 0; i < mat.Count(); i++)
                {
                    page.WellDgv.Rows.Add(mat[i]);
                    page.WellDgv.Rows[i].HeaderCell.Value = char.ConvertFromUtf32(65 + i);
                }
                page.WellDgv.ResumeLayout();
            }
        }

        public void ShowThisDialog()
        {
            this.WindowState = FormWindowState.Maximized;
            this.ShowDialog();
        }

        public void CloseThisDialog()
        {
            this.Close();
        }

        private void qcSelectorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedQcProperty = (string)qcSelectorCombo.SelectedItem;
            SelectedPlateViewEventArgs args = new SelectedPlateViewEventArgs(plateTabControl.SelectedIndex);
            CalculateQcMetricForSelectedPlateviewPage?.Invoke(this, args);
        }

        private void plateTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedPlateViewEventArgs args = new SelectedPlateViewEventArgs(plateTabControl.SelectedIndex);
            CalculateQcMetricForSelectedPlateviewPage?.Invoke(this, args);
        }
    }
}

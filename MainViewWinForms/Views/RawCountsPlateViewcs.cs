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
    public partial class RawCountsPlateViewcs : Form, Views.IRawCountsPlateView
    {
        // The purpose of this figure is to allow users to easily see QC measures from their GeoMx protein readout in the context/position in the plate
        //  where the samples were hybridized; The data is displayed in a data grid view with one of 7 variables being displayed, the variable specified 
        //  by a combobox
        public string SelectedQcProperty { get; set; }
        public int Threshold { get; set; }

        public event EventHandler ComboBoxSelectionChanged;
        public event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        public RawCountsPlateViewcs(string cartridgeID)
        {
            InitializeComponent();

            this.Text = cartridgeID;

            Threshold = Properties.Settings.Default.CountThreshold;

            // Set DGV dims
            dataGridView1.RowHeadersWidth = 100;
            dataGridView2.RowHeadersWidth = 100;
            dataGridView1.Height = 2 * 34 + dataGridView1.ColumnHeadersHeight;
            dataGridView1.Width = 1202 + dataGridView1.RowHeadersWidth;
            label3.Location = new Point(label2.Location.X, dataGridView1.Location.Y + dataGridView1.Height + 2);
            dataGridView2.Location = new Point(dataGridView1.Location.X, 
                label3.Location.Y + label3.Height + 2);
            dataGridView2.Height = 266 + dataGridView2.ColumnHeadersHeight;
            dataGridView2.Width = dataGridView1.Width;

            // Get display size based on screen size/res
            Screen screen = Screen.FromControl(this);
            int maxWidth = screen.Bounds.Width;
            int maxHeight = screen.WorkingArea.Bottom;
            this.Size = new Size(Math.Min(dataGridView1.Width + 20, maxWidth), 
                Math.Min(dataGridView2.Location.Y + dataGridView2.Height + 20, maxHeight));

            // Combobox items and selected index as well as dgv1 & 2 data set by presenter after View created
        }

        public void SetQcPropertySelectorComboItems(string[] items)
        {
            qcSelectorCombo.Items.AddRange(items);
            qcSelectorCombo.SelectedItem = items[0];
        }

        public void SetDgv1Values(string[][] mat)
        {
            dataGridView1.SuspendLayout();
            dataGridView1.Rows.Add(mat[0]);
            dataGridView1.Rows[0].HeaderCell.Value = "% FOV Counted";
            dataGridView1.Rows.Add(mat[1]);
            dataGridView1.Rows[1].HeaderCell.Value = "Binding Density";
            dataGridView1.ResumeLayout();
        }

        public void SetDgv2Values(string[][] mat)
        {
            dataGridView2.SuspendLayout();
            dataGridView2.Rows.Clear();
            for(int i = 0; i < mat.Count(); i++)
            {
                dataGridView2.Rows.Add(mat[i]);
                dataGridView2.Rows[i].HeaderCell.Value = char.ConvertFromUtf32(65 + i);
            }
            dataGridView2.ResumeLayout();
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
            ComboBoxSelectionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

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
        
        public event EventHandler ComboBoxSelectionChanged;
        public event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        public RawCountsPlateViewcs(string cartridgeID, List<string> qcProperties)
        {
            InitializeComponent();

            this.Text = cartridgeID;

            // Get display size based on screen size/res
            Screen screen = Screen.FromControl(this);
            int maxWidth = screen.Bounds.Width;
            int maxHeight = screen.WorkingArea.Bottom;
            this.Size = new Size(Math.Min(2113, maxWidth), Math.Min(1241, maxHeight));

            foreach(string s in qcProperties)
            {
                qcSelectorCombo.Items.Add(s);
            }
        }

        private void qcSelectorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedQcProperty = (string)qcSelectorCombo.SelectedItem;
            ComboBoxSelectionChanged.Invoke(this, EventArgs.Empty);
        }
    }
}

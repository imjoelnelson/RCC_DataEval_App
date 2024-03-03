using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public partial class ThresholdSetFormcs : Form
    {
        public ThresholdSetFormcs()
        {
            InitializeComponent();

            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(imagingUpDown, "Threshold for FovCounted/FovCount ratio indicating the fraction of intended fields of view actually counted");
            ToolTip toolTip2 = new ToolTip();
            toolTip2.SetToolTip(sprintUpDown, "Binding density threshold for Sprint instruments");
            ToolTip toolTip3 = new ToolTip();
            toolTip3.SetToolTip(daUpDown, "Binding density threshold for Digital Analyzers");
            ToolTip toolTip4 = new ToolTip();
            toolTip4.SetToolTip(linearityUpDown, "Positive control linearity threshold indicating the minimum r^2 for correlation of log2 POS control concentrations vs. log2 POS control counts");
            ToolTip toolTip5 = new ToolTip();
            toolTip5.SetToolTip(coeffUpDown, "Coefficient for multiplying NEG control standard deviation when calculating limit of detection (LOD)");
            ToolTip toolTip6 = new ToolTip();
            toolTip6.SetToolTip(countUpDown, "Threshold for identifying 'genes above background' value");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ImagingThreshold = Convert.ToInt32(imagingUpDown.Value);
            Properties.Settings.Default.SprintDensityThreshold = Convert.ToInt32(sprintUpDown.Value);
            Properties.Settings.Default.DaDensityThreshold = Convert.ToInt32(daUpDown.Value);
            Properties.Settings.Default.LinearityThreshold = Convert.ToInt32(linearityUpDown.Value);
            Properties.Settings.Default.LodSdCoefficient = Convert.ToInt32(coeffUpDown.Value);
            Properties.Settings.Default.CountThreshold = Convert.ToInt32(countUpDown.Value);
            this.Close();
            this.Dispose();
        }
    }
}

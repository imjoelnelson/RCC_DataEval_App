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

            // Set tooltips
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

            // Set updown values
            imagingUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.ImagingThreshold);
            sprintUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.SprintDensityThreshold);
            daUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.DaDensityThreshold);
            linearityUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.LinearityThreshold);
            coeffUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.LodSdCoefficient);
            if(Properties.Settings.Default.CountThreshold > -1)
            {
                setToValRadioButton.Checked = true;
                countUpDown.Value = Convert.ToDecimal(Properties.Settings.Default.CountThreshold);
            }
            else
            {
                threshToLodRadioButton.Checked = true;
                countUpDown.Value = 40;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ImagingThreshold = Convert.ToDouble(imagingUpDown.Value);
            Properties.Settings.Default.SprintDensityThreshold = Convert.ToDouble(sprintUpDown.Value);
            Properties.Settings.Default.DaDensityThreshold = Convert.ToDouble(daUpDown.Value);
            Properties.Settings.Default.LinearityThreshold = Convert.ToDouble(linearityUpDown.Value);
            Properties.Settings.Default.LodSdCoefficient = Convert.ToInt32(coeffUpDown.Value);
            if(setToValRadioButton.Checked)
            {
                Properties.Settings.Default.CountThreshold = Convert.ToInt32(countUpDown.Value);
            }
            else
            {
                Properties.Settings.Default.CountThreshold = -1;
            }
            Properties.Settings.Default.Save();
            this.Close();
            this.Dispose();
        }
    }
}

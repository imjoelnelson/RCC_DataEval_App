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
    public partial class Form1 : Form
    {
        // Bound collection for main form DGV
        BindingList<Rcc> Rccs { get; set; }
        BindingSource RccSource { get; set; }
        DBDataGridView Gv { get; set; }

        // QC flagging thresholds (Used for determining QC flags in Rcc class)
        public static double ImagingPassThresh { get; set; }
        public static double DensityPassThreshDA { get; set; }
        public static double DensityPassThreshS { get; set; }
        public static double PosLinearityPassThresh { get; set; }
        public static int LODSDCoeff { get; set; }

        public Form1()
        {
            InitializeComponent();

            // *************   Pull From Settings   *****************
            ImagingPassThresh = Properties.Settings.Default.ImagingPassThresh;
            DensityPassThreshDA = Properties.Settings.Default.DensityPassThresh_DA;
            DensityPassThreshS = Properties.Settings.Default.DensityPassThresh_S;
            PosLinearityPassThresh = Properties.Settings.Default.PosLinearityPassThresh;
            LODSDCoeff = Properties.Settings.Default.LodSDCoeff;
        }

        //private void LoadFiles(string[] strings)
        //{
        //    Load RLFs into collection first, then PKCs, then RCCs
        //}
    }
}

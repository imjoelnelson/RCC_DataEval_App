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
        // QC flagging thresholds (Used for determining QC flags in Rcc class) PUT THIS ELSEWHERE
        public static double ImagingPassThresh { get; set; }
        public static double DensityPassThreshDA { get; set; }
        public static double DensityPassThreshS { get; set; }
        public static double PosLinearityPassThresh { get; set; }
        public static int LODSDCoeff { get; set; }

        // Display setting params
        /// <summary>
        /// Height of maximized window (for setting max height when window not maximized (i.e. when not at max width)
        /// </summary>
        public static int maxWidth { get; set; }
        /// <summary>
        /// Width of maximized window (for setting max width when window not maximized (i.e. when not at max height)
        /// </summary>
        public static int maxHeight { get; set; }

        public Form1()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;

            // *************   Pull From Settings   *****************
            ImagingPassThresh = Properties.Settings.Default.ImagingPassThresh;
            DensityPassThreshDA = Properties.Settings.Default.DensityPassThresh_DA;
            DensityPassThreshS = Properties.Settings.Default.DensityPassThresh_S;
            PosLinearityPassThresh = Properties.Settings.Default.PosLinearityPassThresh;
            LODSDCoeff = Properties.Settings.Default.LodSDCoeff;
        }

        #region Max height and width
        // Event for updating display settings when resolution/primary screen changed
        public void DisplaySettings_Changed(object sender, EventArgs e)
        {
            ChangeDisplaySettings();
        }

        // Event for updating display settings when window moved
        public void This_Move(object sender, EventArgs e)
        {
            ChangeDisplaySettings();
        }
        private void ChangeDisplaySettings()
        {
            Screen screen = Screen.FromControl(this);
            maxWidth = screen.Bounds.Width;
            maxHeight = screen.WorkingArea.Bottom;
            this.Height = maxHeight;
        }


        #endregion

        //private void LoadFiles(string[] strings)
        //{
        //    Load RLFs into collection first, then PKCs, then RCCs
        //}
    }
}

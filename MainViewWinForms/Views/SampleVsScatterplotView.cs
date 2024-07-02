using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace MainViewWinForms.Views
{
    public partial class SampleVsScatterplotView : Form, ISampleVsScatterplotView
    {
        /// <summary>
        /// Indicates whether count threshold cut off is used for filtering gene content for display
        /// </summary>
        public bool UseCutoff { get; set; }
        /// <summary>
        /// Count threshold for filtering gene content; based on value entered in numericUpDown1
        /// </summary>
        public int Threshold { get; set; }
        /// <summary>
        /// Binding source for the sample selection DGV
        /// </summary>
        public BindingSource Source { get; set; }
        /// <summary>
        /// List of target names for the content shown in the chart; for the mouse hover => identify point functionality
        /// </summary>
        public string[] TargetNames { get; set; }
        /// <summary>
        /// The panel holding the sample selection DGV
        /// </summary>
        private Panel panel2 { get; set; }
        /// <summary>
        /// Screen height, used for determining window/control heights
        /// </summary>
        private int MaxHeight { get; set; }
        /// <summary>
        /// The list for selecting samples to view in correlation chart
        /// </summary>
        private DBDataGridView Dgv { get; set; }
        /// <summary>
        /// For holding chart to be copied or saved via mouse right click handler
        /// </summary>
        private static Chart[] Charts = new Chart[1];
        /// <summary>
        /// Save menu item for context menu from right click handler
        /// </summary>
        private MenuItem save = new MenuItem("Save Chart", Save_onClick);
        /// <summary>
        /// Copy menu item for context menu from right click handler
        /// </summary>
        private MenuItem copy = new MenuItem("Copy Chart", Copy_onClick);
        /// <summary>
        /// <value>Previous position of tooltip (in mouse_move handler); to help avoid continuously regenerating if hovering over same point</value>
        /// </summary>
        private Point? PrevPosition = null;
        /// <summary>
        /// <value>The tool tip to provide point label</value>
        /// </summary>
        private ToolTip Tooltip = new ToolTip();

        public event EventHandler UseCutoffChanged;
        public event EventHandler ThresholdChanged;
        public event EventHandler<ScatterPlotSelectArgs> DgvCellContentClick;

        public SampleVsScatterplotView(int sampleCount)
        {
            InitializeComponent();

            checkBox1.Checked = true;
            UseCutoff = true;
            Threshold = Properties.Settings.Default.CountThreshold;
            numericUpDown1.Value = Threshold;

            this.WindowState = FormWindowState.Maximized;
            Screen screen = Screen.FromControl(this);
            MaxHeight = screen.WorkingArea.Bottom;
            int maxWidth = screen.WorkingArea.Right;

            Source = new BindingSource();
            SetDgv(sampleCount);
            Panel panel = new Panel();
            panel.AutoScroll = true;
            panel.Location = new Point(1, panel1.Location.Y + panel1.Height + 5);
            panel.Size = new Size(380, Math.Min((int)((sampleCount + 1) * 22.8), MaxHeight - 90));
            panel.Controls.Add(Dgv);
            this.Controls.Add(panel);

            panel2 = new Panel();
            panel2.Location = new Point(panel.Location.X + panel.Width + 5, 1);
            panel2.Size = new Size(this.ClientRectangle.Width - (panel2.Location.X + 1), MaxHeight - 5);
            panel2.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(panel2);

            this.Size = new Size(Math.Min(maxWidth, panel2.Location.X + panel2.Width + 10), Math.Min(MaxHeight, panel.Location.Y + panel.Height));
        }

        public void SetCorrelationChart(string[] targetNames, double[] xVals, double[] yVals, 
            string xName, string yName, Tuple<double, double> regLine, double rSquared)
        {
            TargetNames = targetNames;
            // Set up chart object and chart area
            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.Click += new EventHandler(Chart_RightClick);
            chart.MouseMove += new MouseEventHandler(Chart_MouseMove);
            ChartArea area = new ChartArea("area");
            area.AxisY = new Axis(area, AxisName.Y);
            area.AxisX = new Axis(area, AxisName.X);
            area.AxisX.Title = $"{xName}   log2 Counts";
            area.AxisX.Interval = 1;
            area.AxisX.Minimum = 0;
            area.AxisX.MajorGrid.LineWidth = 0;
            area.AxisY.Title = $"{yName}   log2 Counts";
            area.AxisY.Interval = 1;
            area.AxisY.Minimum = 0;
            area.AxisY.MajorGrid.LineWidth = 0;
            chart.ChartAreas.Add(area);
            // sample vs sample series (Points)
            Series series = new Series($"{xName}\r\nvs\r\n{yName}");
            series.ChartArea = "area";
            series.ChartType = SeriesChartType.Point;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 4;
            series.Points.DataBindXY(xVals, yVals);
            series.Legend = null;
            //Legend
            Legend leg = new Legend("leg");
            leg.IsDockedInsideChartArea = true;
            leg.Position = new ElementPosition(15, 3, 70, 5);
            leg.Alignment = StringAlignment.Near;
            leg.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            chart.Legends.Add(leg);
            // linear regression series (for displaying regression, r^2, and equation)
            Series series1 = new Series($"r^2 = {Math.Round(rSquared, 3).ToString()}\r\ny={Math.Round(regLine.Item2, 2).ToString()}x+{Math.Round(regLine.Item1, 2).ToString()}");
            series1.ChartArea = "area";
            series1.ChartType = SeriesChartType.FastLine;
            double[] xLinePoints = new double[] { 0.0, xVals.Max() };
            double[] yLinePoints = new double[] { regLine.Item1, (regLine.Item2 * xVals.Max()) + regLine.Item1 };
            series1.Points.DataBindXY(xLinePoints, yLinePoints);
            series1.MarkerStyle = MarkerStyle.None;
            series1.Legend = "leg";
            chart.Series.Add(series);
            chart.Series.Add(series1);

            panel2.Controls.Clear();
            foreach(Control c in panel2.Controls)
            {
                if(c != null)
                {
                    c.Dispose();
                }
            }
            panel2.Controls.Add(chart);
            if(!this.Controls.Contains(panel2))
            {
                this.Controls.Add(panel2);
            }
        }

        public void ShowThisDialog()
        {
            this.ShowDialog();
        }

        private void SetDgv(int sampleCount)
        {
            Dgv = new DBDataGridView(false);
            Dgv.DataSource = Source;
            Dgv.AllowUserToResizeColumns = false;
            Dgv.Dock = DockStyle.Fill;
            Dgv.AutoSize = false;
            Dgv.AutoGenerateColumns = false;
            Dgv.BackgroundColor = SystemColors.Window;
            Dgv.ColumnHeadersDefaultCellStyle.Font = new Font(Dgv.Font, FontStyle.Bold);
            Dgv.CellContentClick += new DataGridViewCellEventHandler(Dgv_ContentClicked);
            Dgv.Height = (30 * sampleCount) + 30;

            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = col.Name = "Sample Filename";
            col.ReadOnly = true;
            col.DataPropertyName = "Name";
            col.Width = 300;
            Dgv.Columns.Add(col);

            DataGridViewCheckBoxColumn col2 = new DataGridViewCheckBoxColumn();
            col2.HeaderText = col2.Name = "X";
            col2.ReadOnly = false;
            col2.DataPropertyName = "X";
            col2.Width = 30;
            Dgv.Columns.Add(col2);

            col2 = new DataGridViewCheckBoxColumn();
            col2.HeaderText = col2.Name = "Y";
            col2.ReadOnly = false;
            col2.DataPropertyName = "Y";
            col2.Width = 30;
            Dgv.Columns.Add(col2);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                UseCutoff = true;
                numericUpDown1.Enabled = true;
            }
            else
            {
                UseCutoff = false;
                numericUpDown1.Enabled = false;
            }

            UseCutoffChanged?.Invoke(this, EventArgs.Empty);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Threshold = (int)numericUpDown1.Value;
            ThresholdChanged?.Invoke(this, EventArgs.Empty);
        }

        private void Dgv_ContentClicked(object sender, DataGridViewCellEventArgs e)
        {
            ScatterPlotSelectArgs args = new ScatterPlotSelectArgs(e.RowIndex, e.ColumnIndex);
            DgvCellContentClick?.Invoke(this, args);
        }

        #region Events handled solely within View

        private void This_Load(object sender, EventArgs e)
        {
            
        }

        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            // Handle old location; handles re-hovering over same location (i.e. reduce processing for all intermediate points over a location)
            Point pos = e.Location;
            if (PrevPosition.HasValue && pos == PrevPosition.Value)
            {
                return;
            }

            // Process a new location
            Chart chart1 = sender as Chart;
            Tooltip.RemoveAll();
            PrevPosition = pos;
            HitTestResult[] results = chart1.HitTest(pos.X, pos.Y, false, ChartElementType.DataPoint);
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i].ChartElementType == ChartElementType.DataPoint)
                {
                    DataPoint prop = results[i].Object as DataPoint;
                    if (prop != null)
                    {
                        int index = results[i].PointIndex;
                        string pointName = TargetNames[index];

                        double xPixel = results[i].ChartArea.AxisX.ValueToPixelPosition(prop.XValue);
                        double yPixel = results[i].ChartArea.AxisY.ValueToPixelPosition(prop.YValues[0]);

                        // check if the cursor is really close to the point (2 pixels around the point, as opposed to only when directly over)
                        if (Math.Abs(pos.X - xPixel) < 2 && Math.Abs(pos.Y - yPixel) < 2)
                        {
                            Tooltip.Show(pointName, chart1, pos.X, pos.Y - 15);
                        }
                    }
                }
            }
        }

        private void Chart_RightClick(object sender, EventArgs e)
        {
            MouseEventArgs args = (MouseEventArgs)e;

            if (args.Button == MouseButtons.Right)
            {
                Chart temp = sender as Chart;
                Charts[1] = temp;
                MenuItem[] items = new MenuItem[] { save, copy };
                ContextMenu menu = new ContextMenu(items);
                menu.Show(temp, new Point(args.X, args.Y));
            }
        }

        private static void Save_onClick(object sender, EventArgs e)
        {
            Chart temp = Charts[0];

            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "JPEG|*.jpeg|PNG|*.png|BMP|*.bmp|TIFF|*.tiff|GIF|*.gif|EMF|*.emf|EmfDual|*.emfdual|EmfPlus|*.emfplus";
                    sfd.FileName = $"{DateTime.Now.ToString("yyyyMMdd_hhmmss")}_{temp.Text}";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        int i = sfd.FilterIndex;
                        switch (i)
                        {
                            case 0:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.Jpeg);
                                break;
                            case 1:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.Png);
                                break;
                            case 2:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.Bmp);
                                break;
                            case 3:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.Tiff);
                                break;
                            case 4:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.Gif);
                                break;
                            case 5:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.Emf);
                                break;
                            case 6:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.EmfDual);
                                break;
                            case 7:
                                temp.SaveImage(sfd.FileName, ChartImageFormat.EmfPlus);
                                break;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch(Exception er)
            {
                MessageBox.Show($"{er.Message}\r\n\r\n{er.StackTrace}", "Chart Save Failed", MessageBoxButtons.OK);
            }
        }

        private static void Copy_onClick(object sender, EventArgs e)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                Charts[0].SaveImage(ms, ChartImageFormat.Bmp);
                Bitmap bm = new Bitmap(ms);
                Clipboard.SetImage(bm);
            }
        }
        #endregion
    }
}

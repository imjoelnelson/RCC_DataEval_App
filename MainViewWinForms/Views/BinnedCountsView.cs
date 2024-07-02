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
    public partial class BinnedCountsView : Form, IBinnedCountsView
    {
        private Chart Chart1 { get; set; }
        private Series Series1 { get; set; }

        private static Font LittleFont = new System.Drawing.Font("Microsoft Sans Serif", 7F);
        private static Font SmallishFont = new System.Drawing.Font("Microsoft Sans Serif", 8F);
        private static Font MedFont = new System.Drawing.Font("Microsoft Sans Serif", 11F);
        private static Font BigFont = new System.Drawing.Font("Microsoft Sans Serif", 18F);

        public event EventHandler<Views.SelectedPlateViewEventArgs> ComboBoxIndexChanged;

        public BinnedCountsView()
        {
            InitializeComponent();

            comboBox1.Items.AddRange(new string[] { "Percent FOV Counted", "Binding Density", "POS Control GeoMean" });
            this.WindowState = FormWindowState.Maximized;
        }

        public void SetChart(double[][] vals, string[] binNames, string[] sampleIDs)
        {
            if (!vals.All(x => x.Length == vals[0].Length))
            {
                throw new Exception("Count bins exception: Data matrix is a jagged array. All series must have the same length.");
            }
            if (binNames.Length != vals.Length)
            {
                throw new Exception($"Count bins exception: Count bin name length ({binNames}) is different than data matrix length ({vals.Length})");
            }
            if (sampleIDs.Length != vals[0].Length)
            {
                throw new Exception($"Count bins exception: Sample ID array length ({sampleIDs}) is different than data matrix 'width' ({vals[0].Length})");
            }

            Chart1 = new Chart();
            Chart1.Dock = DockStyle.Fill;
            Chart1.MouseClick += new MouseEventHandler(Chart1_MouseClick);

            ChartArea area = new ChartArea("area");
            area.AxisX = new Axis(area, AxisName.X);
            area.AxisY = new Axis(area, AxisName.Y);
            area.AxisX.MajorGrid.LineWidth = 0;
            area.AxisX.Interval = 1;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = 1;
            area.AxisY.MajorGrid.LineWidth = 0;
            Chart1.ChartAreas.Add(area);
            area.AxisX.TitleFont = MedFont;
            area.AxisX.LabelStyle.Angle = 45;
            area.AxisY.LabelStyle.Font = LittleFont;
            area.AxisY.TitleFont = MedFont;

            Title title = new Title("Binned Count Percentages")
            {
                Font = BigFont,
                Alignment = ContentAlignment.MiddleLeft
            };
            Chart1.Titles.Add(title);

            Legend leg1 = new Legend("leg1")
            {
                IsDockedInsideChartArea = true,
                LegendStyle = LegendStyle.Row,
                Position = new ElementPosition(41, 5, 45, 5),
                Font = SmallishFont,
                TitleFont = MedFont
            };
            Chart1.Legends.Add(leg1);

            // Build up bar chart series
            Color[] colors = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Gray, Color.Blue, Color.HotPink };
            for (int i = 0; i < vals.Length; i++)
            {
                Chart1.Series.Add(new Series(binNames[i]));
                Chart1.Series[binNames[i]].IsValueShownAsLabel = false;
                Chart1.Series[binNames[i]].ChartType = SeriesChartType.StackedColumn;
                Chart1.Series[binNames[i]].Points.DataBindXY(sampleIDs, vals[i]);
                Chart1.Series[binNames[i]].Legend = "leg1";
                Chart1.Series[binNames[i]].Color = colors[i];
            }

            panel1.Controls.Add(Chart1);
        }

        public void SetChartOverlay(string name, double[] vals, string[] sampleIDs)
        {
            if (Series1 == null)
            {
                Chart1.ChartAreas["area"].AxisY2 = new Axis(Chart1.ChartAreas["area"], AxisName.Y2);
                Chart1.ChartAreas["area"].AxisY2.Title = name;
                Chart1.ChartAreas["area"].AxisY2.MajorGrid.LineWidth = 0;
                Chart1.ChartAreas["area"].AxisY2.TitleFont = MedFont;
                Chart1.ChartAreas["area"].AxisY2.LabelStyle.Font = LittleFont;
                Chart1.ChartAreas["area"].AxisY2.Enabled = AxisEnabled.True;

                Series1 = new Series(name);
                Series1.YAxisType = AxisType.Secondary;
                Series1.Color = Color.DarkBlue;
                Series1.MarkerStyle = MarkerStyle.Square;
                Series1.MarkerSize = 8;
                Chart1.Series.Add(Series1);
                Chart1.Series[name].ChartType = SeriesChartType.Line;
            }

            if (comboBox1.SelectedIndex > -1)
            {
                Series1.Name = name;
                Chart1.ChartAreas["area"].AxisY2.Title = name;
                Series1.Enabled = true;
                Series1.Points.DataBindXY(sampleIDs, vals);
                Chart1.ChartAreas["area"].AxisY2.Enabled = AxisEnabled.True;
            }
            else
            {
                Series1.Enabled = false;
                Chart1.ChartAreas["area"].AxisY2.Enabled = AxisEnabled.False;
            }
        }

        private void Chart1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                
                MenuItem item = new MenuItem("Save Chart As PNG", Save_Chart);
                ContextMenu menu = new ContextMenu(new MenuItem[] { item });
                menu.Show(Chart1, new Point(e.X, e.Y));
            }

        }

        private void Save_Chart(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG|*.png";
                sfd.RestoreDirectory = true;
                if(sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        Chart1.SaveImage(sfd.FileName, ChartImageFormat.Png);
                    }
                    catch(Exception er)
                    {
                        MessageBox.Show($"{er.Message}\r\n\r\n{er.StackTrace}",
                                        "Message Save Error",
                                        MessageBoxButtons.OK);
                    }
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var args = new Views.SelectedPlateViewEventArgs(comboBox1.SelectedIndex, -1, -1); // -1 == placeholder
            ComboBoxIndexChanged?.Invoke(this, args);
        }
    }
}

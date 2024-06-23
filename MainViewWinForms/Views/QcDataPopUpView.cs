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
    public partial class QcDataPopUpView : Form
    {
        public QcDataPopUpView(double[][] data, string title)
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
            this.Text = title ;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);

            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.Dock = DockStyle.Fill;
            int w = (flowLayoutPanel1.Width) / 12;
            int h = (flowLayoutPanel1.Height - 18) / 8;
            var chartSize = new Size(w, h);
            double dataMax = data.Select(x => x.Max()).Max();
            for(int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 12; j++)
                {
                    var chart = GetChart(data[i][j], dataMax + (0.5 * dataMax));
                    chart.Size = chartSize;
                    flowLayoutPanel1.Controls.Add(chart);
                    if(j == 11)
                    {
                        flowLayoutPanel1.SetFlowBreak(chart, true);
                    }
                }
            }
            flowLayoutPanel1.Visible = true;
        }

        private Chart GetChart(double val, double yLimit)
        {
            Chart chart = new Chart();
            // Chart area
            ChartArea area = new ChartArea("area");
            area.AxisX = new Axis(area, AxisName.X);
            area.AxisY = new Axis(area, AxisName.Y);
            area.AxisY.Title = "Counts";
            area.AxisX.MajorGrid.LineWidth = 0;
            area.AxisX.LabelStyle.Enabled = false;
            area.AxisY.Minimum = 0;
            area.AxisY.Maximum = yLimit;
            area.AxisY.RoundAxisValues();
            area.AxisY.LabelStyle.Enabled = false;
            area.AxisY.MajorGrid.LineWidth = 0;
            chart.ChartAreas.Add(area);
            // Chart series added
            Series ser = new Series();
            ser.ChartArea = "area";
            ser.ChartType = SeriesChartType.Column;
            ser.Points.AddXY(string.Empty, val);
            chart.Series.Add(ser);

            return chart;
        }
    }
}

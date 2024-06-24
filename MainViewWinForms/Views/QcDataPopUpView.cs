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
            // Add lane labels
            var labelFont = new Font("Microsoft Sans Serif", 16.125F, FontStyle.Bold);
            Label spacer = new Label();
            spacer.Width = 70;
            spacer.Text = string.Empty;
            flowLayoutPanel1.Controls.Add(spacer);
            for (int i = 1; i < 13; i++)
            {
                Label label = new Label();
                label.Width = w;
                label.Font = labelFont;
                label.Text = i.ToString();
                label.TextAlign = ContentAlignment.MiddleCenter;
                flowLayoutPanel1.Controls.Add(label);
                if(i == 12)
                {
                    flowLayoutPanel1.SetFlowBreak(label, true);
                }
            }
            // Add charts and row labels
            var chartSize = new Size(w, h);
            double dataMax = data.Select(x => x.Max()).Max();
            for(int i = 0; i < 8; i++)
            {
                Label label = new Label();
                label.Font = labelFont;
                label.Text = char.ConvertFromUtf32(65 + i);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = new Size(70, h);
                flowLayoutPanel1.Controls.Add(label);
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
            Label yRange = new Label();
            yRange.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            yRange.Text = $"Y Axis Range = 0 - {dataMax}";
            yRange.Width = 400;
            flowLayoutPanel1.Controls.Add(yRange);
        }

        private Chart GetChart(double val, double yLimit)
        {
            Chart chart = new Chart();
            // Chart area
            ChartArea area = new ChartArea("area");
            area.AxisX = new Axis(area, AxisName.X);
            area.AxisY = new Axis(area, AxisName.Y);
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

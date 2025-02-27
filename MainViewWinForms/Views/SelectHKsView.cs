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
    public partial class SelectHKsView : Form, ISelectHKsView
    {
        public SelectHKsView()
        {
            InitializeComponent();
        }

        private Chart OrderVsVariabilityChart(Tuple<double?, bool>[] input)
        {
            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.Click += new EventHandler(Chart_RightClick);

            ChartArea area = new ChartArea("area");
            area.AxisY = new Axis(area, AxisName.Y);
            area.AxisX = new Axis(area, AxisName.X);
            area.AxisX.Title = $"Order Removed";
            area.AxisX.Interval = 1;
            area.AxisX.Minimum = 0;
            area.AxisX.MajorGrid.LineWidth = 0;
            area.AxisY.Title = $"Pairwise Variation During Stepwise Selection";
            area.AxisY.Minimum = 0;
            area.AxisY.MajorGrid.LineWidth = 0;
            chart.ChartAreas.Add(area);

            Legend legend = new Legend("legend");
            legend.IsDockedInsideChartArea = true;
            legend.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            chart.Legends.Add(legend);

            Series series = new Series("Recommend To Keep");
            Series series1 = new Series("Recommend To Discard");
            series.ChartType = series1.ChartType = SeriesChartType.Point;
            series.ChartArea = series1.ChartArea = "area";
            series.MarkerBorderColor = series1.MarkerBorderColor = Color.Black;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerColor = Color.Black;
            series1.MarkerStyle = MarkerStyle.Circle;
            series1.MarkerColor = Color.White;
            for(int i = 0; i < input.Length; i++)
            {
                if(input[i].Item2)
                {
                    series.Points.AddXY(i, input[i].Item1);
                }
                else
                {
                    series1.Points.AddXY(i, input[i].Item1);
                }
            }

            series.Legend = "legend";
            series1.Legend = "legend";
            chart.Series.Add(series);
            chart.Series.Add(series1);

            

            return chart;
        }
        
        public void UpdateChart(Tuple<double?, bool>[] input)
        {
            Chart chart = OrderVsVariabilityChart(input);
            panel1.Controls.Add(chart);
        }

        public void ShowForm()
        {
            this.ShowDialog();
        }

        public void Chart_RightClick(object sender, EventArgs e)
        {

        }
    }
}

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
        private double[][] Data { get; set; }
        private string Title { get; set; }

        public QcDataPopUpView(double[][] data, string title)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            this.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            this.Load += new EventHandler(This_Load);

            Title = title;
            Data = data;
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

            chart.MouseClick += new MouseEventHandler(Chart_MouseClick);

            return chart;
        }

        // Handle flowLayoutPanel here rather than constructor to ensure dimensions are based on final layout panel size
        private void This_Load(object sender, EventArgs e)
        {
            flowLayoutPanel1.FlowDirection = FlowDirection.LeftToRight;
            flowLayoutPanel1.Dock = DockStyle.Fill;
            flowLayoutPanel1.MouseClick += new MouseEventHandler(Chart_MouseClick);

            // Get dimensions for controls to be added
            int w = (flowLayoutPanel1.Width - 200) / 12;
            int h = (flowLayoutPanel1.Height - 400) / 8;

            var labelFont = new Font("Microsoft Sans Serif", 16.125F, FontStyle.Bold);

            // Add main figure label
            Label main = new Label();
            main.Text = Title;
            main.Font = labelFont;
            main.Size = new Size(300, 40);
            flowLayoutPanel1.Controls.Add(main);
            flowLayoutPanel1.SetFlowBreak(main, true);
            
            // Add lane labels
            //    -Add spacer for row label width
            Label spacer = new Label();
            spacer.Width = 70;
            spacer.Text = string.Empty;
            //    -Add the labels
            flowLayoutPanel1.Controls.Add(spacer);
            for (int i = 1; i < 13; i++)
            {
                Label label = new Label();
                label.Width = w;
                label.Font = labelFont;
                label.Text = i.ToString();
                label.TextAlign = ContentAlignment.MiddleCenter;
                flowLayoutPanel1.Controls.Add(label);
                if (i == 12)
                {
                    flowLayoutPanel1.SetFlowBreak(label, true);
                }
            }

            // Add charts and row labels
            var chartSize = new Size(w, h);
            double dataMax = Data.Select(x => x.Max()).Max();
            for (int i = 0; i < 8; i++)
            {
                Label label = new Label();
                label.Font = labelFont;
                label.Text = char.ConvertFromUtf32(65 + i);
                label.TextAlign = ContentAlignment.MiddleCenter;
                label.Size = new Size(70, h);
                flowLayoutPanel1.Controls.Add(label);
                for (int j = 0; j < 12; j++)
                {
                    var chart = GetChart(Data[i][j], dataMax + (0.5 * dataMax));
                    chart.Size = chartSize;
                    flowLayoutPanel1.Controls.Add(chart);
                    if (j == 11)
                    {
                        flowLayoutPanel1.SetFlowBreak(chart, true);
                    }
                }
            }

            // Add Y-axis range label
            //     -Add spacer for row label width
            Label spacer1 = new Label();
            spacer1.Width = 70;
            spacer1.Text = string.Empty;
            flowLayoutPanel1.Controls.Add(spacer1);
            //     -Add the label
            Label yRange = new Label();
            yRange.Font = labelFont;
            yRange.Text = $"Y Axis Range = 0 - {dataMax}";
            yRange.Width = 400;
            yRange.Height = 40;
            flowLayoutPanel1.Controls.Add(yRange);
        }

        // Pops up context menu for saving the chart as a PNG
        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MenuItem item = new MenuItem("Save Chart As PNG", SaveAsPng);
                ContextMenu menu = new ContextMenu(new MenuItem[] { item });
                menu.Show((Control)sender, new Point(e.X, e.Y));
            }
        }

        private void SaveAsPng(object sender, EventArgs e)
        {
            int w = flowLayoutPanel1.Width;
            int h = flowLayoutPanel1.Height;
            Bitmap b = new Bitmap(w, h);
            try
            {
                flowLayoutPanel1.DrawToBitmap(b, new Rectangle(flowLayoutPanel1.Location, flowLayoutPanel1.Size));
            }
            catch(Exception er)
            {
                MessageBox.Show($"There was an error rendering the figure:\r\n{er.Message}\r\n\r\n{er.StackTrace}",
                                "Figure Rendering ERror",
                                MessageBoxButtons.OK);
            }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG|*.png";
                sfd.Title = "Save the window as a PNG";
                sfd.RestoreDirectory = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        b.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show($"There was an error saving the message:\r\n{er.Message}\r\n\r\n{er.StackTrace}",
                                        "Image Save Error",
                                        MessageBoxButtons.OK);
                    }
                }
            }
        }
    }
}

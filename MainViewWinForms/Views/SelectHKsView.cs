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
        /// <summary>
        /// When GeNorm PreFilter Settings button is clicked, sends event to presenter to trigger new chart and table given the changed settings
        /// </summary>
        public event EventHandler SettingsButtonClicked;
        
        public SelectHKsView()
        {
            InitializeComponent();

            this.WindowState = FormWindowState.Maximized;
        }

        /// <summary>
        /// Method for displaying any exceptions or error messages from model or presenter, via presenter
        /// </summary>
        public Tuple<string, bool> ErrorMessage
        {
            get { return _ErrorMessage; }
            set
            {
                if(value != _ErrorMessage)
                {
                    _ErrorMessage = value;
                    MessageBox.Show($"{ErrorMessage.Item1}", "Error", MessageBoxButtons.OK);
                    if(ErrorMessage.Item2)
                    {
                        CloseForm();
                    }
                }
            }
        }

        /// <summary>
        /// Method for creating pairwise variance vs stepwise removal chart and ranked housekeeping gene table given output from GeNorm (input)
        /// </summary>
        /// <param name="input">Ranked housekeeping gene list from GeNormCalculation (via model and presenter); item1 = gene name; item2 = pairwise variance; item3 = bool indicating if HK is included</param>
        public void UpdateChart(Tuple<string, double?, bool>[] input)
        {
            Chart chart = OrderVsVariabilityChart(input);
            panel1.Controls.Clear();
            panel1.Controls.Add(chart);

            int h = Math.Min(24 * input.Length, 2 + this.Height - panel2.Location.Y);
            List<Tuple<string, double?, bool>> temp = input.ToList();
            temp.Reverse();
            DBDataGridView table = GetHkTable(temp.ToArray());
            panel2.Controls.Clear();
            panel2.Controls.Add(table);
        }

        /// <summary>
        /// Method for calling class to show the form
        /// </summary>
        public void ShowForm()
        {
            this.ShowDialog();
        }

        /// <summary>
        /// Method for calling class to close the form
        /// </summary>
        public void CloseForm()
        {
            this.Close();
            this.Dispose(true);
        }

        /// <summary>
        /// Creates the pairwise variability vs stepwise removal chart from GeNorm
        /// </summary>
        /// <param name="input">Ranked housekeeping gene list from GeNormCalculation (via model and presenter); item1 = gene name; item2 = pairwise variance; item3 = bool indicating if HK is included</param>
        /// <returns>The pairwise variability vs stepwise removal point chart</returns>
        private Chart OrderVsVariabilityChart(Tuple<string, double?, bool>[] input)
        {
            Chart chart = new Chart();
            chart.Dock = DockStyle.Fill;
            chart.MouseClick += new MouseEventHandler(Chart_MouseClick);

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
            legend.LegendStyle = LegendStyle.Column;
            legend.Position = new ElementPosition(15, 5, 33, 7);
            legend.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
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
                if(input[i].Item3)
                {
                    series.Points.AddXY(i, input[i].Item2);
                }
                else
                {
                    series1.Points.AddXY(i, input[i].Item2);
                }
            }

            series.Legend = "legend";
            series1.Legend = "legend";
            chart.Series.Add(series);
            chart.Series.Add(series1);

            return chart;
        }

        /// <summary>
        /// Creates a DataGridView of the ranked list of filtered housekeepers indicating pairwise variation and whether or not they're discarded
        /// </summary>
        /// <param name="input">Ranked housekeeping gene list from GeNormCalculation (via model and presenter); item1 = gene name; item2 = pairwise variance; item3 = bool indicating if HK is included</param>
        /// <returns>DBDataGridView table of ranked housekeepers; column 0 = Gene Name, column 1 = pairwise variance, and column 2 = string indicating if housekeeper should be retained for normalization or discarded</returns>
        private DBDataGridView GetHkTable(Tuple<string, double?, bool>[] input)
        {
            DBDataGridView table = new DBDataGridView(true);
            table.Size = new Size(600, 27 + (22 * input.Length));
            table.BackgroundColor = SystemColors.Window;
            table.AllowUserToResizeColumns = true;
            table.ScrollBars = ScrollBars.None;
            table.MouseClick += new MouseEventHandler(Table_MouseClick);
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Gene Name";
            col.Width = 330;
            col.Resizable = DataGridViewTriState.True;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            col.ReadOnly = true;
            table.Columns.Add(col);
            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Pairwise Variation";
            col.Width = 165;
            col.Resizable = DataGridViewTriState.True;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            col.ReadOnly = true;
            table.Columns.Add(col);
            col = new DataGridViewTextBoxColumn();
            col.HeaderText = "Include";
            col.Width = 105;
            col.Resizable = DataGridViewTriState.True;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;
            col.ReadOnly = true;
            table.Columns.Add(col);

            for (int i = 0; i < input.Length; i++)
            {
                string pairVar = input[i].Item2 != null ? Math.Round((double)input[i].Item2, 3).ToString() : "NA";
                var discard = input[i].Item3 ? "Keep" : "Discard";
                table.Rows.Add(new string[] { input[i].Item1, pairVar, discard});
            }

            return table;
        }

        /// <summary>
        /// private field for ErrorMessage getter/setter
        /// </summary>
        private Tuple<string, bool> _ErrorMessage;

        private void Chart_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Table_MouseClick(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Handles the prefilter threshold settings button click (for sending event to open GeNorm prefilter settings view)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            SettingsButtonClicked.Invoke(sender, EventArgs.Empty);
        }
    }
}

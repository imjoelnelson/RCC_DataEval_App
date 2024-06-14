using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms.Views
{
    public class RawCountPlatePage : TabPage
    {
        /// <summary>
        /// 0-based index for order in tab control; for allowing presenter to easily link with item in Model
        /// </summary>
        public int PageIndex { get; set; }
        public DBDataGridView LaneDgv { get; set; }
        public DBDataGridView WellDgv { get; set; }

        public event EventHandler<DgvCellClickEventArgs> PlateViewCellClick;
        public event EventHandler<DgvCellClickEventArgs> PlateViewCellTooTipTextNeeded;

        public RawCountPlatePage(string cartridgeID, int index)
        {
            this.Text = cartridgeID;
            PageIndex = index;
            this.AutoScroll = true;

            // Create Lane Dgv label
            Label label1 = new Label();
            label1.Text = "Lane QC";
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            label1.Location = new System.Drawing.Point(12, 6);
            label1.Size = new System.Drawing.Size(115, 29);
            this.Controls.Add(label1);
            // Create Lane Dgv
            LaneDgv = new DBDataGridView(false);
            LaneDgv.Location = new System.Drawing.Point(0, 36);
            LaneDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            LaneDgv.RowHeadersVisible = true;
            LaneDgv.RowHeadersWidth = 100;
            LaneDgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            LaneDgv.Height = 2 * 30 + LaneDgv.ColumnHeadersHeight;
            LaneDgv.Width = 1202 + LaneDgv.RowHeadersWidth;
            for (int i = 0; i < 12; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = col.HeaderText = (i + 1).ToString();
                col.Width = 100;
                col.ReadOnly = true;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                LaneDgv.Columns.Add(col);
            }
            this.Controls.Add(LaneDgv);

            // Create Well Dgv Label
            Label label2 = new Label();
            label2.Text = "Well QC";
            label2.Font = label1.Font;
            label2.Location = new System.Drawing.Point(12, LaneDgv.Location.Y + LaneDgv.Height + 12);
            label2.Size = new System.Drawing.Size(110, 29);
            this.Controls.Add(label2);
            // Create Well Dgv
            WellDgv = new DBDataGridView(false);
            WellDgv.Location = new System.Drawing.Point(LaneDgv.Location.X,
                label2.Location.Y + label2.Height);
            WellDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            WellDgv.RowHeadersVisible = true;
            WellDgv.RowHeadersWidth = 100;
            WellDgv.Height = 8 * WellDgv.RowTemplate.Height + WellDgv.ColumnHeadersHeight;
            WellDgv.Width = LaneDgv.Width;
            WellDgv.CellContentClick += new DataGridViewCellEventHandler(WellDgv_CellContentClick);
            WellDgv.CellToolTipTextNeeded += new DataGridViewCellToolTipTextNeededEventHandler(WellDgv_CellToolTipTextNeeded);
            for(int i = 0; i < 12; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.Name = col.HeaderText = (i + 1).ToString();
                col.Width = 100;
                col.ReadOnly = true;
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                WellDgv.Columns.Add(col);
            }
            this.Controls.Add(WellDgv);
        }

        private void WellDgv_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            // Get row and column and send event
        }
        private void WellDgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Function to create csv
        }
    }
}

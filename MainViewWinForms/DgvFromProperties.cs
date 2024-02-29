using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public class DgvFromProperties
    {
        public DBDataGridView Dgv { get; set; }
        public DgvFromProperties(List<Tuple<string, string>> properties)
        {
            Dgv = new DBDataGridView(true);
            Dgv.ColumnHeadersDefaultCellStyle.Font = new Font(Dgv.Font.FontFamily, Dgv.Font.Size, FontStyle.Bold);
            Dgv.AllowUserToResizeColumns = true;
            // Binding from presenter method to avoid specifying class-specific types here


            for(int i = 0; i < properties.Count; i++)
            {
                if(properties[i].Item1.EndsWith("Pass"))
                {
                    DataGridViewCheckBoxColumn col = new DataGridViewCheckBoxColumn();
                    col.HeaderText = properties[i].Item2;
                    col.DataPropertyName = properties[i].Item1;
                    col.TrueValue = true;
                    col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;
                    col.ReadOnly = true;
                    Dgv.Columns.Add(col);
                }
                else
                {
                    DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                    col.HeaderText = properties[i].Item2;
                    col.DataPropertyName = properties[i].Item1;
                    col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
                    col.SortMode = DataGridViewColumnSortMode.NotSortable; // Sort using custom sort method
                    col.ReadOnly = true;
                    Dgv.Columns.Add(col);
                }
            }



        }
    }
}

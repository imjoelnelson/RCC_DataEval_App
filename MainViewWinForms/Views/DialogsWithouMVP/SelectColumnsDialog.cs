using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public partial class SelectColumnsDialog : Form
    {
        private List<Tuple<string, string>> Columns { get; set; }
        public List<string> SelectedColumns { get; set; }
        public SelectColumnsDialog(List<Tuple<string, string>> columns, string[] selected)
        {
            InitializeComponent();

            Columns = columns;
            for(int i = 0; i < Columns.Count; i++)
            {
                checkedListBox1.Items.Add(columns[i].Item2, selected.Contains(columns[i].Item1));
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SelectedColumns = Columns.Where((x, i) => checkedListBox1.CheckedIndices.Contains(i))
                                     .Select(y => y.Item1).ToList();
                                     
            // Save selected columns setting
            Properties.Settings.Default.SelectedProperties = string.Join(",", SelectedColumns);
            Properties.Settings.Default.Save();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

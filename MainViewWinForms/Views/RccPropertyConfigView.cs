using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms.Views
{
    public partial class RccPropertyConfigView : Form , IRccPropertyConfigView
    {
        public List<string> SelectedProperties { get; set; }
        private List<Tuple<string, string>> RccProperties { get; set; }

        public event EventHandler OkButtonClicked;

        public RccPropertyConfigView(List<Tuple<string, string>> properties, List<string> selectedProperties)
        {
            InitializeComponent();

            if (properties == null)
            {
                throw new ArgumentException("Properties list cannot be null.");
            }

            RccProperties = properties;
            SelectedProperties = new List<string>(RccProperties.Count);

            checkedListBox1.Height = Convert.ToInt32(15.5 * properties.Count);
            this.Height = label1.Location.Y + label1.Height + checkedListBox1.Height + okButton.Height + 50;

            // Add property items and make checked if present in selected properties
            foreach(Tuple<string, string> kvp in RccProperties)
            {
                checkedListBox1.Items.Add(kvp.Item2, selectedProperties.Contains(kvp.Item1));
            }
        }

        public void ShowForm()
        {
            this.ShowDialog();
        }

        public void CloseForm()
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            foreach(object o in checkedListBox1.CheckedItems)
            {
                SelectedProperties.Add(RccProperties.Where(x => x.Item2.Equals((string)o)).First().Item1);
            }
            OkButtonClicked.Invoke(this, EventArgs.Empty);
        }
    }
}

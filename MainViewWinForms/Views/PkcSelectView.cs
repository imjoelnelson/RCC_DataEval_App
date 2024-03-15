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
    public partial class PkcSelectView : Form, IPkcSelectView
    {
        public string[] SelectedPkcs { get; set; }

        public event EventHandler AddButtonCicked;
        public event EventHandler<PkcAddRemoveArgs> RemoveButtonClicked;
        public event EventHandler NextButtonClicked;

        public PkcSelectView()
        {
            InitializeComponent();

            SelectedPkcs = new string[8];
        }

        public void CloseForm()
        {
            this.Close();
            this.Dispose();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            AddButtonCicked.Invoke(this, EventArgs.Empty);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem != null)
            {
                // Send event with name of selected PKC
                var args = new PkcAddRemoveArgs(((KeyValuePair<string, string>)listBox1.SelectedItem).Key);
                RemoveButtonClicked.Invoke(this, args);
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            NextButtonClicked.Invoke(this, EventArgs.Empty);
        }

        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            if(listBox1.SelectedItem != null)
            {
                // Set sneder textbox text to name of selected PKC
                TextBox box = sender as TextBox;
                box.Text = ((KeyValuePair<string, string>)listBox1.SelectedItem).Key;
                // Set SelectedPkcs array, at the index of textbox in question, to the same PKC name from listbox selection
                SelectedPkcs[(int)box.Tag] = box.Text;
            }
        }
    }
}

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
        public event EventHandler AddButtonCicked;
        public event EventHandler<PkcAddRemoveArgs> RemoveButtonClicked;
        public event EventHandler NextButtonClicked;
        public event EventHandler<PkcSelectBoxEventArgs> TabPageListBox2DoubleClicked;

        public PkcSelectView(List<string> cartIDs)
        {
            InitializeComponent();

            TabControl tabControl = new TabControl();
            tabControl.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular);
            tabControl.ShowToolTips = true;
            tabControl.ItemSize = new Size(100, 20);
            tabControl.SizeMode = TabSizeMode.Fixed;
            tabControl.Dock = DockStyle.Fill;
            tabControl.TabPages.Clear();
            for(int i = 0; i < cartIDs.Count; i++)
            {
                PkcSelectTabPage page = new PkcSelectTabPage(cartIDs[i]);
                page.ListBox2DoubleClicked += new EventHandler<PkcSelectBoxEventArgs>(TabPage_ListBox2DoubleClicked);
                tabControl.TabPages.Add(page);
            }
            this.panel1.Controls.Add(tabControl);
            tabControl.Focus();
        }

        public void ShowForm()
        {
            this.ShowDialog();
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
                // Set sender textbox text to name of selected PKC
                TextBox box = sender as TextBox;
                box.Text = ((KeyValuePair<string, string>)listBox1.SelectedItem).Key;
            }
        }

        private void TabPage_ListBox2DoubleClicked(object sender, PkcSelectBoxEventArgs e)
        {
            TabPageListBox2DoubleClicked.Invoke(sender, e);
        }
    }
}

using RccAppDataModels;
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
        private TabControl TabControl1 { get; set; }

        public event EventHandler AddButtonCicked;
        public event EventHandler<PkcAddRemoveArgs> RemoveButtonClicked;
        public event EventHandler NextButtonClicked;
        public event EventHandler<PkcSelectBoxEventArgs> SelectButtonClicked;
        public event EventHandler<PkcSelectBoxEventArgs> CartridgeRemoveButtonClicked;

        public PkcSelectView(List<string> cartIDs)
        {
            InitializeComponent();

            TabControl1 = new TabControl();
            TabControl1.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular);
            TabControl1.ShowToolTips = true;
            TabControl1.ItemSize = new Size(100, 20);
            TabControl1.SizeMode = TabSizeMode.Fixed;
            TabControl1.Dock = DockStyle.Fill;
            TabControl1.TabPages.Clear();
            this.panel1.Controls.Add(TabControl1);
            TabControl1.Focus();

            for (int i = 0; i < cartIDs.Count; i++)
            {
                Views.PkcSelectTabPage page = new Views.PkcSelectTabPage(cartIDs[i]);
                TabControl1.TabPages.Add(page);
            }
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
                var args = new PkcAddRemoveArgs((string)listBox1.SelectedItem);
                RemoveButtonClicked.Invoke(this, args);
            }
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            NextButtonClicked.Invoke(this, EventArgs.Empty);
        }

        private void SelectButton_Clicked(object sender, EventArgs e)
        {
            if(listBox1.SelectedItems.Count > 0)
            {
                // Passing tab page event through to presenter
                string[] names = new string[listBox1.SelectedItems.Count];
                for (int i = 0; i < listBox1.SelectedItems.Count; i++)
                {
                    names[i] = (string)listBox1.SelectedItems[i];
                }
                var args = new PkcSelectBoxEventArgs(TabControl1.SelectedTab.Text, names);
                SelectButtonClicked.Invoke(sender, args);
            }
        }

        private void TabPage_RemoveButtonClicked(object sender, EventArgs e)
        {
            PkcSelectTabPage page = (PkcSelectTabPage)TabControl1.SelectedTab;
            string[] names = new string[page.listBox2.SelectedItems.Count];
            for (int i = 0; i < page.listBox2.SelectedItems.Count; i++)
            {
                names[i] = (string)page.listBox2.SelectedItems[i];
            }
            var args = new PkcSelectBoxEventArgs(page.Text, names);
            CartridgeRemoveButtonClicked.Invoke(this, args);
        }

        public void UpdateCartridgePkcBox(string cartridgeID, string[] pkcNames)
        {
            foreach(TabPage p in TabControl1.TabPages)
            {
                if(p.Text.Equals(cartridgeID))
                {
                    PkcSelectTabPage page = (PkcSelectTabPage)p;
                    page.listBox2.Items.Clear();
                    foreach(string s in pkcNames)
                    {
                        page.listBox2.Items.Add(s);
                    }
                }
            }
        }

        public void UpdateSavedPkcBox(string[] pkcNames)
        {
            listBox1.Items.Clear();
            for(int i = 0; i < pkcNames.Length; i++)
            {
                listBox1.Items.Add(pkcNames[i]);
            }
        }
    }
}

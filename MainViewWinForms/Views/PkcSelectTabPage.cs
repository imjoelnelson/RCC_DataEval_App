using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms.Views
{
    public class PkcSelectTabPage : TabPage
    {
        public string CartridgeId { get; set; }
        private PkcSelectView ParentView { get; set; }
        public ListBox listBox2 { get; set; }
        public BindingSource ListSource { get; set; }

        public event EventHandler<PkcSelectBoxEventArgs> ListBox2DoubleClicked;

        public PkcSelectTabPage(string cartridgeID)
        {
            // Misc settings
            ListSource = new BindingSource();
            this.Text = this.Name = CartridgeId = cartridgeID;
            ParentView = this.Parent as PkcSelectView;
            this.Size = new Size(465, 459);

            // Page label
            Font labelsFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
            Label topLabel = new Label();
            topLabel.Font = labelsFont;
            topLabel.Text = $"Selected PKCs";
            topLabel.Location = new Point(12, 9);
            topLabel.Size = new Size(439, 25);
            this.Controls.Add(topLabel);

            // Page listbox for selected PKCs
            listBox2 = new ListBox();
            listBox2.Location = new Point(6, 37);
            listBox2.Size = new Size(442, 429);
            listBox2.DataSource = ListSource;
            listBox2.DisplayMember = "Key";
            listBox2.DoubleClick += new EventHandler(ListBox2_DoubleClick);
            this.Controls.Add(listBox2);

            // Setting of ListSource datasource done in presenter method
        }

        private void ListBox2_DoubleClick(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection selected = listBox2.SelectedItems;
            string[] selectedPkcNames = new string[selected.Count];
            for(int i = 0; i < selected.Count; i++)
            {
                selectedPkcNames[i] = ((KeyValuePair<string, string>)(selected[i])).Key;
            }
            var args = new PkcSelectBoxEventArgs(CartridgeId, selectedPkcNames);
            ListBox2DoubleClicked.Invoke(this, args);
        }
    }
}

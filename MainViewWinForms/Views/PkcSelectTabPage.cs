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

        public PkcSelectTabPage(string cartridgeID)
        {
            // Misc settings
            this.Text = this.Name = CartridgeId = cartridgeID;
            ParentView = this.Parent as PkcSelectView;
            this.Size = new Size(465, 499);

            // Page label
            Font labelsFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
            Label topLabel = new Label();
            topLabel.Font = labelsFont;
            topLabel.Text = $"Selected PKCs";
            topLabel.Location = new Point(2, 3);
            topLabel.Size = new Size(439, 20);
            this.Controls.Add(topLabel);

            // Page listbox for selected PKCs
            listBox2 = new ListBox();
            listBox2.Location = new Point(2, 23);
            listBox2.Size = new Size(213, 238);
            listBox2.DoubleClick += new EventHandler(ListBox2_DoubleClick);
            this.Controls.Add(listBox2);
        }

        /// <summary>
        /// Copies PKC names from Parent View listbox1 selected items to tab page's selected items list and sends PKC names to Model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListBox2_DoubleClick(object sender, EventArgs e)
        {
            ListBox.SelectedObjectCollection selectedItems = ParentView.listBox1.SelectedItems;
            string[] selectedPaths = new string[selectedItems.Count];
            for(int i = 0; i < selectedItems.Count; i++)
            {
                selectedPaths[i] = ((KeyValuePair<string, string>)selectedItems[i]).Key;
            }
            var args = new PkcSelectBoxEventArgs(CartridgeId, selectedPaths);
        }
    }
}

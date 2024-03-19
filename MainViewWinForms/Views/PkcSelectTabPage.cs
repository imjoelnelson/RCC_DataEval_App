using System;
using System.Collections.Generic;
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
        private PkcSelectView ParentView;
        private CheckBox addForAllRowsCheckBox;
        public event EventHandler<PkcSelectBoxEventArgs> TextboxModified;

        public PkcSelectTabPage(string cartridgeID)
        {
            ParentView = this.Parent as PkcSelectView;
            this.Size = new Size(465, 1020);

            Font labelsFont = new Font("Microsoft Sans Serif", 8F, FontStyle.Bold);
            Label topLabel = new Label();
            topLabel.Font = labelsFont;
            topLabel.Text = $"Select PKCs for each plate row used for:";
            topLabel.Location = new Point(12, 9);
            topLabel.Size = new Size(439, 25);
            this.Controls.Add(topLabel);

            Label bottomLabel = new Label();
            bottomLabel.Font = labelsFont;
            bottomLabel.Text = cartridgeID;
            bottomLabel.Location = new Point(10, 37);
            bottomLabel.Size = new Size(439, 25);
            this.Controls.Add(bottomLabel);

            addForAllRowsCheckBox = new CheckBox();
            addForAllRowsCheckBox.Text = "Apply PKC(s) to all rows";
            addForAllRowsCheckBox.Checked = true;
            addForAllRowsCheckBox.Location = new Point(15, 96);
            addForAllRowsCheckBox.Size = new Size(275, 29);
            this.Controls.Add(addForAllRowsCheckBox);

            for(int i = 0; i < 8; i++)
            {
                TextBox textBox = new TextBox();
                textBox.ReadOnly = true;
                textBox.Multiline = true;
                textBox.Location = new Point(15, 131 + (111 * i));
                textBox.Size = new Size(352, 105);
                textBox.Tag = i;
                textBox.DoubleClick += new EventHandler(TextBox_DoubleClick);
                textBox.TextChanged += new EventHandler(TextBox_TextChanged);
                this.Controls.Add(textBox);
            }
        }

        private void TextBox_DoubleClick(object sender, EventArgs e)
        {
            if(addForAllRowsCheckBox.Checked)
            {
                // Set all textboxes to the same set of PKCs
                for(int i = 0; i < this.Controls.Count; i++)
                {
                    if(this.Controls[i].GetType() == typeof(TextBox))
                    {
                        TextBox box = (TextBox)this.Controls[i];
                        box.Lines = new string[ParentView.listBox1.Items.Count];
                        for(int j = 0; j < box.Lines.Length; j++)
                        {
                            box.Lines[j] = (string)ParentView.listBox1.Items[j];
                        }
                    }
                }
            }
            else
            {
                // Set the clicked text box to the selected set of PKCs
                TextBox box = (TextBox)sender;
                box.Lines = new string[ParentView.listBox1.Items.Count];
                for (int i = 0; i < box.Lines.Length; i++)
                {
                    box.Lines[i] = (string)ParentView.listBox1.Items[i];
                }
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox box = (TextBox)sender;
            PkcSelectBoxEventArgs args = new PkcSelectBoxEventArgs((int)box.Tag, CartridgeId, box.Lines);
            TextboxModified.Invoke(this, args);
        }
    }
}

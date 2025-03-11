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
    public partial class GeNormPreFilterSettingsView : Form, IGenormPreFilterSettingsView
    {
        /// <summary>
        /// Sends event to presenter to set the count threshold and average count threshold settings in Properties.Settings.Default
        /// </summary>
        public event EventHandler OKButtonClicked;
        /// <summary>
        /// Getter and setter for the Property Setting GeNormPreFilterCountThreshold
        /// </summary>
        public int CountThreshold
        {
            get { return (int)numericUpDown1.Value; }
            set
            {
                if(numericUpDown1.Value != value)
                {
                    if(value > 0)
                    {
                        numericUpDown1.Value = value;
                    }
                    else
                    {
                        numericUpDown1.Value = 0;
                    }
                }
            }
        }
        public bool UseCountThreshold
        {
            get { return checkBox1.Checked; }
            set
            {
                if(checkBox1.Checked != value)
                {
                    checkBox1.Checked = value;
                }
            }
        }
        /// <summary>
        /// Getter and setter for the Property setting GeNormPreFilterAvgCountThreshold
        /// </summary>
        public int AvgCountThreshold
        {
            get { return (int)numericUpDown2.Value; }
            set
            {
                if(numericUpDown2.Value != value)
                {
                    if(value > 0)
                    {
                        numericUpDown2.Value = value;
                    }
                    else
                    {
                        numericUpDown2.Value = 0;
                    }
                }
            }
        }

        public bool UseAvgCountThreshold
        {
            get { return checkBox2.Checked; }
            set
            {
                if(checkBox2.Checked != value)
                {
                    checkBox2.Checked = value;
                }
            }
        }

        public GeNormPreFilterSettingsView()
        {
            InitializeComponent();
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

        /// <summary>
        /// Handles OK button click event, firing the EventHandler to send to presenter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            OKButtonClicked.Invoke(sender, EventArgs.Empty);
        }
    }
}

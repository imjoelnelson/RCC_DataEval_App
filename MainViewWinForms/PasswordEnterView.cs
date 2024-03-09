using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public partial class PasswordEnterView : Form, RccAppPresenters.IPasswordEnterView
    {
        public string CurrentPassword { get; set; }

        public event EventHandler PasswordEntered;
        public event EventHandler Skipped;
        public PasswordEnterView(string filePath)
        {
            InitializeComponent();
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            fileNameTextBox.Text = $"Enter password for {fileName}";
        } 

        public void CloseForm()
        {
            this.Close();
            this.Dispose();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CurrentPassword = passwordTextBox.Text;
            PasswordEntered.Invoke(this, EventArgs.Empty);
        }

        private void skipButton_Click(object sender, EventArgs e)
        {
            Skipped.Invoke(this, EventArgs.Empty);
        }
    }
}

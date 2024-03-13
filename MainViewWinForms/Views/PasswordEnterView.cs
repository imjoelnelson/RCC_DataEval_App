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
    public partial class PasswordEnterView : Form, IPasswordEnterView
    {
        public PasswordEnterPresenter PasswordPresenter { get; set; }
        public string Password { get; set; }

        public event EventHandler PasswordEntered;
        public event EventHandler Skipped;

        public PasswordEnterView(string filePath)
        {
            InitializeComponent();
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            fileNameTextBox.Text = $"Enter password for {fileName}";
        } 

        public DialogResult ShowAsDialog()
        {
            return this.ShowDialog();
        }

        public void CloseForm()
        {
            this.Close();
            this.Dispose();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Password = passwordTextBox.Text;
            PasswordEntered.Invoke(this, EventArgs.Empty);
        }

        private void skipButton_Click(object sender, EventArgs e)
        {
            Skipped.Invoke(this, EventArgs.Empty);
        }
    }
}

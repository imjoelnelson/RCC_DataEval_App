using System;
using System.IO;
using System.Windows.Forms;

namespace RccAppDataModels
{
    public partial class PasswordEnter : Form
    {
        public string Password { get; set; }
        public PasswordEnter(string filePath)
        {
            InitializeComponent();
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            textBox2.Text = $"Enter password for {fileName}";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Password = textBox1.Text;
            this.Close();
        }
    }
}

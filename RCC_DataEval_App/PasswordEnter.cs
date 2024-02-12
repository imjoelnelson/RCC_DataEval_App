using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    public partial class PasswordEnter : Form
    {
        public string Password { get; set; }
        public PasswordEnter(string fileName)
        {
            InitializeComponent();

            textBox2.Text = $"Enter Password for: {System.IO.Path.GetFileName(fileName)}";
        }
    }
}

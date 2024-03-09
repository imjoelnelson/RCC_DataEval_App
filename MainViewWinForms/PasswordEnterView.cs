using RCCAppPresenters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public partial class PasswordEnterView : Form
    {
        public string CurrentPassword { get; set; }

        public event EventHandler PasswordEntered;
        public event EventHandler Skipped;
        public PasswordEnterView()
        {
            InitializeComponent();
        }
    }
}

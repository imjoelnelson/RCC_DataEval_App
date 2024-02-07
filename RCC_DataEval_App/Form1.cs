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
    public partial class Form1 : Form
    {
        // Bound collection for main form DGV
        BindingList<Rcc> Rccs { get; set; }
        BindingSource RccSource { get; set; }
        DBDataGridView Gv { get; set; }
        public Form1()
        {
            InitializeComponent();

        }

        //private void LoadFiles(string[] strings)
        //{
        //    Load RLFs into collection first, then PKCs, then RCCs
        //}
    }
}

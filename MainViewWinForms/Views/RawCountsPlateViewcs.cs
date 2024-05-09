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
    public partial class RawCountsPlateViewcs : Form
    {
        public RawCountsPlateViewcs(string cartridgeID)
        {
            InitializeComponent();

            this.Text = cartridgeID;

            Screen screen = Screen.FromControl(this);
            int maxWidth = screen.Bounds.Width;
            int maxHeight = screen.WorkingArea.Bottom;
            this.Size = new Size(Math.Min(2113, maxWidth), Math.Min(1241, maxHeight));
        }


    }
}

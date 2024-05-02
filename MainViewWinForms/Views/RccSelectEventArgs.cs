using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class RccSelectEventArgs : EventArgs
    {
        public List<int> IDs { get; set; }

        public RccSelectEventArgs(List<int> ids)
        {
            IDs = ids;
        }
    }
}

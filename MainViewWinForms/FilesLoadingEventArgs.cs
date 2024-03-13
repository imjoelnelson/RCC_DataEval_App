using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    public class FilesLoadingEventArgs : EventArgs
    {
        public FilesLoadingEventArgs(int index)
        {
            Index = index;
        }
        public int Index { get; set; }
    }
}

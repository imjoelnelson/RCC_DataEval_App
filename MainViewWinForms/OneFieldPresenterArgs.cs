using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    public class OneFieldPresenterArgs : EventArgs
    {
        public string Name { get; set; }
        public OneFieldPresenterArgs(string name)
        {
            Name = name;
        }
    }
}

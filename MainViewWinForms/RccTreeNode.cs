using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    public class RccTreeNode : System.Windows.Forms.TreeNode
    {
        public RccTreeNode(bool isRcc, Rcc thisRcc, string name)
        {
            IsRcc = isRcc;
            ThisRcc = thisRcc;
            Name = name;
        }

        public bool IsRcc { get; set; }
        public Rcc ThisRcc { get; set; }
    }
}

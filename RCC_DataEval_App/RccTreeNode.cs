using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCC_DataEval_App
{
    public class RccTreeNode : System.Windows.Forms.TreeNode
    {
        public RccTreeNode(bool isRcc, Rcc thisRcc)
        {
            IsRcc = isRcc;
            ThisRcc = thisRcc;
        }

        public bool IsRcc { get; set; }
        public Rcc ThisRcc { get; set; }
    }
}

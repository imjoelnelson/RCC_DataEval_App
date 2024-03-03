using NCounterCore;
using Syncfusion.Windows.Forms.Tools.MultiColumnTreeView;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    public class McTreeViewFromProperties
    {
        public static MultiColumnTreeView GetTreeViewFromProperties(List<Rcc2> rccs, List<string> propertiesIncluded)
        {
            MultiColumnTreeView treeView = new MultiColumnTreeView
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                SelectionMode = TreeSelectionMode.MultiSelectSameLevel
            };

            HashSet<string> hash = new HashSet<string>(propertiesIncluded);

            // Add columns
            foreach(string s in Rcc2.PropertyOrder.Keys)
            {
                if(hash.Contains(s))
                {
                    TreeColumnAdv col = new TreeColumnAdv
                    {
                        Text = Rcc2.PropertyDisplayNames[s],
                        Width = GetTextWidth(Rcc2.PropertyDisplayNames[s], treeView.Font)
                    };
                    treeView.Columns.Add(col);
                }
            }

            // Split RCCs by RLF 
            IEnumerable<string> rlfs = rccs.Select(x => x.StringProperties["Rlf"]).Distinct();
            foreach(string rlf in rlfs)
            {
                // Add a node for the RLF name
                TreeNodeAdv rlfNode = new TreeNodeAdv(rlf);
                treeView.Nodes.Add(rlfNode);
                // Get RCCs with the same RLF
                IEnumerable<Rcc2> rccsByRlf = rccs.Where(x => x.StringProperties["Rlf"].Equals(rlf));
                // Get all the cartIDs
                IEnumerable<string> cartIDs = rccsByRlf.Select(x => x.StringProperties["CartridgeID"]).Distinct();
                // Split by cartID
                foreach(string cartID in cartIDs)
                {
                    // Add node for the cartID
                    rlfNode.Nodes.Add(new TreeNodeAdv(cartID));
                    // Get RCCs with same cartID
                    IEnumerable<Rcc2> rccsByCartID = rccsByRlf.Where(x => x.StringProperties["CartridgeID"].Equals(cartID));
                    foreach (Rcc2 r in rccsByCartID)
                    {
                        TreeNodeAdv node = GetTreeNodeAdvFromRcc(r, hash);
                        treeView.Nodes.Add(node);
                    }
                }
            }

            return treeView;
        }

        private static TreeNodeAdv GetTreeNodeAdvFromRcc(Rcc2 rcc, HashSet<string> propsIncluded)
        {
            TreeNodeAdv node = new TreeNodeAdv(rcc.StringProperties["FileName"]);
            
            foreach(string s in Rcc2.PropertyOrder.Keys)
            {
                if(propsIncluded.Contains(s))
                {
                    TreeNodeAdvSubItem item = new TreeNodeAdvSubItem();
                    if(Rcc2.PropertyOrder[s] == PropertyType.StringType)
                    {
                        item.Text = rcc.StringProperties[s];
                    }
                    else if(Rcc2.PropertyOrder[s] == PropertyType.Numeric)
                    {
                        var test = s;
                        item.Text = rcc.DoubleProperties[s].ToString();
                    }
                    else
                    {
                        item.Text = rcc.BoolProperties[s] ? string.Empty : "<<FLAG>>";
                    }
                    node.SubItems.Add(item);
                }
            }

            return node;
        }

        private static int GetTextWidth(string text, Font font)
        {
            Size size = System.Windows.Forms.TextRenderer.MeasureText(text, font);
            return size.Width;
        }
    }
}

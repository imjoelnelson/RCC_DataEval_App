using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCCAppPresenters
{
    public class HeaderClickEventArgs : EventArgs
    {
        public string ColumnName { get; set; }

        public HeaderClickEventArgs(string columnName)
        {
            ColumnName = columnName;
        }
    }
}

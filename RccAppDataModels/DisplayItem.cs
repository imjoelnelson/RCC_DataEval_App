using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class DisplayItem
    {
        public string Dis { get; set; }
        public string Val { get; set; }

        public DisplayItem(string dis, string val)
        {
            Dis = dis;
            Val = val;
        }
    }
}

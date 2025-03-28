using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class RlfItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool ProbeIDsAndSeqInfoLoaded { get; set; }
        public List<int> AssocaitedProbeIDs { get; set; }

        public static Dictionary<string, string> PsRowTranslate = new Dictionary<string, string>()
        {
            { "1", "A" }, { "2", "B" }, { "3", "C" }, { "4", "D" }, { "5", "E" }, { "6", "F" }, { "7", "G" }, { "8", "H" }
        };
        public static Dictionary<string, string> PsTranslateRow = new Dictionary<string, string>()
        {
            { "A", "1" }, { "B", "2" }, { "C", "3" }, { "D", "4" }, { "E", "5" }, { "F", "6" }, { "G", "7" }, { "H", "8" }
        };

        public RlfItem(string name, int id, bool fromRlfFile)
        {
            ID = id;
            Name = name;
            ProbeIDsAndSeqInfoLoaded = fromRlfFile;
            AssocaitedProbeIDs = new List<int>();
        }

        public void AddProbeMainKeyIDs(List<int> ids)
        {
            // Any checks for content count? Since this is before RLF loaded, not sure what to check against
            AssocaitedProbeIDs.AddRange(ids);
        }
    }
}

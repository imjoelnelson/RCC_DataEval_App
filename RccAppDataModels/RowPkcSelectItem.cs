using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class RowPkcSelectItem
    {
        public string CartridgeID { get; private set; }
        public int Row { get; private set; }
        public List<string> RowSelectedPkcs { get; private set; }
        public PkcCollector DspIdTranslator { get; private set; }
        public bool Compatible { get; private set; }

        public RowPkcSelectItem() { }

        public void SetRowPkcSelectItem(string cartridgeID, int row, List<Tuple<string, string>> rowSelectedPkcs)
        {
            CartridgeID = cartridgeID;
            Row = row;
            RowSelectedPkcs = rowSelectedPkcs.Select(x => x.Item1).ToList();
            DspIdTranslator = new PkcCollector(rowSelectedPkcs.Select(x => new PkcReader(x.Item1)));
            Compatible = DspIdTranslator.OverlappingIDs.Count < 1;
        }
    }
}

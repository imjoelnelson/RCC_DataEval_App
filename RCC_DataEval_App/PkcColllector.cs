using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RCC_DataEval_App
{
    /// <summary>
    /// Checks to make sure all included PKCs don't include duplicate DSP_IDs (i.e. are compatible) and then merges the DSP_ID to ProbeItem dictionaries
    /// </summary>
    internal class PkcColllector
    {
        public Dictionary<string, ProbeItem> MergedTranslator { get; private set; }
        
        internal PkcColllector(IEnumerable<PkcReader> readers)
        {
            if(readers.Count() > 1)
            {
                Dictionary<string, ProbeItem> merged = new Dictionary<string, ProbeItem>(readers.Select(x => x.DspTranslator.Count).Sum());
                foreach (PkcReader r in readers)
                {
                    foreach (var p in r.DspTranslator)
                    {
                        if (!merged.ContainsKey(p.Key))
                        {
                            merged.Add(p.Key, p.Value);
                        }
                        else
                        {
                            MessageBox.Show($"The selected PKCs could not be imported because the DSP_ID, {p.Key} is present in at least two of them. Check to ensure the correct PKCs were selected.",
                                "Incompatible PKCs Detected",
                                MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
                MergedTranslator = merged;
            }
            else
            {
                if(readers.Count() > 0)
                {
                    MergedTranslator = readers.ElementAt(0).DspTranslator;
                }
            }
        }
    }
}

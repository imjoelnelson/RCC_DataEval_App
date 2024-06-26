﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCounterCore
{
    /// <summary>
    /// Checks to make sure all included PKCs don't include duplicate DSP_IDs (i.e. are compatible) and then merges the DSP_ID to ProbeItem dictionaries
    /// </summary>
    public class PkcCollector : IPkcReader
    {
        public string Name { get; set; }
        public Dictionary<string, ProbeItem> DspTranslator { get; private set; }
        public List<string> OverlappingIDs { get; private set; }

        public PkcCollector(IEnumerable<IPkcReader> readers)
        {
            OverlappingIDs = new List<string>();
            
            if (readers.Count() > 1)
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
                            OverlappingIDs.Add(p.Key);
                        }
                    }
                }
                DspTranslator = merged;
                Name = string.Join("$", readers.Select(x => x.Name));
            }
            else if (readers.Count() > 0)
            {
                DspTranslator = readers.ElementAt(0).DspTranslator;
                Name = readers.ElementAt(0).Name;
            }
            else
            {
                DspTranslator = new Dictionary<string, ProbeItem>();
                Name = string.Empty;
            }
        }
    }
}

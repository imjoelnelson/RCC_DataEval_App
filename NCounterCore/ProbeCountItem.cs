using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class ProbeCountItem
    {
        /// <summary>
        /// Numeric ID for the sample the count comes from
        /// </summary>
        public int SampleID { get; set; }
        /// <summary>
        /// Numeric ID for the probe the count is for
        /// </summary>
        public int ProbeID { get; set; }
        /// <summary>
        /// The probe count from the original RCC (or, for miRNA, after ligation background correction)
        /// </summary>
        public double RawCount { get; set; }
        /// <summary>
        /// List of norm factors for this probe, one for each normalization group the count is included in; key = NormalizationGroup ID
        /// </summary>
        public List<KeyValuePair<int, double>> NormFactors { get; set; }

        public ProbeCountItem(int samp, int prob, double raw)
        {
            SampleID = samp;
            ProbeID = prob;
            RawCount = raw;
        }
    }
}

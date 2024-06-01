using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public delegate double GetValue(int[] input);
    public class QcPropertyItem
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public QcPropertyItem(string name, int[] input, GetValue callback)
        {
            Name = name;
            Value = callback(input);
        }
    }
    /// <summary>
    /// Item for binding plateview DGVf
    /// </summary>
    public class PlateDisplayItem
    {
        /// <summary>
        /// Column index for the well
        /// </summary>
        public int ColumnIndex { get; set; }
        /// <summary>
        /// Row index for the well
        /// </summary>
        public int RowIndex { get; set; }
        /// <summary>
        /// Well POS Hyb Control Count
        /// </summary>
        public int PosCount { get; set; }
        /// <summary>
        /// Well NEG Hyb Control Count
        /// </summary>
        public int NegCount { get; set; }
        /// <summary>
        /// GeoMx protein well assay POS geomean
        /// </summary>
        public double AssayPosGeoMean { get; set; }
        /// <summary>
        /// GeoMx protein well assay NEG geomean
        /// </summary>
        public double AssayNegGeoMean { get; set; }
        /// <summary>
        /// PlexSet well Housekeeping gene geomean
        /// </summary>
        public double HkGeoMean { get; set; }
        /// <summary>
        /// Highest endogenous target count in well
        /// </summary>
        public int EndoMax { get; set; }
        /// <summary>
        /// Lowest endogenous target count in well
        /// </summary>
        public int EndoMin { get; set; }
        /// <summary>
        /// Percent of target counts above user set threshold (same value as the RCC property with same name)
        /// </summary>
        public double PctAboveThresh { get; set; }

        public PlateDisplayItem(int posCount, int negCount, double assayPosGeomean, double assayNegMean, double hkGeomean, int endoMax, int endoMin, double pctAboveThresh)
        {
            PosCount = posCount;
            NegCount = negCount;
            AssayPosGeoMean = assayPosGeomean;
            AssayNegGeoMean = AssayNegGeoMean;
            HkGeoMean = hkGeomean;
            EndoMax = endoMax;
            EndoMin = endoMin;
            PctAboveThresh = pctAboveThresh;
        }

        public PlateDisplayItem(Rcc rcc, int plexRow)
        {
            // Set indices
            ColumnIndex = rcc.LaneID - 1; // Adjusted for 1-based counting of lane IDs
            RowIndex = plexRow;

            // Get probes for cell
            string let = char.ConvertFromUtf32(41 + plexRow).ToString();
            List<ProbeItem> probes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow.Equals(let))
                                                       .Select(x => x.Value)
                                                       .ToList();

            // Get POS count
            var posProbe = probes.Where(x => x.TargetName.Equals("hyb-pos", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            PosCount = posProbe != null ? rcc.ProbeCounts[posProbe.ProbeID] : -1;
            // Get NEG count
            var negProbe = probes.Where(x => x.TargetName.Equals("hyb-neg", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            NegCount = negProbe != null ? rcc.ProbeCounts[negProbe.ProbeID] : -1;
            // Get AssayPosGeoMean
            var posProbeIDs = probes.Where(x => x.CodeClass.StartsWith("Po") && !x.TargetName.Equals("hyb-pos", StringComparison.OrdinalIgnoreCase))
                                  .Select(x => x.ProbeID);
            AssayPosGeoMean = Util.GetGeoMean(posProbeIDs.Select(x => rcc.ProbeCounts[x]).ToArray());
            // Get AssayNegGeoMean
            var negProbeIDs = probes.Where(x => x.CodeClass.StartsWith("Ne") && !x.TargetName.Equals("hyb-neg", StringComparison.OrdinalIgnoreCase))
                                    .Select(x => x.ProbeID);
            AssayNegGeoMean = Util.GetGeoMean(negProbeIDs.Select(x => rcc.ProbeCounts[x]).ToArray());
            // Get HKGeoMean
            var hkProbeIDs = probes.Where(x => x.CodeClass.StartsWith("Co"))
                                 .Select(x => x.ProbeID);
            HkGeoMean = Util.GetGeoMean(hkProbeIDs.Select(x => rcc.ProbeCounts[x]).ToArray());
            // Get endo max and min
            var endoIDs = probes.Where(x => x.CodeClass.StartsWith("En"))
                                .Select(x => x.ProbeID);
            var endoCounts = endoIDs.Select(x => rcc.ProbeCounts[x]);
            EndoMax = endoCounts.Max();
            EndoMin = endoCounts.Min();
            // Get % above threshold
            PctAboveThresh = rcc.PctAboveThresh;
        }
    }
}

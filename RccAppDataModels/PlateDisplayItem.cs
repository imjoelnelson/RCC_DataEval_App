using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    /// <summary>
    /// Item for binding plateview DGVf
    /// </summary>
    public class PlateDisplayItem
    {
        /// <summary>
        /// Column index for the well
        /// </summary>
        int ColumnIndex { get; set; }
        /// <summary>
        /// Row index for the well
        /// </summary>
        int RowIndex { get; set; }
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

        public PlateDisplayItem(int posCount, int negCount, double assayPosGeomean, double assayNegMean, double hkGeomean, int endoMax, int endoMin)
        {
            PosCount = posCount;
            NegCount = negCount;
            AssayPosGeoMean = assayPosGeomean;
            AssayNegGeoMean = AssayNegGeoMean;
            HkGeoMean = hkGeomean;
            EndoMax = endoMax;
            EndoMin = endoMin;
        }

        public PlateDisplayItem(Rcc rcc, int plexRow)
        {
            // Set indices
            ColumnIndex = rcc.LaneID - 1; // Adjusted for 1-based counting
            RowIndex = plexRow;

            // Get probe counts
        }
    }
}

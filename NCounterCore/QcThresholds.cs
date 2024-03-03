using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class QcThresholds
    {
       /// <summary>
       /// Threshold for PctFovCounted QC
       /// </summary>
        public double ImagingThreshold { get; set; }
        /// <summary>
        /// Threshold for BindingDensity QC for Sprint instruments
        /// </summary>
        public double SprintDensityThreshold { get; set; }
        /// <summary>
        /// Threshold for BindingDensity QC for DA instruments
        /// </summary>
        public double DaDensityThreshold { get; set; }
        /// <summary>
        /// Threshold for POS linearity QC
        /// </summary>
        public double LinearityThreshold { get; set; }
        /// <summary>
        /// Coefficient for standard deviation of NEG counts in calculating LOD
        /// </summary>
        public int LodSdCoefficient { get; set; }
        /// <summary>
        /// Threshold for background
        /// </summary>
        public int CountThreshold { get; set; }

        public QcThresholds() { }
        public QcThresholds(double imaging, double sprintDensity, double daDensity, double linearity, int lod, int count)
        {
            ImagingThreshold = imaging;
            SprintDensityThreshold = sprintDensity;
            DaDensityThreshold = daDensity;
            LinearityThreshold = linearity;
            LodSdCoefficient = lod;
            CountThreshold = count;
        }
    }
}

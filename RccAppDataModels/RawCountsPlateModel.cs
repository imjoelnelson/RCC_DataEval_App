using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class RawCountsPlateModel : RccAppDataModels.IRawCountsPlateModel
    {
        public string[] PlexQcPropertyList { get; set; }
        public string SelectedQcProperty { get; set; }
        public string[][] DisplayMatrix { get; set; }

        



        public RawCountsPlateModel(List<Rcc> rccs, string initialProperty)
        {

        }

        public string[][] GetSelectedQcData(string selectedProperty, List<PlateDisplayItem[]> plateItems)
        {
            for (int i = 0; i < 8; i++)
            {
                string[] row = new string[12];
                for (int j = 0; j < 12; j++)
                {
                    row[j] = 
                }
            }
        }


        #region PlexQcPropertyItem list and callbacks
        private static PlexQcPropertyItem[] AvailableQcMetrics = new PlexQcPropertyItem[]
        {
            
        };

        // Callbacks
        private static double GetPosCount(Rcc rcc, int rowIndex, bool isDsp)
        {
            if(isDsp)
            {
                var posProbe = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                      && x.Value.TargetName.Equals("hyb-pos", StringComparison.OrdinalIgnoreCase))
                                             .FirstOrDefault();
                return posProbe.Value != null ? rcc.ProbeCounts[posProbe.Value.ProbeID] : -1;
            }
            else
            {
                var posProbe = rcc.ThisRLF.Probes.Where(x => x.Value.TargetName.Equals($"POS_{char.ConvertFromUtf32(42 + rowIndex)}"))
                                             .FirstOrDefault();
                return posProbe.Value != null ? rcc.ProbeCounts[posProbe.Value.ProbeID] : -1;
            }
        }

        private static double GetNegCount(Rcc rcc, int rowIndex, bool isDsp)
        {
            if(isDsp)
            {
                var posProbe = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                      && x.Value.TargetName.Equals("hyb-neg", StringComparison.OrdinalIgnoreCase))
                                             .FirstOrDefault();
                return posProbe.Value != null ? rcc.ProbeCounts[posProbe.Value.ProbeID] : -1;
            }
            else
            {
                var posProbe = rcc.ThisRLF.Probes.Where(x => x.Value.TargetName.Equals($"NEG_{char.ConvertFromUtf32(42 + rowIndex)}"))
                                             .FirstOrDefault();
                return posProbe.Value != null ? rcc.ProbeCounts[posProbe.Value.ProbeID] : -1;
            }
        }

        private static double GetAssayPosGeoMean(Rcc rcc, int rowIndex, bool isDsp)
        {
            if(isDsp)
            {

            }
            else
            {

            }
        }
        #endregion
    }
}

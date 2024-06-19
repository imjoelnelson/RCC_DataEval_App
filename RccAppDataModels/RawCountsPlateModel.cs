using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    // TO DO: Convert to a paginated format to work with tab control in View (i.e. add a page item containing one cart's RCCs and matrices)
    public class RawCountsPlateModel : RccAppDataModels.IRawCountsPlateModel
    {
        public string[] PlexQcPropertyList { get; set; }
        private static int Threshold { get; set; }
        private List<RawCountPlateViewPageItem> PageItems { get; set; }

        public RawCountsPlateModel(List<Rcc> rccs, string initialProperty, int threshold)
        {
            PageItems = new List<RawCountPlateViewPageItem>();
            List<string> cartIdOrder = rccs.Select(x => x.CartridgeID).Distinct().ToList();
            for(int i = 0; i < cartIdOrder.Count; i++)
            {
                List<Rcc> pageRccs = rccs.Where(x => x.CartridgeID.Equals(cartIdOrder[i])).ToList();
                PageItems.Add(new RawCountPlateViewPageItem(i, pageRccs, initialProperty));
            }
            Threshold = threshold;
            if(rccs.All(x => x.ThisRLF.ThisType == RlfType.DSP))
            {
                PlexQcPropertyList = AvailableQcMetrics.Select(x => x.Name).ToArray();
            }
            else
            {
                PlexQcPropertyList = AvailableQcMetrics.Where(x => !x.IsDspOnly)
                                                                                 .Select(x => x.Name)
                                                                                 .ToArray();
            }
        }

        public string[][] GetSelectedLaneQcData(int index)
        {
            string[][] mat = new string[2][];
            mat[0] = new string[12];
            mat[1] = new string[12];
            for(int i = 0; i < 12; i++)
            {
                Rcc temp = PageItems[index].Rccs.Where(x => x.LaneID == i + 1).FirstOrDefault();
                mat[0][i] = temp != null ? temp.PctFovCounted.ToString() : "-";
                mat[1][i] = temp != null ? temp.BindingDensity.ToString() : "-";
            }

            return mat.ToArray();
        }

        public string[][] GetSelectedCellQcData(string selectedProperty, int index)
        {
            PlexQcPropertyItem item = AvailableQcMetrics
                .Where(x => x.Name == selectedProperty).First();
            string[][] retMat = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                string[] row = new string[12];
                for (int j = 0; j < 12; j++)
                {
                    Rcc temp = PageItems[index].Rccs.Where(x => x.LaneID == j + 1).FirstOrDefault();
                    string rowVal = temp != null ? item.Callback(temp, i, 
                          temp.ThisRLF.ThisType == RlfType.DSP, Threshold).ToString() : "-";
                    row[j] = !rowVal.Equals("-1") ? rowVal : "NA";
                }
                retMat[i] = row;
            }

            return retMat;
        }

        #region PlexQcPropertyItem list and callbacks
        public static PlexQcPropertyItem[] AvailableQcMetrics = new PlexQcPropertyItem[]
        {
            new PlexQcPropertyItem("Hyb POS Control Count", GetPosCount, false),
            new PlexQcPropertyItem("Hyb NEG Control Count", GetNegCount, false),
            new PlexQcPropertyItem("Assay POS Control GeoMean", GetAssayPosGeoMean, true),
            new PlexQcPropertyItem("Assay NEG Control GeoMean", GetAssayNegGeoMean, true),
            new PlexQcPropertyItem("Housekeeping Target GeoMean", GetHkGeoMean, false),
            new PlexQcPropertyItem("Endogenous Targets Max Count", GetEndoMax, false),
            new PlexQcPropertyItem("Endogenous Targets Min Count", GetEndoMin, false),
            new PlexQcPropertyItem("Percent of Targets Above Threshold", GetPctAboveThresh, false)
        };

        // Callbacks
        private static double GetPosCount(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if(rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            if (isDsp)
            {
                var posProbe = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                          && x.Value.TargetName.Equals("hyb-pos", StringComparison.OrdinalIgnoreCase))
                                                 .FirstOrDefault();
                return posProbe.Value != null ? rcc.ProbeCounts[posProbe.Value.ProbeID] : -1;
            }
            else
            {
                int retVal;
                string name = $"POS_{char.ConvertFromUtf32(42 + rowIndex)}";
                bool present = rcc.ProbeCounts.TryGetValue(name, out retVal);
                return present ? retVal : -1;
            }
        }

        private static double GetNegCount(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            if (isDsp)
            {
                var negProbe = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                          && x.Value.TargetName.Equals("hyb-neg", StringComparison.OrdinalIgnoreCase))
                                                 .FirstOrDefault();
                return negProbe.Value != null ? rcc.ProbeCounts[negProbe.Value.ProbeID] : -1;
            }
            else
            {
                int retVal;
                string name = $"NEG_{char.ConvertFromUtf32(42 + rowIndex)}";
                bool present = rcc.ProbeCounts.TryGetValue(name, out retVal);
                return present ? retVal : -1;
            }


        }

        private static double GetAssayPosGeoMean(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var posProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("P")
                                                        && !x.Value.TargetName.Equals("hyb-pos", StringComparison.OrdinalIgnoreCase));
            var posCounts = posProbes.Count() > 0 ? posProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return posCounts.Length > 0 ? Math.Round(Util.GetGeoMean(posCounts), 2) : -1;
        }

        private static double GetAssayNegGeoMean(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var negProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("N")
                                                        && !x.Value.TargetName.Equals("hyb-neg", StringComparison.OrdinalIgnoreCase));
            var negCounts = negProbes.Count() > 0 ? negProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return negCounts.Length > 0 ? Math.Round(Util.GetGeoMean(negCounts), 2) : -1;
        }

        private static double GetHkGeoMean(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            string pat;
            if (isDsp)
            {
                pat = "C";
            }
            else
            {
                pat = "H";
            }
            var hkProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                      && x.Value.CodeClass.StartsWith(pat));
            var hkCounts = hkProbes.Count() > 0 ? hkProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return hkCounts.Length > 0 ? Math.Round(Util.GetGeoMean(hkCounts), 2) : -1;
        }

        private static double GetEndoMax(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var endoProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("E"));
            var endoCounts = endoProbes.Count() > 0 ? endoProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return endoCounts.Length > 0 ? endoCounts.Max() : -1;
        }

        private static double GetEndoMin(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var endoProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("E"));
            var endoCounts = endoProbes.Count() > 0 ? endoProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return endoCounts.Length > 0 ? endoCounts.Min() : -1;
        }

        private static double GetPctAboveThresh(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var endoProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("E"));
            var endoCounts = endoProbes.Count() > 0 ? endoProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return endoCounts.Length > 0 ? Math.Round((double)(endoCounts.Where(x => x > threshold).Count()) / endoCounts.Count(), 2) : -1;
        }
        #endregion
    }
}

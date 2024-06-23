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
                PageItems.Add(new RawCountPlateViewPageItem(i, pageRccs, initialProperty, threshold));
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

        public string[][] GetSelectedLaneQcData(int pageIndex)
        {
            string[][] mat = new string[2][];
            mat[0] = new string[12];
            mat[1] = new string[12];
            for(int i = 0; i < 12; i++)
            {
                Rcc temp = PageItems[pageIndex].Rccs.Where(x => x.LaneID == i + 1).FirstOrDefault();
                mat[0][i] = temp != null ? temp.PctFovCounted.ToString() : "-";
                mat[1][i] = temp != null ? temp.BindingDensity.ToString() : "-";
            }

            return mat.ToArray();
        }

        public string[][] GetSelectedCellQcData(string selectedProperty, int pageIndex)
        {
            string[][] retMat = new string[8][];
            for (int i = 0; i < 8; i++)
            {
                string[] row = new string[12];
                for (int j = 0; j < 12; j++)
                {
                    row[j] = PageItems[pageIndex].WellData[i][j].NamedQcMetrics[selectedProperty].ToString();
                }
                retMat[i] = row;
            }

            return retMat;
        }

        public void ExportQcData(int selectedIndex)
        {
            List<string> lines = new List<string>(75);

            // Add lane QC data
            lines.Add("Lane QC Data,,1,2,3,4,5,6,7,8,9,10,11,12");
            string[][] laneData = GetSelectedLaneQcData(selectedIndex);
            lines.Add($",Pct FOV Counted,{string.Join(",", laneData[0])}");
            lines.Add($",Binding Density,{string.Join(",", laneData[1])}");

            // Add Well QC data
            foreach(PlexQcPropertyItem item in AvailableQcMetrics)
            {
                lines.Add($"{item.Name},,,,,,,,,,,,,");
                string[][] wellData = GetSelectedCellQcData(item.Name, selectedIndex);
                for(int j = 0; j < wellData.Length; j++)
                {
                    lines.Add($",{char.ConvertFromUtf32(65 + j)},{string.Join(",", wellData[j])}");
                }
            }

            string path;
            using(var sfd = new System.Windows.Forms.SaveFileDialog())
            {
                sfd.Filter = "CSV|*.csv";
                sfd.Title = "Save Qc Data as a CSV";
                sfd.RestoreDirectory = true;
                if(sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    path = sfd.FileName;
                    try
                    {
                        System.IO.File.WriteAllLines(sfd.FileName, lines.ToArray());
                    }
                    catch(Exception er)
                    {
                        System.Windows.Forms.MessageBox.Show($"{er.Message}\r\n\r\n{er.StackTrace}", "CSV Save Error", System.Windows.Forms.MessageBoxButtons.OK);
                        return;
                    }
                }
                else
                {
                    path = string.Empty;
                }
            }

            Util.OpenFileAfterSaved(path, 5000);
        }

        public double[][] GetDataForChart(int pageIndex, string selectedProperty)
        {
            double[][] retVal = new double[8][];
            for(int i = 0; i < PageItems[pageIndex].WellData.Count; i++)
            {
                retVal[i] = PageItems[pageIndex].WellData[i].Select(x => x.NamedQcMetrics[selectedProperty]).ToArray();
            }
            return retVal;
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
        public static double GetPosCount(Rcc rcc, int rowIndex, bool isDsp, int threshold)
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

        public static double GetNegCount(Rcc rcc, int rowIndex, bool isDsp, int threshold)
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

        public static double GetAssayPosGeoMean(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var posProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("P")
                                                        && !x.Value.TargetName.Equals("hyb-pos", StringComparison.OrdinalIgnoreCase)
                                                        && rcc.ProbeCounts[x.Key] > threshold);
            var posCounts = posProbes.Count() > 0 ? posProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return posCounts.Length > 0 ? Math.Round(Util.GetGeoMean(posCounts), 2) : -1;
        }

        public static double GetAssayNegGeoMean(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var negProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("N")
                                                        && !x.Value.TargetName.Equals("hyb-neg", StringComparison.OrdinalIgnoreCase)
                                                        && rcc.ProbeCounts[x.Key] > threshold);
            var negCounts = negProbes.Count() > 0 ? negProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return negCounts.Length > 0 ? Math.Round(Util.GetGeoMean(negCounts), 2) : -1;
        }

        public static double GetHkGeoMean(Rcc rcc, int rowIndex, bool isDsp, int threshold)
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
                                                      && x.Value.CodeClass.StartsWith(pat)
                                                      && rcc.ProbeCounts[x.Key] > threshold);
            var hkCounts = hkProbes.Count() > 0 ? hkProbes.Select(x => rcc.ProbeCounts[x.Key]).ToArray() : new int[0];
            return hkCounts.Length > 0 ? Math.Round(Util.GetGeoMean(hkCounts), 2) : -1;
        }

        public static double GetEndoMax(Rcc rcc, int rowIndex, bool isDsp, int threshold)
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

        public static double GetEndoMin(Rcc rcc, int rowIndex, bool isDsp, int threshold)
        {
            if (rcc.ThisRLF.Probes == null)
            {
                return -1;
            }
            var endoProbes = rcc.ThisRLF.Probes.Where(x => x.Value.PlexRow == rowIndex
                                                        && x.Value.CodeClass.StartsWith("E")
                                                        && rcc.ProbeCounts[x.Key] > threshold);
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

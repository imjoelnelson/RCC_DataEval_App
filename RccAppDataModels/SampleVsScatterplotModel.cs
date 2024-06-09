using NCounterCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class SampleVsScatterplotModel : ISampleVsScatterplotModel
    {
        public BindingList<ScatterSelectItem> Samples { get; set; }
        private List<string> Content { get; set; }
        public SampleVsScatterplotModel(List<Rcc> rccs)
        {
            var temp = rccs.Select(x => new ScatterSelectItem(x)).ToList();
            Samples = new BindingList<ScatterSelectItem>(temp);
            // Set 1st and 2nd samples as those to be initially displayed
            Samples[0].X = true;
            Samples.Where(x => x.Name != Samples[0].Name).ToList().ForEach(x => x.X = false);
            Samples[1].Y = true;
            Samples.Where(x => x.Name != Samples[1].Name).ToList().ForEach(x => x.Y = false);
            // While currently this only accepts RCCs from a single RLF this function is anticipation of accepting cross-RLF
            IEnumerable<Rlf> rlfs = rccs.Select(x => x.ThisRLF).Distinct(new RlfEqualityComparer());
            Content = rlfs.SelectMany(x => x.Probes.Where(y => !y.Value.CodeClass.StartsWith("Pos")
                                                                    && !y.Value.CodeClass.StartsWith("Neg")
                                                                    && !y.Value.CodeClass.StartsWith("Lig"))
                                                   .Select(y => y.Key))
                          .Distinct()
                          .ToList();
            if (Content.Count >= 3)
            {
                return;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Fewer than 3 genes overlap between included RLFs.", "Insufficient Content",
                    System.Windows.Forms.MessageBoxButtons.OK);
                Content = null;
                return;
            }
        }

        public void UpdateSelection(int row, int col)
        {
            if (col == 1)
            {
                for (int i = 0; i < Samples.Count; i++)
                {
                    if (i == row)
                    {
                        Samples[i].X = true;
                    }
                    else
                    {
                        Samples[i].X = false;
                    }
                }
            }

            if (col == 2)
            {
                for (int i = 0; i < Samples.Count; i++)
                {
                    if (i == row)
                    {
                        Samples[i].Y = true;
                    }
                    else
                    {
                        Samples[i].Y = false;
                    }
                }
            }
        }

        public ScatterChartDto GetChartData(bool thresholded, int threshold)
        {
            Rcc x = Samples.Where(z => z.X).Select(z => z.ThisRcc).FirstOrDefault();
            Rcc y = Samples.Where(z => z.Y).Select(z => z.ThisRcc).FirstOrDefault();

            if(x == null || y == null)
            {
                return null;
            }

            int threshToUse;
            if(thresholded)
            {
                threshToUse = threshold;
            }
            else
            {
                threshToUse = 0;
            }

            // Get thresholded counts
            var retainedContent = Content.Where(z => x.ProbeCounts[z] > threshToUse && y.ProbeCounts[z] > threshToUse).ToArray();

            if (retainedContent.Length < 3)
            {
                System.Windows.Forms.MessageBox.Show("Fewer than three targets remain after thresholding so correlation cannot be processed. Try again after lowering or turning off the count threshold");
                return null;
            }
            
            double[] xVals = retainedContent.Select(z => Util.GetLog2((double)x.ProbeCounts[z])).ToArray();
            double[] yVals = retainedContent.Select(z => Util.GetLog2((double)y.ProbeCounts[z])).ToArray();

            var linReg = MathNet.Numerics.LinearRegression.SimpleRegression.Fit(xVals.ToArray(), yVals.ToArray());
            Tuple<double, double> regLine = Tuple.Create(linReg.A, linReg.B);
            double rSquared = MathNet.Numerics.GoodnessOfFit.RSquared(xVals.Select(z => regLine.Item1 + (regLine.Item2 * z)), yVals);


            return new ScatterChartDto(retainedContent, xVals, yVals, x.FileName, y.FileName, regLine, rSquared);
        }
    }
}

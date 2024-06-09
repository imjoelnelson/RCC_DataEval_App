using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface ISampleVsScatterplotView
    {
        bool UseCutoff { get; set; }
        int Threshold { get; set; }
        System.Windows.Forms.BindingSource Source { get; set; }
        
        event EventHandler UseCutoffChanged;
        event EventHandler ThresholdChanged;
        event EventHandler<ScatterPlotSelectArgs> DgvCellContentClick;

        void SetCorrelationChart(string[] targetNames, double[] xVals, double[] yVals,
            string xName, string yName, Tuple<double, double> regLine, double rSquared);
        void ShowThisDialog();
    }
}

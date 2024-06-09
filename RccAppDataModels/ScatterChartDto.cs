using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class ScatterChartDto
    {
        public string[] TargetNames { get; set; }
        public double[] XVals { get; set; }
        public double[] YVals { get; set; }
        public string XName { get; set; }
        public string YName { get; set; } 
        public Tuple<double, double> RegLine { get; set; }
        public double RSquared { get; set; }

        public ScatterChartDto(string[] targetNames, double[] xVals, double[] yVals, string xName, string yName,
            Tuple<double, double> regLine, double rSquared)
        {
            TargetNames = targetNames;
            XVals = xVals;
            YVals = yVals;
            XName = xName;
            YName = yName;
            RegLine = regLine;
            RSquared = rSquared;
        }
    }
}

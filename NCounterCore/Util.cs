using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NCounterCore
{
    public static class Util
    {
        /// <summary>
        /// Method for getting values from key,value pairs in RCC SampleAttributes and LaneAttributes sections
        /// </summary>
        /// <param name="input">Input line</param>
        /// <returns>attribute value</returns>
        public static string GetValue(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            string[] bits = input.Split(',');
            return bits.Length > 1 ? bits[1] : string.Empty;
        }

        /// <summary>
        /// Wraps int.TryParse into a single line
        /// </summary>
        /// <param name="input">string to parse</param>
        /// <returns>parsed int</returns>
        public static int SafeParseInt(string input)
        {
            int i;
            bool parsed = int.TryParse(input, out i);
            return parsed ? i : -1;
        }

        /// <summary>
        /// Wraps double.TryParse into a single line
        /// </summary>
        /// <param name="input">string to parse</param>
        /// <returns>parsed double</returns>
        public static double SafeParseDouble(string input)
        {
            double d;
            bool parsed = double.TryParse(input, out d);
            return parsed ? d : -1.0;
        }

        /// <summary>
        /// Calculate geometric mean
        /// </summary>
        /// <param name="input">int array to calculate geomean from</param>
        /// <returns>The geomean as a double</returns>
        public static double GetGeoMean(int[] input)
        {
            IEnumerable<double> dubs = input.Select(x => x > 0 ? Math.Log(x, 2) : 0.0);
            return Math.Pow(2, dubs.Sum() / dubs.Count());
        }

        public static double GetGeoMean(double[] input)
        {
            IEnumerable<double> dubs = input.Select(x => x > 0 ? Math.Log(x, 2) : 0.0);
            return Math.Pow(2, dubs.Sum() / dubs.Count());
        }
    }
}

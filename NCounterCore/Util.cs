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

        /// <summary>
        /// Converts values from a dgv with textbox cells and checkbox cells to a list of strings, then saves as a csv
        /// </summary>
        /// <param name="dgv">The DataGridView to be saved</param>
        public static string[] ConvertDgvToStrings(DataGridView dgv)
        {
            if(dgv == null)
            {
                return null;
            }
            if(dgv.RowCount < 1)
            {
                return null;
            }

            List<string> collector = new List<string>(dgv.Rows.Count + 1);
            // Add column headers
            List<string> headers = new List<string>(dgv.ColumnCount);
            for (int i = 0; i < dgv.ColumnCount; i++)
            {
                headers.Add(dgv.Columns[i].HeaderText);
            }
            collector.Add(string.Join(",", headers));
            // Add rows
            for (int i = 0; i < dgv.RowCount; i++)
            {
                List<string> cells = new List<string>(dgv.ColumnCount);
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    DataGridViewCell cell = dgv.Rows[i].Cells[j];
                    if (cell.GetType() == typeof(DataGridViewTextBoxCell))
                    {
                        cells.Add(cell.Value.ToString());
                    }
                    else if (cell.GetType() == typeof(DataGridViewCheckBoxCell))
                    {
                        bool val = (bool)cell.Value;
                        if (!val)
                        {
                            cells.Add(string.Empty);
                        }
                        else
                        {
                            cells.Add("<<FLAG>>");
                        }
                    }
                }
                collector.Add(string.Join(",", cells));
            }
            return collector.ToArray();
        }
    }
}

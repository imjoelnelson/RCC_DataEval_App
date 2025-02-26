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
        /// Returns log2 transformation of a double
        /// </summary>
        /// <param name="input">the double to transform</param>
        /// <returns>Double representing log2 transformation of input</returns>
        public static double GetLog2(double input)
        {
            return input > 0 ? Math.Log(input, 2) : 0;
        }

        /// <summary>
        /// Converts array of string[] which represent columns to array of string[] which represent rows
        /// </summary>
        /// <param name="lines">Array of string[] which represent columns of the table</param>
        /// <returns>Array of string[] which represent rows of the table</returns>
        public static string[][] TransformTable(string[][] lines)
        {
            if (lines.Any(x => x.Length != lines[0].Length))
            {
                throw new Exception("TransformTable cannot operate on jagged array");
            }
            List<List<string>> transformed = new List<List<string>>(lines[0].Length);
            for (int r = 0; r < lines[0].Length; r++)
            {
                List<string> temp = new List<string>(lines.Length);
                for (int c = 0; c < lines.Length; c++)
                {
                    temp.Add(lines[c][r]);
                }
                transformed.Add(temp);
            }
            return transformed.Select(x => x.ToArray()).ToArray();
        }

        /// <summary>
        /// Converts array of double[] which represent one dimension of a table to an array of double[] that represent the other dimension (i.e. rows to columns or vice versa
        /// </summary>
        /// <param name="input">Array of double[] which represent one dimension of the table</param>
        /// <returns>Array of double[] which represent the other dimension of the table</returns>
        public static double[][] TransformTable(double[][] input)
        {
            if (input.Any(x => x.Length != input[0].Length))
            {
                throw new Exception("TransformTable cannot operate on jagged array");
            }

            List<double[]> retVal = new List<double[]>(input.Length);
            for(int r = 0; r < input[0].Length; r++)
            {
                List<double> temp = new List<double>();
                for(int c = 0; c < input.Length; c++)
                {
                    temp.Add(input[c][r]);
                }
                retVal.Add(temp.ToArray());
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Opens the saved CSV after being saved by SaveTable(string[][])
        /// </summary>
        /// <param name="path">Path in temp folder where the file is saved</param>
        /// <param name="maxDelay">Amount of time to check to see if file has been saved at path before giving up</param>
        public static void OpenFileAfterSaved(string path, int maxDelay)
        {
            // Ask if user wants to open file
            string message = $"Would you like to open {Path.GetFileName(path)} now?";
            string cap = "File Saved";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result = MessageBox.Show(message, cap, buttons);
            // Open file if 'yes'
            if (result == DialogResult.Yes)
            {
                int sleepAmount = 1000; // Delay to give time for file to be saved before trying to open
                int sleepStart = 0;
                int maxSleep = maxDelay;
                while (true)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(path);
                        break;
                    }
                    catch (Exception er)
                    {
                        if (sleepStart <= maxSleep)
                        {
                            System.Threading.Thread.Sleep(sleepAmount);
                            sleepStart += sleepAmount;
                        }
                        else
                        {
                            MessageBox.Show($"The file could not be opened due to an exception:\r\n\r\n{er.Message}\r\n\r\n{er.StackTrace}",
                                            "File Open Error",
                                            MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
            }
        }
    }
}

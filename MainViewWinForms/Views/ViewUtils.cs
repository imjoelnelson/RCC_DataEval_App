﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms.Views
{
    public static class ViewUtils
    {
        /// <summary>
        /// Converts values from a dgv with textbox cells and checkbox cells to a list of strings, then saves as a csv
        /// </summary>
        /// <param name="dgv">The DataGridView to be saved</param>
        public static string[] ConvertDgvToStrings(DataGridView dgv)
        {
            if (dgv == null)
            {
                return null;
            }
            if (dgv.RowCount < 1)
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

        /// <summary>
        /// Opens the saved CSV after being saved by SaveTable(string[][])
        /// </summary>
        /// <param name="path">Path in temp folder where the file is saved</param>
        /// <param name="maxDelay">Amount of time to check to see if file has been saved at path before giving up</param>
        public static void OpenFileAfterSaved(string path, int maxDelay)
        {
            // Ask if user wants to open file
            string message = $"Would you like to open {System.IO.Path.GetFileName(path)} now?";
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

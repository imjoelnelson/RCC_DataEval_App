using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class PlexQcPropertyItem
    {
        /// <summary>
        /// Placeholder for method for getting the QC value in Name
        /// </summary>
        /// <param name="input">Counts the QC value will be calculated from</param>
        /// <returns>The named QC value</returns>
        public delegate double GetValue(Rcc rcc, int rowIndex, bool isDsp);
        /// <summary>
        /// Name of the QC value; to be used in combo box for selecting QC value to display
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Method for calculating the QC value from the counts
        /// </summary>
        public GetValue Callback { get; set; }
        /// <summary>
        /// For filtering property items to include in combobox based on whether RCCs are from DSP readout or PlexSet
        /// </summary>
        public bool IsDspOnly { get; set; }

        public PlexQcPropertyItem(string name, GetValue callback, bool isDspOnly)
        {
            Name = name;
            Callback = callback;
            IsDspOnly = isDspOnly;
        }
    }
}

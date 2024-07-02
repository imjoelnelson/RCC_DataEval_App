using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    /// <summary>
    /// Class for defining a group of RCCs intended to be analyzed together, and thus normalized as a group. Includes norm factors. May add enum for method used if multiple are available
    /// </summary>
    public class NormalizationGroup
    {
        /// <summary>
        /// RCCs for the samples to be normalized together
        /// </summary>
        List<Rcc> Rccs { get; set; }
        /// <summary>
        /// RLF for all of the included RCCs; MAY NOT NEED
        /// </summary>
        Rlf ThisRlf { get; set; }
        /// <summary>
        /// Calculated Norm factors as a dictionary with the filename of each sample they're associated with (i.e. Dictionary of {FileName, factor} )
        /// </summary>
        Dictionary<string, double> NormFactors { get; set; }
    }
}

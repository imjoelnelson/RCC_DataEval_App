using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    /// <summary>
    /// Produces a raw count table for RCC collections of all types (either single RLF or cross RLF, as long as RCCs aren't PlexSet or DSP)
    /// </summary>
    public class RawProbeCountsTable
    {
        /// <summary>
        /// The table lines 
        /// </summary>
        public string[][] TableLines { get; set; }
        
        private static string[] RawTableProbeOrder = new string[] { "Positive", "Negative", "Ligation", "Housekeeping", "Invariant", "SpikeIn", "Control",
            "Endogenous", "Endogenous1", "Endogenous2", string.Empty, "NA" };

        /// <summary>
        /// Constructor for single RLF raw counts table
        /// </summary>
        /// <param name="rccs">List of RCCs included in the table</param>
        /// <param name="rlf">The RLF used for all the RCCs</param>
        /// <param name="properties">RCC properties to include in the header section of the table (from user prefs)</param>
        public RawProbeCountsTable(List<Rcc> rccs, Rlf rlf, string[] properties)
        {
            List<string[]> collector = new List<string[]>(rccs.Count + 2);

            // Get ordered probes with included codeclasses
            List<string> orderedProbesToKeep = new List<string>(rlf.Probes.Count);
            List<string> includedCodeClasses = rlf.Probes.Select(x => x.Value.CodeClass).Distinct().ToList();
            for (int i = 0; i < RawTableProbeOrder.Length; i++)
            {
                if (includedCodeClasses.Contains(RawTableProbeOrder[i]))
                {
                    orderedProbesToKeep.AddRange(rlf.Probes.Where(x => x.Value.CodeClass.Equals(RawTableProbeOrder[i]))
                                                       .Select(x => x.Key)
                                                       .OrderBy(y => y));
                }
            }

            // List capacity for column building
            int cap = properties.Length + 7 + orderedProbesToKeep.Count;

            // ** First column
            List<string> firstCol = new List<string>(cap);
            // Add included properties names to first column
            firstCol.AddRange(properties);
            // Add Flag names to first column
            firstCol.AddRange(new string[] { "--------------------", "Imaging QC Flag", "Density QC Flag", "Linearity QC Flag", 
                "LOD QC Flag", "--------------------", "Probe Name" });
            // Add probes to first column and add to collector
            firstCol.AddRange(orderedProbesToKeep);
            collector.Add(firstCol.ToArray());

            // ** CodeClass column
            List<string> secondCol = new List<string>(cap);
            secondCol.AddRange(Enumerable.Repeat(string.Empty, properties.Length));
            secondCol.Add("--------------------");
            secondCol.AddRange(Enumerable.Repeat(string.Empty, 4));
            secondCol.AddRange(new string[] { "--------------------", "CodeClass" });
            secondCol.AddRange(orderedProbesToKeep.Select(x => rlf.Probes[x].CodeClass));
            collector.Add(secondCol.ToArray());

            // ** Add RCC columns
            collector.AddRange(rccs.Select(x => GetRccColumn(x, properties, orderedProbesToKeep)));
            // Make comma separated lines
            TableLines = collector.ToArray();
        }
        /// <summary>
        /// Constructor for multi-RLF raw counts table
        /// </summary>
        /// <param name="rccs">List of RCCs included in the table</param>
        /// <param name="rlfs">List of RLFs used by all included RCCs</param>
        /// <param name="properties">RCC properties to include in the header section of the table (from user prefs)</param>
        public RawProbeCountsTable(List<Rcc> rccs, List<Rlf> rlfs, string[] properties)
        {
            List<string[]> collector = new List<string[]>(rccs.Count + 2);

            // Get overlapping probes
            List<List<string>> listOfLists = rlfs.Select(x => x.Probes.Select(y => y.Value.ProbeID).ToList()).ToList();
            List<string> intersection = listOfLists.Skip(1).Aggregate(
                new HashSet<string>(listOfLists.First()), (h, e) => { h.IntersectWith(e); return h; }).ToList();

            // Get ordered probes with included codeclasses
            List<ProbeItem> orderedProbesToKeep = new List<ProbeItem>(intersection.Count);
            List<string> includedCodeClasses = rlfs[0].Probes.Where(x => intersection.Contains(x.Value.ProbeID))
                                                         .Select(x => x.Value.CodeClass)
                                                         .Distinct().ToList();
            for (int i = 0; i < RawTableProbeOrder.Length; i++)
            {
                if (includedCodeClasses.Contains(RawTableProbeOrder[i]))
                {
                    orderedProbesToKeep.AddRange(intersection.Select(x => rlfs[0].Probes[x])
                                                             .Where(x => x.CodeClass.Equals(RawTableProbeOrder[i]))
                                                             .OrderBy(y => y.TargetName));
                }
            }

            // List capacity for column building
            int cap = properties.Length + 7 + orderedProbesToKeep.Count;

            // ** First column
            List<string> firstCol = new List<string>(cap);
            // Add included properties names to first column
            firstCol.AddRange(properties);
            // Add Flag names to first column
            firstCol.AddRange(new string[] { "--------------------", "Imaging QC Flag", "Density QC Flag", "Linearity QC Flag", 
                "LOD QC Flag", "--------------------", "Probe Name" });
            // Add probes to first column and add to collector
            firstCol.AddRange(orderedProbesToKeep.Select(x => x.TargetName));
            collector.Add(firstCol.ToArray());

            // ** CodeClass column
            List<string> secondCol = new List<string>(cap);
            secondCol.AddRange(Enumerable.Repeat(string.Empty, properties.Length));
            secondCol.Add("--------------------");
            secondCol.AddRange(Enumerable.Repeat(string.Empty, 4));
            secondCol.AddRange(new string[] { "--------------------", "CodeClass" });
            secondCol.AddRange(orderedProbesToKeep.Select(x => x.CodeClass));
            collector.Add(secondCol.ToArray());

            // ** Add RCC columns
            collector.AddRange(rccs.Select(x => GetRccColumn(x, properties, orderedProbesToKeep.Select(y => y.TargetName).ToList())).ToArray());
            // Save restult as TableLines
            TableLines = collector.ToArray();
        }

        /// <summary>
        /// Constructor for sample-multiplexed raw count table
        /// </summary>
        /// <param name="rccs">List of RCCs included in the table</param>
        /// <param name="rlf">RLF object from either PlexSet RLF or based on PKC readers for DSP</param>
        /// <param name="properties">RCC properties to include in the header section of the table (from user prefs)</param>
        /// <param name="IsDsp">Boolean indicating if the RCCs are PlexSet or from DSP readout</param>
        public RawProbeCountsTable(List<Rcc> rccs, Rlf rlf, string[] properties, bool IsDsp)
        {
            List<string[]> collector = new List<string[]>(rccs.Count + 2);

            if (IsDsp) // For GeoMx protein
            {
                // Get ordered probes with included codeclasses
                List<ProbeItem> orderedProbesToKeep = new List<ProbeItem>(rlf.Probes.Count);
                // Get codeclasses represented in the RCCs
                List<string> includedCodeClasses = rlf.Probes.Select(x => x.Value.CodeClass).Distinct().ToList();
                // For iterating through PlexRows
                for(int j = 0; j < lets.Length; j++) // Order first by PlexRow
                {
                    for (int i = 0; i < RawTableProbeOrder.Length; i++) // Then by CodeClass
                    {
                        if (includedCodeClasses.Contains(RawTableProbeOrder[i]))
                        {
                            var retainedProbes = rlf.Probes.Where(x => x.Value.CodeClass.Equals(RawTableProbeOrder[i]) &&
                                                                       x.Value.PlexRow.Equals(j));
                            if (retainedProbes.Count() > 0)
                            {
                                orderedProbesToKeep.AddRange(retainedProbes.Select(x => x.Value)
                                                                           .OrderBy(y => y.TargetName));  // Then finally order by target name
                            }
                        }
                    }
                }

                // List capacity for column building
                int cap = properties.Length + 7 + orderedProbesToKeep.Count;

                // ** First column
                List<string> firstCol = new List<string>(cap);
                // Add included properties names to first column
                firstCol.AddRange(properties);
                // Add Flag names to first column
                firstCol.AddRange(new string[] { "--------------------", "Imaging QC Flag", "Density QC Flag", "Linearity QC Flag",
                "LOD QC Flag", "--------------------", "Probe Name" });
                // Add probes to first column and add to collector
                firstCol.AddRange(orderedProbesToKeep.Select(x => x.TargetName));
                collector.Add(firstCol.ToArray());

                // ** CodeClass column
                List<string> secondCol = new List<string>(cap);
                secondCol.AddRange(Enumerable.Repeat(string.Empty, properties.Length));
                secondCol.Add("--------------------");
                secondCol.AddRange(Enumerable.Repeat(string.Empty, 4));
                secondCol.AddRange(new string[] { "--------------------", "CodeClass" });
                secondCol.AddRange(orderedProbesToKeep.Select(x => x.CodeClass));
                collector.Add(secondCol.ToArray());

                // ** Well column
                List<string> thirdCol = new List<string>(cap);
                thirdCol.AddRange(Enumerable.Repeat(string.Empty, properties.Length));
                thirdCol.Add("--------------------");
                thirdCol.AddRange(Enumerable.Repeat(string.Empty, 4));
                thirdCol.AddRange(new string[] { "--------------------", "Plate Row" });
                thirdCol.AddRange(orderedProbesToKeep.Select(x => char.ConvertFromUtf32(x.PlexRow + 41)));
                collector.Add(thirdCol.ToArray());

                // ** Add RCC columns
                collector.AddRange(rccs.Select(x => GetRccColumn(x, properties, orderedProbesToKeep.Select(y => y.ProbeID).ToList())));
                // Make comma separated lines
                TableLines = collector.ToArray();
            }
            else // For PlexSet
            {
                // Get ordered probes with included codeclasses
                List<ProbeItem> orderedProbesToKeep = new List<ProbeItem>(rlf.Probes.Count);
                string[] codeClassOrder = new string[] { "P", "N", "H", "E" };
                for (int i = 1; i < 9; i++)
                {
                    for(int j = 0; j < codeClassOrder.Length; j++)
                    {
                        orderedProbesToKeep.AddRange(rlf.Probes.Where(x => x.Value.CodeClass.EndsWith($"{i}s") && x.Value.CodeClass.StartsWith(codeClassOrder[j]))
                                                               .Select(x => x.Value)
                                                               .OrderBy(y => y.TargetName));
                    }
                }

                // List capacity for column building
                int cap = properties.Length + 7 + orderedProbesToKeep.Count;

                // ** First column
                List<string> firstCol = new List<string>(cap);
                // Add included properties names to first column
                firstCol.AddRange(properties);
                // Add Flag names to first column
                firstCol.AddRange(new string[] { "--------------------", "Imaging QC Flag", "Density QC Flag", "Linearity QC Flag",
                    "LOD QC Flag", "--------------------", "Probe Name" });
                firstCol.AddRange(orderedProbesToKeep.Select(x => x.TargetName.Substring(0, x.TargetName.IndexOf('_'))));
                collector.Add(firstCol.ToArray());

                // ** Codeclass column
                List<string> secondCol = new List<string>(cap);
                secondCol.AddRange(Enumerable.Repeat(string.Empty, properties.Length));
                secondCol.Add("--------------------");
                secondCol.AddRange(Enumerable.Repeat(string.Empty, 4));
                secondCol.AddRange(new string[] { "--------------------", "CodeClass" });
                secondCol.AddRange(orderedProbesToKeep.Select(x => x.CodeClass.Substring(0, x.CodeClass.Length - 2)));
                collector.Add(secondCol.ToArray());

                // ** Well column
                List<string> thirdCol = new List<string>(cap);
                secondCol.AddRange(Enumerable.Repeat(string.Empty, properties.Length));
                secondCol.Add("--------------------");
                secondCol.AddRange(Enumerable.Repeat(string.Empty, 4));
                secondCol.AddRange(new string[] { "--------------------", "Plate Row" });
                secondCol.AddRange(orderedProbesToKeep.Select(x => x.PlexRow));
                collector.Add(thirdCol.ToArray());

                // Add RCC columns
                List<string> probeNames = orderedProbesToKeep.Select(y => y.TargetName).ToList();
                collector.AddRange(rccs.Select(x => GetRccColumn(x, properties, probeNames)));
                // Make comma separated lines
                TableLines = collector.ToArray();
            }
        }

        /// <summary>
        /// Provides a column for a raw counts table given RCC properties and probes to include
        /// </summary>
        /// <param name="rcc">The RCC the column is for</param>
        /// <param name="properties">RCC properties to include in the header section</param>
        /// <param name="probes">Selected probes to be included in the table</param>
        /// <returns>Single column from RCC raw count table as string[]</returns>
        public string[] GetRccColumn(Rcc rcc, string[] properties, List<string> probes)
        {
            // Collector with length for header, flags, 2 separator lines, and probes
            List<string> collector = new List<string>(properties.Length + probes.Count + 7);
            
            // Add header row info
            for (int i = 0; i < properties.Length; i++)
            {
                bool gotValue = TryGetPropertyValue(rcc, properties[i], out string val);
                collector.Add(gotValue ? val : string.Empty);
            }
            
            // Add separator
            collector.Add("--------------------");
            
            // - Add flags
            //      * Add imaging flag
            collector.Add(rcc.PctFovPass ? string.Empty : "<<FLAG>>");
            //      * Add density flag
            collector.Add(rcc.BindingDensityPass ? string.Empty : "<<FLAG>>");
            //      * Add POS linearity flag
            collector.Add(rcc.PosLinearityPass ? string.Empty : "<<FLAG>>");
            //      * Add LOD flag
            collector.Add(rcc.LodPass ? string.Empty : "<<FLAG>>");
            
            // Add separator
            collector.Add("--------------------");
            collector.Add(string.Empty); // White space compensation for annot column headers

            // Add probe counts
            for(int i = 0; i < probes.Count; i++)
            {
                collector.Add(rcc.ProbeCounts[probes[i]].ToString());
            }

            return collector.ToArray();
        }

        /// <summary>
        /// Gets the value of an RCC property by string name
        /// </summary>
        /// <param name="rcc">The RCC the property value will come from</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="value">out: value of the property via its ToString method</param>
        /// <returns>Bool indicating if non-null property value was returned</returns>
        public static bool TryGetPropertyValue(Rcc rcc, string propertyName, out string value)
        {
            value = default;
            if (rcc is null)
            {
                return false;
            }
            PropertyInfo propertyInfo = typeof(Rcc).GetProperty(propertyName);
            if (propertyInfo is null)
            {
                return false;
            }
            string propertyValue = propertyInfo.GetValue(rcc).ToString();

            if (propertyValue != null)
            {
                value = propertyValue;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

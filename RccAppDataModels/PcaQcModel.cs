using NCounterCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class PcaQcModel : IPcaQcModel
    {
        /// <summary>
        /// Collection of samples and their values for the first 4 PCs
        /// </summary>
        public Tuple<string, double[]>[] PcaMatrix { get; set; }
        /// <summary>
        /// Collection of genes and their loadings for each of the first four PCs
        /// </summary>
        public Tuple<string, double[]>[] GeneLoadings { get; set; }
        public PcaQcModel(List<Rcc> rccs)
        {
            List<string> normProbes;
            if (rccs.Select(x => x.RlfName).Distinct().Count() == 1 && rccs[0].ThisRLF.Probes.Any(x => x.Value.CodeClass.StartsWith("Ho") || x.Value.CodeClass.StartsWith("In")))
            {
                normProbes = rccs[0].ThisRLF.Probes.Where(x => x.Value.CodeClass.StartsWith("Ho") || x.Value.CodeClass.StartsWith("In"))
                                                   .Select(x => x.Key)
                                                   .ToList();
            }
            else
            {
                // PLACEHOLDER FOR MULTIRLF OR NO ANNOTATED HKS
                normProbes = null;
            }
            NormalizationGroup normGroup = new NormalizationGroup(rccs, normProbes, "GeNorm_GeoMean");
            //TESTING
            var facts = normGroup.NormFactors.Select(x => $"{rccs.Where(y => y.ID == x.Key).First().FileName},{x.Value}").ToList();
            facts.Add("RefGenesUsed,");
            facts.AddRange(normGroup.RefGenesUsed.Select(x => $"{x},"));
            System.IO.File.WriteAllLines("C:\\Users\\Joel Nelson\\Desktop\\normFactors.csv", facts);
            // TESTING
            PcaForQc pcaObject = new PcaForQc(normGroup.NormTable.ToArray(), rccs);

            string stahp = "STAHP!";
        }
    }
}

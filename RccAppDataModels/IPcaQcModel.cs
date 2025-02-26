using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface IPcaQcModel
    {
        Tuple<string, double[]>[] PcaMatrix { get; set; }
        Tuple<string, double[]>[] GeneLoadings { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public interface ISampleData
    {
        int ID { get; set; }
        List<KeyValuePair<string, object>> AnnotationInfo { get; set; }
        List<Tuple<string, double, bool>> QcInfo { get; set; }
        string ErrorMessage { get; set; }

        int? GetIntAnnot(string key);
        double? GetDoubleAnnot(string key);
        string GetStringAnnot(string key);
    }
}

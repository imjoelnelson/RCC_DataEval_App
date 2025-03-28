using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public interface IProbeInfo
    {
        int MainKeyID { get; set; }
        List<KeyValuePair<string, object>> AnnotationInfo { get; set; }
        string ErrorMessage { get; set; }

        string GetStringAnnot(string key);
        int? GetIntAnnot(string key);
        double? GetDoubleAnnot(string key);
    }
}

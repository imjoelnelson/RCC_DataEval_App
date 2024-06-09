using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class RlfEqualityComparer : EqualityComparer<Rlf>
    {
        public override bool Equals(Rlf rlf1, Rlf rlf2)
        {
            if(ReferenceEquals(rlf1, rlf2))
            {
                return true;
            }
            if(rlf1 == null || rlf2 == null)
            {
                return false;
            }
            return rlf1.Name.Equals(rlf2.Name) &&
                   rlf1.Probes.Count() == rlf2.Probes.Count();
        }

        public override int GetHashCode(Rlf rlf) => Enumerable.Range(0, rlf.Name.Length).Select(x => char.ConvertToUtf32(rlf.Name, x)).Sum() + rlf.Probes.Count;
    }
}

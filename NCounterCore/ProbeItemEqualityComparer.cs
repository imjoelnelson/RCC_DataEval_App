using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class ProbeItemEqualityComparer : EqualityComparer<ProbeItem>
    {
        public override bool Equals(ProbeItem probe1, ProbeItem probe2)
        {
            if(ReferenceEquals(probe1, probe2))
            {
                return true;
            }
            if(probe1 == null || probe2 == null)
            {
                return false;
            }

            return probe1.ProbeID.Equals(probe2.ProbeID);
        }

        public override int GetHashCode(ProbeItem probe) => Enumerable.Range(0, probe.ProbeID.Length).Select(x => char.ConvertToUtf32(probe.ProbeID, x)).Sum();
    }
}

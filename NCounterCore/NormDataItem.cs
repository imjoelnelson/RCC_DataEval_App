using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NCounterCore
{
    public class NormDataItem
    {
        public int Sample { get; set; }
        public int Target { get; set; }
        public double NormalizedCount { get; set; }
        public double RawCount { get; set; }

        public NormDataItem(int samp, int targ, double norm, double raw)
        {
            Sample = samp;
            Target = targ;
            NormalizedCount = norm;
            RawCount = raw;
        }

        public NormDataItem(string input)
        {
            string[] fields = input.Split(',');
            if(fields.Length != 4)
            {
                throw new Exception($"Normalized data read error: Cannot parse line, '{input}'");
            }
            int one;
            int two;
            int three;
            int four;
            bool isSamp = int.TryParse(fields[0], out one);
            bool isTarg = int.TryParse(fields[1], out two);
            bool isNorm = int.TryParse(fields[2], out three);
            bool isRaw = int.TryParse(fields[3], out four);
            Sample = isSamp ? one : -1;
            Target = isTarg ? two : -1;
            NormalizedCount = isNorm ? three : -1;
            RawCount = isRaw ? four : -1;
        }

        public override string ToString()
        {
            return $"{this.Sample},{this.Target},{this.NormalizedCount},{this.RawCount}";
        }
    }
}

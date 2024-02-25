 using System.Collections.Generic;

namespace NCounterCore
{
    public interface IPkcReader
    {
        Dictionary<string, ProbeItem> DspTranslator { get; }
        string Name { get; }
    }
}

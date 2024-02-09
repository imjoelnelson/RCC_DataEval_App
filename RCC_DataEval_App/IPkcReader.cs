using System.Collections.Generic;

namespace RCC_DataEval_App
{
    public interface IPkcReader
    {
        Dictionary<string, ProbeItem> DspTranslator { get; }
        string Name { get; }
    }
}
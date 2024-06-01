using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface IRawCountsPlateModel
    {
        string[] PlexQcPropertyList { get; set; }
        string SelectedQcProperty { get; set; }
        string[][] DisplayMatrix { get; set; }
    }
}

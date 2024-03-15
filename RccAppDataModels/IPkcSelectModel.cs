using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public interface IPkcSelectModel
    {
        Dictionary<string, string> SavedPkcList { get; set; }

        Dictionary<string, string> GetSavedPkcsFromFile();
    }
}

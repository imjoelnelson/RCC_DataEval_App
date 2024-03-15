using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RccAppDataModels
{
    public class PkcSelectModel : IPkcSelectModel
    {
        public Dictionary<string, string> SavedPkcList { get; set; }

        public Dictionary<string, string> GetSavedPkcsFromFile()
        {
            // Do I check for App directory in Roaming here or should that be a utility for models
        }
    }
}

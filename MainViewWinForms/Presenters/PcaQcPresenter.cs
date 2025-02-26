using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Presenters
{
    public class PcaQcPresenter
    {
        private Views.IPcaQcView View { get; set; }
        private RccAppDataModels.IPcaQcModel Model { get; set; }

        public PcaQcPresenter(Views.IPcaQcView view, RccAppDataModels.IPcaQcModel model)
        {
            View = view;
            Model = model;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Presenters
{
    public class SelectHKsPresenter
    {
        private Views.ISelectHKsView View { get; set; }
        private RccAppDataModels.ISelectHKsModel Model { get; set; }

        public SelectHKsPresenter(Views.ISelectHKsView view, RccAppDataModels.ISelectHKsModel model)
        {
            View = view;
            Model = model;

            view.UpdateChart(Model.HousekeeperList.Select(x => Tuple.Create(x.Item2, x.Item3)).ToArray());
        }
    }
}

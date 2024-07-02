using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Presenters
{
    public class BinnedCountsPresenter
    {
        private Views.IBinnedCountsView View { get; set; }
        private RccAppDataModels.IBinnedCountsModel Model { get; set; }

        public BinnedCountsPresenter(Views.IBinnedCountsView view, RccAppDataModels.IBinnedCountsModel model)
        {
            View = view;
            Model = model;

            View.ComboBoxIndexChanged += new EventHandler<Views.SelectedPlateViewEventArgs>(View_ComboboxSelectedIndexChanged);
            if(Model.CountProportionsMatrix != null && Model.BinNames != null && Model.Rccs != null)
            {
                View.SetChart(Model.CountProportionsMatrix, Model.BinNames, Model.Rccs.Select(x => x.FileName).ToArray());
            }
        }

        private void View_ComboboxSelectedIndexChanged(object sender, Views.SelectedPlateViewEventArgs e)
        {
            if(e.SelectedIndex == 0)
            {
                View.SetChartOverlay("Percent FOV Counted", 
                                     Model.Rccs.Select(x => x.PctFovCounted).ToArray(), 
                                     Model.Rccs.Select(x => x.FileName).ToArray());
            }
            else if(e.SelectedIndex == 1)
            {
                View.SetChartOverlay("Binding Density",
                                     Model.Rccs.Select(x => x.BindingDensity).ToArray(),
                                     Model.Rccs.Select(x => x.FileName).ToArray());
            }
            else if(e.SelectedIndex == 2)
            {
                View.SetChartOverlay("POS Control GeoMean",
                                     Model.Rccs.Select(x => x.GeoMeanOfPos).ToArray(),
                                     Model.Rccs.Select(x => x.FileName).ToArray());
            }
            else
            {
                return;
            }
        }
    }
}

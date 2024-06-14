using RccAppDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Presenters
{
    public class RawCountsPlateViewPresenter
    {
        private Views.IRawCountsPlateView View { get; set; }
        private IRawCountsPlateModel Model { get; set; }

        public RawCountsPlateViewPresenter(Views.IRawCountsPlateView view, IRawCountsPlateModel model)
        {
            View = view;
            Model = model;

            View.SetQcPropertySelectorComboItems(model.PlexQcPropertyList);

            view.CalculateQcMetricForSelectedPlateviewPage += new EventHandler<Views.SelectedPlateViewEventArgs>(View_CalculateQcMetricForSelectedPlateviewPage);

            string[][] mat0 = Model.GetSelectedLaneQcData(0);
            string[][] mat1 = Model.GetSelectedCellQcData(View.SelectedQcProperty, 0);
            View.SetDgv1Values(mat0, 0);
            View.SetDgv2Values(mat1, 0);
        }

        private void View_CalculateQcMetricForSelectedPlateviewPage(object sender, Views.SelectedPlateViewEventArgs e)
        {
            string[][] mat0 = Model.GetSelectedLaneQcData(e.SelectedIndex);
            string[][] mat1 = Model.GetSelectedCellQcData(View.SelectedQcProperty, e.SelectedIndex);
            View.SetDgv1Values(mat0, e.SelectedIndex);
            View.SetDgv2Values(mat1, e.SelectedIndex);
        }
    }
}

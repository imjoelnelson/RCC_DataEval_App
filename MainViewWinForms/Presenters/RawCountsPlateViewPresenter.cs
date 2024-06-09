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
        private static Dictionary<string, string> QcTypeList = new Dictionary<string, string>()
        {
            { "Hyb POS Control Count", "PosCount" },
            { "Hyb NEG Control Count", "NegCount" },
            { "Assay POS Control GeoMean", "AssayPosGeoMean" },
            { "Assay NEG Control GeoMean", "AssayNegGeoMean" },
            { "Housekeeping GeoMean", "HkGeoMean" },
            { "Endogenous Max Count", "EndoMax" },
            { "Endogenous Min Count", "EndoMin" },
            { "% Above Threshold", "PctAboveThresh" }
        };
        private Views.IRawCountsPlateView View { get; set; }
        private IRawCountsPlateModel Model { get; set; }
        

        public RawCountsPlateViewPresenter(Views.IRawCountsPlateView view, IRawCountsPlateModel model)
        {
            View = view;
            Model = model;

            View.ComboBoxSelectionChanged += new EventHandler(View_ComboBoxSelectionChanged);
            View.SetQcPropertySelectorComboItems(model.PlexQcPropertyList);

            string[][] mat0 = Model.GetSelectedLaneQcData(model.Rccs);
            string[][] mat1 = Model.GetSelectedCellQcData(Model.SelectedQcProperty, model.Rccs);
            view.SetDgv1Values(mat0);
            view.SetDgv2Values(mat1);
        }

        private void View_ComboBoxSelectionChanged(object sender, EventArgs e)
        {
            if(Model.Rccs != null)
            {
                if(Model.Rccs.Count > 0)
                {
                    string[][] mat = Model.GetSelectedCellQcData(View.SelectedQcProperty, Model.Rccs);
                    View.SetDgv2Values(mat);
                }
            }
        }
    }
}

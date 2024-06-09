using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Presenters
{
    public class SampleVsScatterplotPresenter
    {
        private Views.ISampleVsScatterplotView View { get; set; }
        private RccAppDataModels.ISampleVsScatterplotModel Model { get; set; }

        public SampleVsScatterplotPresenter(Views.ISampleVsScatterplotView view, 
            RccAppDataModels.ISampleVsScatterplotModel model)
        {
            View = view;
            Model = model;

            View.UseCutoffChanged += new EventHandler(View_UseCutoffChanged);
            View.ThresholdChanged += new EventHandler(View_ThresholdChanged);
            View.DgvCellContentClick += new EventHandler<Views.ScatterPlotSelectArgs>(View_DgvCellContentClick);

            // Reset bindings
            View.Source.DataSource = Model.Samples;
            View.Source.ResetBindings(false);

            // Model should have initially marked sample 1 and 2 for display so call for chart for these two samples here
            RccAppDataModels.ScatterChartDto dto = Model.GetChartData(View.UseCutoff, View.Threshold);
            if(dto != null)
            {
                View.SetCorrelationChart(dto.TargetNames, dto.XVals, dto.YVals, dto.XName, dto.YName, dto.RegLine, dto.RSquared);
            }
        }

        private void View_UseCutoffChanged(object sender, EventArgs e)
        {
            RccAppDataModels.ScatterChartDto dto = Model.GetChartData(View.UseCutoff, View.Threshold);
            if (dto != null)
            {
                View.SetCorrelationChart(dto.TargetNames, dto.XVals, dto.YVals, dto.XName, dto.YName, dto.RegLine, dto.RSquared);
            }
        }
        private void View_ThresholdChanged(object sender, EventArgs e)
        {
            RccAppDataModels.ScatterChartDto dto = Model.GetChartData(View.UseCutoff, View.Threshold);
            if (dto != null)
            {
                View.SetCorrelationChart(dto.TargetNames, dto.XVals, dto.YVals, dto.XName, dto.YName, dto.RegLine, dto.RSquared);
            }
        }
        private void View_DgvCellContentClick(object sender, Views.ScatterPlotSelectArgs e)
        {
            Model.UpdateSelection(e.RowIndex, e.ColIndex);
            View.Source.DataSource = Model.Samples;
            View.Source.ResetBindings(false);
            RccAppDataModels.ScatterChartDto dto = Model.GetChartData(View.UseCutoff, View.Threshold);
            if (dto != null)
            {
                View.SetCorrelationChart(dto.TargetNames, dto.XVals, dto.YVals, dto.XName, dto.YName, dto.RegLine, dto.RSquared);
            }
        }
    }
}

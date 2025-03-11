using MessageCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace MainViewWinForms.Presenters
{
    public class SelectHKsPresenter
    {
        private Views.ISelectHKsView View { get; set; }
        private RccAppDataModels.ISelectHKsModel Model { get; set; }
        private bool Result { get; set; }

        public SelectHKsPresenter(Views.ISelectHKsView view, RccAppDataModels.ISelectHKsModel model)
        {
            View = view;
            Model = model;

            View.SettingsButtonClicked += new EventHandler(View_SettingsButtonClicked);
            PresenterHub.MessageHub.Subscribe<GeNormPreFilterSettingsClosedMessage>((m) => HandleGeNormFilterSettingsClosed());

            if (Model.ErrorMessage != null)
            {
                HandleModelError(Model.ErrorMessage);
            }

            try
            {
                bool useCountThreshold = Properties.Settings.Default.GeNormPreFilterUseCountThreshold;
                bool useAvgCountThreshold = Properties.Settings.Default.GeNormPreFilterUseAvgCountThreshold;
                int countThreshold = Properties.Settings.Default.GeNormPreFilterCountThreshold;
                int avgCountThreshold = Properties.Settings.Default.GeNormPreFilterAvgCountThreshold;
                Result = model.UpdateHousekeeperList(useCountThreshold, useAvgCountThreshold, countThreshold, avgCountThreshold);
            }
            catch(Exception er)
            {
                string message = $"{er.Message}\r\n{er.StackTrace}";
                HandleModelError(Tuple.Create(message, true));
                Result = false;
            }
            if(!Result)
            {
                HandleModelError(Model.ErrorMessage);
            }

            if(Result)
            {
                view.UpdateChart(Model.HousekeeperList);
            }
        }

        private void HandleGeNormFilterSettingsClosed()
        {
            try
            {
                bool useCountThreshold = Properties.Settings.Default.GeNormPreFilterUseCountThreshold;
                bool useAvgCountThreshold = Properties.Settings.Default.GeNormPreFilterUseAvgCountThreshold;
                int countThreshold = Properties.Settings.Default.GeNormPreFilterCountThreshold;
                int avgCountThreshold = Properties.Settings.Default.GeNormPreFilterAvgCountThreshold;
                Result = Model.UpdateHousekeeperList(useCountThreshold, useAvgCountThreshold, countThreshold, avgCountThreshold);
            }
            catch (Exception er)
            {
                string message = $"{er.Message}\r\n{er.StackTrace}";
                HandleModelError(Tuple.Create(message, true));
                Result = false;
            }
            if (!Result)
            {
                HandleModelError(Model.ErrorMessage);
            }

            if (Result)
            {
                View.UpdateChart(Model.HousekeeperList);
            }
        }

        private void HandleModelError(Tuple<string, bool> errorMessage)
        {
            View.ErrorMessage = errorMessage;
        }

        private void View_SettingsButtonClicked(object sender, EventArgs e)
        {
            Views.IGenormPreFilterSettingsView view = MVPFactory.ThisGeNormPreFilterSettingsView();
            view.ShowForm();
        }
    }
}

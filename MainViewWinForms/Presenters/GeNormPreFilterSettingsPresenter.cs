using MessageCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace MainViewWinForms.Presenters
{
    public class GeNormPreFilterSettingsPresenter
    {
        private Views.IGenormPreFilterSettingsView View { get; set; }

        public GeNormPreFilterSettingsPresenter(Views.IGenormPreFilterSettingsView view)
        {
            View = view;
            View.UseCountThreshold = Properties.Settings.Default.GeNormPreFilterUseCountThreshold;
            View.CountThreshold = Properties.Settings.Default.GeNormPreFilterCountThreshold;
            View.UseAvgCountThreshold = Properties.Settings.Default.GeNormPreFilterUseAvgCountThreshold;
            View.AvgCountThreshold = Properties.Settings.Default.GeNormPreFilterAvgCountThreshold;
            View.OKButtonClicked += new EventHandler(View_OKButtonClicked);
        }

        private void View_OKButtonClicked(object sender, EventArgs e)
        {
            Properties.Settings.Default.GeNormPreFilterUseCountThreshold = View.UseCountThreshold;
            Properties.Settings.Default.GeNormPreFilterCountThreshold = View.CountThreshold;
            Properties.Settings.Default.GeNormPreFilterUseAvgCountThreshold = View.UseAvgCountThreshold;
            Properties.Settings.Default.GeNormPreFilterAvgCountThreshold = View.AvgCountThreshold;
            Properties.Settings.Default.Save();
            View.CloseForm();
            PresenterHub.MessageHub.Publish<GeNormPreFilterSettingsClosedMessage>(new GeNormPreFilterSettingsClosedMessage());
        }
    }
}

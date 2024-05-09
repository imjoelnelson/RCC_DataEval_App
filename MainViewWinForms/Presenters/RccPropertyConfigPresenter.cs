using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Presenters
{
    public class RccPropertyConfigPresenter
    {
        Views.IRccPropertyConfigView View { get; set; }
        public RccPropertyConfigPresenter(Views.IRccPropertyConfigView view)
        {
            View = view;
            View.OkButtonClicked += new EventHandler(View_OkButtonClicked);
        }

        private void View_OkButtonClicked(object sender, EventArgs e)
        {
            Properties.Settings.Default.SelectedProperties = string.Join(",", View.SelectedProperties);
            View.CloseForm();
        }
    }
}

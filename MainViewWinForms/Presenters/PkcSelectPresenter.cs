using RccAppDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public class PkcSelectPresenter
    {
        IPkcSelectView View { get; set; }
        IPkcSelectModel Model { get; set; }

        public PkcSelectPresenter(IPkcSelectView view, IPkcSelectModel model, List<string> cartIDs)
        {
            View = view;
            Model = model;

            View.AddButtonCicked += new EventHandler(View_AddButtonClicked);
            View.RemoveButtonClicked += new EventHandler<PkcAddRemoveArgs>(View_RemoveButtonClicked);
            View.NextButtonClicked += new EventHandler(View_NextButtonClicked);
            View.TabPageTextBoxModified += new EventHandler<PkcSelectBoxEventArgs>(View_TabPageTextBoxModified);
        }

        private void View_AddButtonClicked(object sender, EventArgs e)
        {

        }

        private void View_RemoveButtonClicked(object sender, PkcAddRemoveArgs e)
        {

        }

        private void View_NextButtonClicked(object sender, EventArgs e)
        {

        }

        private void View_TabPageTextBoxModified(object sender, PkcSelectBoxEventArgs e)
        {

        }
    }
}

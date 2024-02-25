using RCCAppPresenters;
using RccAppDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms
{
    class MVPFactory
    {
        public static MainView GetMainView()
        {
            IDataModel model = new RccDataModel();
            MainView view = new MainView();
            MainViewPresenter presenter = new MainViewPresenter(view, model);
            view.Presenter = presenter;
            return view;
        }
    }
}

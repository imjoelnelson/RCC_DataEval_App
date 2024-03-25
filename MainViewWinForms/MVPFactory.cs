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

        public static PasswordEnterView PasswordEnterView(string fileName)
        {
            PasswordEnterView passwordView = new PasswordEnterView(fileName);
            _ = new PasswordEnterPresenter(passwordView, fileName);
            return passwordView;
        }

        public static Views.PkcSelectView PkcView(List<string> cartridgeIDs, Dictionary<string, string> savedPkcs)
        {
            Views.PkcSelectView view = new Views.PkcSelectView(cartridgeIDs);
            PkcSelectModel model = new PkcSelectModel(cartridgeIDs, savedPkcs);
            _ = new Presenters.PkcSelectPresenter(view, model, cartridgeIDs);
            return view;
        }
    }
}

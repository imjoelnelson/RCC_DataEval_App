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
        /// <summary>
        /// Creates MVP for the main form of the app
        /// </summary>
        /// <returns>The app main form</returns>
        public static MainView GetMainView()
        {
            IDataModel model = new RccDataModel();
            MainView view = new MainView();
            MainViewPresenter presenter = new MainViewPresenter(view, model);
            view.Presenter = presenter;
            return view;
        }

        /// <summary>
        /// Creates MVP for password enter dialog
        /// </summary>
        /// <param name="fileName">The name of the zip the password is for</param>
        /// <returns>Password enter dialog</returns>
        public static PasswordEnterView PasswordEnterView(string fileName)
        {
            PasswordEnterView passwordView = new PasswordEnterView(fileName);
            _ = new PasswordEnterPresenter(passwordView, fileName);
            return passwordView;
        }

        /// <summary>
        /// Creates MVP for PKC selection dialog
        /// </summary>
        /// <param name="cartridgeIDs">The IDs of cartridges with GeoMx readout RCCs that need PKCs selected</param>
        /// <param name="savedPkcs">PKCs already imported and saved in appdata</param>
        /// <returns>PKC selection dialog</returns>
        public static Views.PkcSelectView PkcView(List<string> cartridgeIDs, Dictionary<string, string> savedPkcs)
        {
            Views.PkcSelectView view = new Views.PkcSelectView(cartridgeIDs);
            PkcSelectModel model = new PkcSelectModel(cartridgeIDs, savedPkcs);
            _ = new Presenters.PkcSelectPresenter(view, model, cartridgeIDs);
            return view;
        }

        /// <summary>
        /// Creates view and presenter for raw count table preferences dialog
        /// </summary>
        /// <returns>The raw counts table preferences dialog</returns>
        public static Views.RccPropertyConfigView RccPropertyView()
        {
            // Get RCC properties from RCC static list of properties; exclude flags
            List<Tuple<string, string>> rccProperties = NCounterCore.Rcc.Properties.Where(x => !x.Value.Item2.EndsWith("Pass"))
                                                                                   .Select(x => Tuple.Create(x.Key, x.Value.Item2))
                                                                                   .ToList();
            // Get saved selectedProperties from settings
            List<string> selectedProperties = Properties.Settings.Default.SelectedProperties.Split(',').ToList();
            // Create view and presenter
            Views.RccPropertyConfigView view = new Views.RccPropertyConfigView(rccProperties, selectedProperties);
            _ = new Presenters.RccPropertyConfigPresenter(view);
            return view;
        }
    }
}

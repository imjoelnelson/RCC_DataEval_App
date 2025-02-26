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
        public static Views.PkcSelectView PkcView(List<Tuple<string, string[]>> cartridgeIDs, Dictionary<string, string> savedPkcs)
        {
            Views.PkcSelectView view;
            if (cartridgeIDs.Any(x => x.Item2 != null))
            {
                view = new Views.PkcSelectView(cartridgeIDs);
            }
            else
            {
                view = new Views.PkcSelectView(cartridgeIDs.Select(x => x.Item1).ToList());
            }
            PkcSelectModel model = new PkcSelectModel(cartridgeIDs.Select(x => x.Item1).ToList(), savedPkcs);
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

        public static Views.RawCountsPlateViewcs RawCountPlateView(List<NCounterCore.Rcc> rccs)
        {
            Views.RawCountsPlateViewcs view = new Views.RawCountsPlateViewcs(rccs.Select(x => x.CartridgeID)
                                                                                 .Distinct()
                                                                                 .ToList());
            RawCountsPlateModel model = new RawCountsPlateModel(rccs.OrderBy(x => x.LaneID).ToList(), "Hyb POS Control Count", view.Threshold);
            Presenters.RawCountsPlateViewPresenter presenter = new Presenters.RawCountsPlateViewPresenter(view, model);

            return view;
        }

        public static Views.SampleVsScatterplotView SampleVsScatterView(List<NCounterCore.Rcc> rccs)
        {
            List<string> rlfs = rccs.Select(x => x.ThisRLF.Name).Distinct().ToList();
            if (rlfs.Count > 1)
            {
                System.Windows.Forms.MessageBox.Show("The included RCCs represent more than one RLF however this function is currently only compatible with one RLF. Try again after selecting RCCs from only one RLF.",
                                                     "Multiple RLFs Detected",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }
            if(rccs[0].ThisRLF.ThisType == NCounterCore.RlfType.DSP || rccs[0].ThisRLF.ThisType == NCounterCore.RlfType.PlexSet)
            {
                System.Windows.Forms.MessageBox.Show("The included RCCs are from a sample-multiplexed assay, however this function is currently incompatible with sample-multiplexed assays.",
                                                     "Sample-Multiplexed Assay Detected",
                                                     System.Windows.Forms.MessageBoxButtons.OK);
                return null;
            }

            Views.SampleVsScatterplotView view = new Views.SampleVsScatterplotView(rccs.Count);
            SampleVsScatterplotModel model = new SampleVsScatterplotModel(rccs);
            _ = new Presenters.SampleVsScatterplotPresenter(view, model);

            return view;
        }

        public static Views.BinnedCountsView CountBinsView(List<NCounterCore.Rcc> rccs, int[] cutoffs)
        {
            Views.BinnedCountsView view = new Views.BinnedCountsView();
            RccAppDataModels.BinnedCountsModel model = new BinnedCountsModel(rccs, cutoffs);
            _ = new Presenters.BinnedCountsPresenter(view, model);

            return view;
        }

        public static Views.PcaQcView ThisPcaQcView(List<NCounterCore.Rcc> rccs)
        {
            Views.PcaQcView view = new Views.PcaQcView();
            PcaQcModel model = new PcaQcModel(rccs);
            _ = new Presenters.PcaQcPresenter(view, model);

            return view;
        }
    }
}

using MessageCenter;
using NCounterCore;
using RccAppDataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinyMessenger;

namespace MainViewWinForms.Presenters
{
    public class PkcSelectPresenter
    {
        Views.IPkcSelectView View { get; set; }
        IPkcSelectModel Model { get; set; }

        public PkcSelectPresenter(Views.IPkcSelectView view, IPkcSelectModel model, List<string> cartIDs)
        {
            View = view;
            Model = model;

            View.AddButtonCicked += new EventHandler(View_AddButtonClicked);
            View.RemoveButtonClicked += new EventHandler<Views.PkcAddRemoveArgs>(View_RemoveButtonClicked);
            View.NextButtonClicked += new EventHandler(View_NextButtonClicked);
            View.TabPageListBox2DoubleClicked += new EventHandler<Views.PkcSelectBoxEventArgs>(View_TabPageListBox2DoubleClicked);
        }

        private void View_AddButtonClicked(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Select PKC file to import";
                ofd.Filter = "PKC|*.PKC";
                ofd.RestoreDirectory = true;
                ofd.Multiselect = true;
                if(ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach(string s in ofd.FileNames)
                    {
                        string name = System.IO.Path.GetFileNameWithoutExtension(s);
                        //try
                        //{
                            if(!Model.SavedPkcs.ContainsKey(s))
                            {
                                _ = new NCounterCore.PkcReader(s);
                                Model.SavedPkcs.Add(name, s);
                                PresenterHub.MessageHub.Publish<PkcAddMessage>(new PkcAddMessage(this, s));
                            }
                        //}
                        //catch
                        //{
                        //    MessageBox.Show($"{name} could not be loaded because it was corrupted, not formatted correctly, or open in another application",
                        //                       "PKC Import Error",
                        //                       MessageBoxButtons.OK);
                        //}
                    }
                }
            }
        }

        private void View_RemoveButtonClicked(object sender, Views.PkcAddRemoveArgs e)
        {
            Model.RemovePkcFromSavedList(e.PkcName);
            PresenterHub.MessageHub.Publish<PkcRemoveMessage>(new PkcRemoveMessage(this, e.PkcName));
        }

        private void View_NextButtonClicked(object sender, EventArgs e)
        {
            foreach (CartridgePkcSelectItem c in Model.CartridgePkcs)
            {
                Tuple<string, Dictionary<string, ProbeItem>> cartTranslator = Tuple.Create(c.CartridgeID, c.Collector.DspTranslator);
                PresenterHub.MessageHub.Publish<TranslatorSendMessage>(new TranslatorSendMessage(this, cartTranslator));
            }
            View.CloseForm();
        }

        /// <summary>
        /// Transfers event and PKC data from TabPage TextBox_TextChanged event to the associated row item in Model
        /// </summary>
        /// <param name="sender">tabpage</param>
        /// <param name="e">Event containing string CartridgeID, int Row, and string[] selected PKC names</param>
        private void View_TabPageListBox2DoubleClicked(object sender, Views.PkcSelectBoxEventArgs e)
        {
            Model.PkcsChanged(e.CartridgeID, e.PkcNames);
        }
    }
}

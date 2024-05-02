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
        private Views.IPkcSelectView View { get; set; }
        private IPkcSelectModel Model { get; set; }


        public PkcSelectPresenter(Views.IPkcSelectView view, IPkcSelectModel model, List<string> cartIDs)
        {
            View = view;
            Model = model;
            view.UpdateSavedPkcBox(Model.SavedPkcs.Keys.ToArray());

            View.AddButtonCicked += new EventHandler(View_AddButtonClicked);
            View.RemoveButtonClicked += new EventHandler<Views.PkcAddRemoveArgs>(View_RemoveButtonClicked);
            View.NextButtonClicked += new EventHandler(View_NextButtonClicked);
            View.SelectButtonClicked += new EventHandler<Views.PkcSelectBoxEventArgs>(View_SelectButtonClicked);
            View.CartridgeRemoveButtonClicked += new EventHandler<Views.PkcSelectBoxEventArgs>(View_CartridgeRemoveButtonClick);
            Model.SelectedPkcsChanged += new EventHandler<ModelPkcSelectBoxArgs>(Model_SelectedPkcsChanged);
            Model.SavedPkcsChanged += new EventHandler<ModelPkcAddRemoveArgs>(Model_SavedPkcsChanged);
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
                        try
                        {
                            if (!Model.SavedPkcs.ContainsKey(s))
                            {
                                _ = new NCounterCore.PkcReader(s);
                                Model.AddPkcToSavedList(s);
                                PresenterHub.MessageHub.Publish<PkcAddMessage>(new PkcAddMessage(this, s));
                            }
                        }
                        catch
                        {
                            MessageBox.Show($"{name} could not be loaded because it was corrupted, not formatted correctly, or open in another application",
                                           "PKC Import Error",
                                           MessageBoxButtons.OK);
                        }
                    }
                }
            }
        }

        private void View_RemoveButtonClicked(object sender, Views.PkcAddRemoveArgs e)
        {
            Model.RemovePkcFromSavedList(e.PkcName);
            PresenterHub.MessageHub.Publish<PkcRemoveMessage>(new PkcRemoveMessage(this, Model.SavedPkcs[e.PkcName]));
        }

        private void View_NextButtonClicked(object sender, EventArgs e)
        {
            if(Model.CartridgePkcs.Any(x => x.PkcReaders.Count > 0)) // At least one cartridge had one or more PKCs selected
            {
                foreach (CartridgePkcSelectItem c in Model.CartridgePkcs)
                {
                    if (c.Collector != null)
                    {
                        if (c.Collector.DspTranslator != null)
                        {
                            Tuple<string, string, Dictionary<string, ProbeItem>> cartTranslator = Tuple.Create(c.CartridgeID, c.Collector.Name, c.Collector.DspTranslator);
                            PresenterHub.MessageHub.Publish<TranslatorSendMessage>(new TranslatorSendMessage(this, cartTranslator));
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Selecct PKCs for at least one of the tabbed cartridges before continuing. To select PKCs, highlight one ore more PKCs in the 'Available' box and click the 'Select >>' button to move it to a cartridge's 'Selected' box.",
                                "No PKCs Selected",
                                MessageBoxButtons.OK);
                return;
            }
            
            View.CloseForm();
        }

        private void View_SelectButtonClicked(object sender, Views.PkcSelectBoxEventArgs e)
        {
            Model.AddNewCartridgePkcs(e.CartridgeID, e.PkcNames, Model.SavedPkcs);
        }

        private void View_CartridgeRemoveButtonClick(object sender, Views.PkcSelectBoxEventArgs e)
        {
            Model.ClearSelectedCartridgePkcs(e.CartridgeID, e.PkcNames);
        }

        private void Model_SelectedPkcsChanged(object sender, ModelPkcSelectBoxArgs e)
        {
            View.UpdateCartridgePkcBox(e.CartridgeID, e.PkcNames);
        }

        private void Model_SavedPkcsChanged(object sender, ModelPkcAddRemoveArgs e)
        {
            View.UpdateSavedPkcBox(e.PkcNames);
        }
    }
}
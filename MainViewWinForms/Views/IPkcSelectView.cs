using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainViewWinForms.Views
{
    public interface IPkcSelectView
    { 
        event EventHandler AddButtonCicked;
        event EventHandler<PkcAddRemoveArgs> RemoveButtonClicked;
        event EventHandler NextButtonClicked;
        event EventHandler<PkcSelectBoxEventArgs> SelectButtonClicked;
        event EventHandler<PkcSelectBoxEventArgs> CartridgeRemoveButtonClicked;

        void ShowForm();
        void CloseForm();
        void ProcessSelectButtonCLicked(string cartridgeID, string[] pkcNames);
        void UpdateCartridgePkcBox(string cartridgeID, string[] pkcNames);
        void UpdateSavedPkcBox(string[] pkcNames);
    }
}

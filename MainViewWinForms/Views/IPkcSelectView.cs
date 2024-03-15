using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface IPkcSelectView
    {
        string[] SelectedPkcs { get; set; }

        event EventHandler AddButtonCicked;
        event EventHandler<PkcAddRemoveArgs> RemoveButtonClicked;
        event EventHandler NextButtonClicked;

        void CloseForm();
    }
}

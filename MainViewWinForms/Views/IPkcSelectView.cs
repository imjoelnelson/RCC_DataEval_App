using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainViewWinForms.Views
{
    public interface IPkcSelectView
    {
        event EventHandler AddButtonCicked;
        event EventHandler<PkcAddRemoveArgs> RemoveButtonClicked;
        event EventHandler NextButtonClicked;
        event EventHandler<PkcSelectBoxEventArgs> TabPageListBox2DoubleClicked;

        void ShowForm();
        void CloseForm();
    }
}

using System;
using System.Windows.Forms;

namespace MainViewWinForms
{
    public interface IPasswordEnterView
    {
        PasswordEnterPresenter PasswordPresenter { get; set; }
        string Password { get; set; }

        event EventHandler PasswordEntered;
        event EventHandler Skipped;

        DialogResult ShowAsDialog();
        void CloseForm();
    }
}

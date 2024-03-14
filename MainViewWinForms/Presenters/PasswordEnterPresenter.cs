using MessageCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace MainViewWinForms
{
    public class PasswordEnterPresenter
    {
        public PasswordEnterPresenter(IPasswordEnterView passwordView, string fileName)
        {
            if(passwordView.ShowAsDialog() == System.Windows.Forms.DialogResult.OK)
            {
                PresenterHub.MessageHub.Publish<PasswordSendMessage>(new 
                    PasswordSendMessage(this, Tuple.Create(fileName, passwordView.Password)));
            }
            passwordView.CloseForm();
        }
    }
}

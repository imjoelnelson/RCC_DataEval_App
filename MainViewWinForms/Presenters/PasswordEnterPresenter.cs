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
        IPasswordEnterView PasswordView { get; set; }
        public PasswordEnterPresenter(IPasswordEnterView passwordView, string fileName)
        {
            PasswordView = passwordView;
            if(passwordView.ShowAsDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Call method to send TinyMessage with password payload destined for MainPresenter
            }
        }

        private void SendPasswordMessage(string fileName, string password)
        {
            GenericTinyMessage<Tuple<string, string>> passwordSend = new GenericTinyMessage<Tuple<string, string>>
                (this, Tuple.Create(fileName, password));
        }
    }
}

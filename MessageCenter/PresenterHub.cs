using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace MessageCenter
{
    public static class PresenterHub
    {
        public static TinyMessengerHub MessageHub = new TinyMessengerHub();
    }

    public class PasswordSendMessage : GenericTinyMessage<Tuple<string, string>>
    {
        public PasswordSendMessage(object sender, Tuple<string, string> content) : base(sender, content) { }
    }

    public class PasswordRequestMessage : GenericTinyMessage<string>
    {
        public PasswordRequestMessage(object sender, string content) : base(sender, content) { }
    }
        
}

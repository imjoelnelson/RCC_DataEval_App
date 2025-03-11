using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyMessenger;

namespace MessageCenter
{
    // Hub
    public static class PresenterHub
    {
        public static TinyMessengerHub MessageHub = new TinyMessengerHub();
    }
    
    // Messages
    public class PasswordSendMessage : GenericTinyMessage<Tuple<string, string>>
    {
        public PasswordSendMessage(object sender, Tuple<string, string> content) : base(sender, content) { }
    }

    public class PasswordRequestMessage : GenericTinyMessage<string>
    {
        public PasswordRequestMessage(object sender, string content) : base(sender, content) { }
    }

    public class DirectoryToDeleteMessage : GenericTinyMessage<string>
    {
        public DirectoryToDeleteMessage(object sender, string content) : base(sender, content) { }
    }
    public class PkcAddMessage : GenericTinyMessage<string>
    {
        public PkcAddMessage(object sender, string content) : base(sender, content) { }
    }
    public class PkcRemoveMessage : GenericTinyMessage<string>
    {
        public PkcRemoveMessage(object sender, string content) : base(sender, content) { }
    }
    public class TranslatorSendMessage : GenericTinyMessage<Tuple<string, string, Dictionary<string, NCounterCore.ProbeItem>>>
    {
        public TranslatorSendMessage(object sender, Tuple<string, string, Dictionary<string, NCounterCore.ProbeItem>> content) : base(sender, content) { }
    }
    public class GeNormPreFilterSettingsClosedMessage : ITinyMessage
    {
        public object Sender { get; private set; }
    }
}

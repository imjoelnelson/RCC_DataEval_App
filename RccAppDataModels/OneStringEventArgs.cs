using System;

namespace RccAppDataModels
{
    public class OneStringEventArgs : EventArgs
    {
        public string EventString { get; set; }
        public OneStringEventArgs(string eventString)
        {
            EventString = eventString;
        }
    }
}

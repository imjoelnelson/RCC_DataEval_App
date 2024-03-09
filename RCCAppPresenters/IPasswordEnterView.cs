using System;

namespace RccAppPresenters
{
    public interface IPasswordEnterView
    {
        string CurrentPassword { get; set; }

        event EventHandler PasswordEntered;
        event EventHandler Skipped;
        void CloseForm();
    }
}

using System;

namespace LocalConnect.Helpers
{
    public class OnDataLoadEventArgs : EventArgs
    {
        public bool IsSuccesful { get; }

        public string ErrorMessage { get; }

        public bool IsUnauthorized { get; }

        public OnDataLoadEventArgs()
        {
            IsSuccesful = true;
        }

        public OnDataLoadEventArgs(string errorMessage, bool isIsUnauthorized = false)
        {
            ErrorMessage = errorMessage;
            IsSuccesful = string.IsNullOrEmpty(errorMessage);
            IsUnauthorized = isIsUnauthorized;
        }
    }

    public delegate void OnDataLoadEventHandler(object sender, OnDataLoadEventArgs e);
}

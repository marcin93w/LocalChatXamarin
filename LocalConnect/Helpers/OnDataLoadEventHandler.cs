using System;
using System.Collections.Generic;
using System.Text;
using LocalConnect.Services;

namespace LocalConnect.Helpers
{
    public class OnDataLoadEventArgs : EventArgs
    {
        public bool IsSuccesful { get; }

        public string ErrorMessage { get; }

        public bool ApplicationNotInitialized { get; }

        public OnDataLoadEventArgs()
        {
            IsSuccesful = true;
        }

        public OnDataLoadEventArgs(string errorMessage, bool appNotInitialized = false)
        {
            ErrorMessage = errorMessage;
            IsSuccesful = string.IsNullOrEmpty(errorMessage);
            ApplicationNotInitialized = appNotInitialized;
        }

        public OnDataLoadEventArgs(Exception ex)
        {
            ErrorMessage = ex.Message;
            ApplicationNotInitialized = ex is MissingAuthenticationTokenException;
            IsSuccesful = false;
        }
    }

    public delegate void OnDataLoadEventHandler(object sender, OnDataLoadEventArgs e);
}

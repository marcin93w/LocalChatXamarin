using System;
using System.Collections.Generic;
using System.Text;
using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface IChatClient
    {
        void Connect(string personId);
        event MessageReceivedEventHandler OnMessageReceived;
        void SendMessage(OutcomeMessage message, int messageIndex);
    }
}

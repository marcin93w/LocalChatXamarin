using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnectTest.Helpers
{
    class SocketClientMock : ISocketClient
    {
        private List<OutcomeMessage> _messagesWitingForSentConfirmation = new List<OutcomeMessage>();
        private List<OutcomeMessage> _messagesWitingForDisplayedConfirmation = new List<OutcomeMessage>();

        public SocketClientMock()
        {
            SentMessages = new List<OutcomeMessage>();
        }
        
        public void MarkMessageAsDisplayed(IncomeMessage message)
        {
        }

        public bool IsConnected { get; private set; }

        public List<OutcomeMessage> SentMessages { get; } 

        public bool Connect()
        {
            IsConnected = true;
            return true;
        }

        public void Disconnect()
        {
            OnMessageReceived = null;
        }

        public event MessageReceivedEventHandler OnMessageReceived;

        public void SendMessage(OutcomeMessage message, int messageIndex)
        {
            message.MessageId = message.ReceiverId + messageIndex;
            _messagesWitingForSentConfirmation.Add(message);
            _messagesWitingForDisplayedConfirmation.Add(message);
            SentMessages.Add(message);
        }

        bool ISocketClient.IsConnected
        {
            get { return IsConnected; }
            set { IsConnected = value; }
        }

        public void InvokeMessageReceive(IncomeMessage msg)
        {
            OnMessageReceived(this, new MessageReceivedEventArgs(msg));
        }
    }
}

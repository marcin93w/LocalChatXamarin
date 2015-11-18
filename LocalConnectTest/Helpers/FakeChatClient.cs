using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnectTest.Helpers
{
    class FakeSocketClient : ISocketClient
    {
        public FakeSocketClient()
        {
            SentMessages = new List<OutcomeMessage>();
        }

        public void MarkMessageAsDisplayed(IncomeMessage message)
        {
            throw new NotImplementedException();
        }

        public bool IsConnected { get; private set; }
        public string ConnectedPersonId { get; private set; }

        public List<OutcomeMessage> SentMessages { get; } 

        public void Connect(string personId)
        {
            IsConnected = true;
            ConnectedPersonId = personId;
        }

        public bool Connect()
        {
            throw new NotImplementedException();
        }

        public event MessageReceivedEventHandler OnMessageReceived;

        public void SendMessage(OutcomeMessage message, int messageIndex)
        {
            SentMessages.Add(message);
        }

        bool ISocketClient.IsConnected
        {
            get { return IsConnected; }
            set { IsConnected = value; }
        }

        public void UpdateMyLocation(Location location)
        {
            throw new NotImplementedException();
        }
    }
}

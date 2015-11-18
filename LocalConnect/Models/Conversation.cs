using System;
using System.Collections;
using System.ComponentModel;
using System.Threading.Tasks;
using LocalConnect.Helpers;
using LocalConnect.Services;
using Newtonsoft.Json.Linq;

namespace LocalConnect.Models
{
    public class Conversation
    {
        private readonly ISocketClient _socketClient;

        private readonly string _personId;

        public ObservableInvokableCollection<Message> Messages { get; }

        public bool IsHolded { set; get; }

        public Conversation(string personId, ISocketClient socketClient, RunOnUiThreadHandler uiThreadHandler)
        {
            _socketClient = socketClient;
            _personId = personId;
            Messages = new ObservableInvokableCollection<Message>(uiThreadHandler);
            socketClient.OnMessageReceived -= HandleMessageReceive;
            socketClient.OnMessageReceived += HandleMessageReceive;
        }

        private void HandleMessageReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            if (!IsHolded && messageReceivedEventArgs.Message.SenderId == _personId)
            {
                Messages.Add(messageReceivedEventArgs.Message);
            }
        }

        public void SendMessage(string message)
        {
            var msg = new OutcomeMessage(_personId, message, DateTime.Now);
            Messages.Add(msg);
            _socketClient.SendMessage(msg, Messages.IndexOf(msg));
        }

        public async Task FetchLastMessages(IRestClient restClient)
        {
            var lastMessages = await restClient.FetchDataAsync($"lastMessagesWith/{_personId}");

            foreach (JContainer message in (IEnumerable)lastMessages)
            {
                Message msg;
                if (message.Value<string>("sender") == _personId)
                {
                    msg = new IncomeMessage(_personId, message.Value<string>("text"), message.Value<DateTime>("dateTime"));
                }
                else
                {
                    msg = new OutcomeMessage(_personId, message.Value<string>("text"), message.Value<DateTime>("dateTime"))
                    {
                        Sent = true
                    };
                    var status = message.Value<int?>("status");
                    if (status.HasValue && status > 2)
                        msg.Displayed = true;
                }

                Messages.Insert(0, msg);
            }
        }
    }
}

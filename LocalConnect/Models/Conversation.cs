using System;
using System.Collections;
using System.Linq;
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
                _socketClient.MarkMessageAsDisplayed(messageReceivedEventArgs.Message);
            }
        }

        public void SendMessage(string message)
        {
            var msg = new OutcomeMessage(null, _personId, message, DateTime.Now);
            Messages.Add(msg);
            _socketClient.SendMessage(msg, Messages.IndexOf(msg));
        }

        private async Task<int> FetchMessages(IRestClient restClient, Message olderThan = null)
        {
            var query = $"lastMessagesWith/{_personId}";
            if (olderThan != null)
                query += $"?olderThan={olderThan.MessageId}";
            var lastMessages = await restClient.FetchDataAsync(query);

            int count = 0;
            foreach (JContainer message in (IEnumerable)lastMessages)
            {
                Message msg;
                if (message.Value<string>("sender") == _personId)
                {
                    msg = new IncomeMessage(message.Value<string>("_id"), 
                        _personId, message.Value<string>("text"), message.Value<DateTime>("dateTime"));
                }
                else
                {
                    msg = new OutcomeMessage(message.Value<string>("_id"), 
                        _personId, message.Value<string>("text"), message.Value<DateTime>("dateTime"))
                    {
                        Sent = true
                    };
                }
                var status = message.Value<int?>("status");
                if (status.HasValue && status > 2)
                {
                    msg.Displayed = true;
                }
                
                if (msg is IncomeMessage && !msg.Displayed)
                {
                    _socketClient.MarkMessageAsDisplayed((IncomeMessage) msg);    
                }

                Messages.Insert(0, msg);
                count++;
            }

            return count;
        }

        public async Task FetchLastMessages(IRestClient restClient)
        {
            await FetchMessages(restClient);
        }

        public async Task<bool> FetchOlderMessages(IRestClient restClient)
        {
            return (await FetchMessages(restClient, Messages.FirstOrDefault())) > 0;
        }
    }
}

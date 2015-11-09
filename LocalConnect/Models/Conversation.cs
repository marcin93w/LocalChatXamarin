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

        public ObservableInvokableCollection<Message> Messages { get; }
        public Person Person { get; }

        public bool IsHolded { set; get; }

        public Conversation(Person person, ISocketClient socketClient, RunOnUiThreadHandler uiThreadHandler)
        {
            _socketClient = socketClient;
            Person = person;
            Messages = new ObservableInvokableCollection<Message>(uiThreadHandler);
            socketClient.OnMessageReceived -= HandleMessageReceive;
            socketClient.OnMessageReceived += HandleMessageReceive;
        }

        private void HandleMessageReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            if (!IsHolded && messageReceivedEventArgs.Message.SenderId == Person.PersonId)
            {
                Messages.Add(messageReceivedEventArgs.Message);
            }
        }

        public void SendMessage(string message)
        {
            var msg = new OutcomeMessage(Person.PersonId, message, DateTime.Now);
            Messages.Add(msg);
            _socketClient.SendMessage(msg, Messages.IndexOf(msg));
        }

        public async Task FetchLastMessages(IRestClient restClient)
        {
            var lastMessages = await restClient.FetchDataAsync($"lastMessagesWith/{Person.PersonId}");

            foreach (JContainer message in (IEnumerable)lastMessages)
            {
                Message msg;
                if (message.Value<string>("sender") == Person.PersonId)
                {
                    msg = new IncomeMessage(Person.PersonId, message.Value<string>("text"), message.Value<DateTime>("dateTime"));
                }
                else
                {
                    msg = new OutcomeMessage(Person.PersonId, message.Value<string>("text"), message.Value<DateTime>("dateTime"));
                }

                Messages.Insert(0, msg);
            }
        }
    }
}

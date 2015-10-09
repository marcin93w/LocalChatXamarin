
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Json;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Helpers;
using LocalConnect.Android;
using LocalConnect.Services;
using LocalConnect.ViewModel;
using LocalConnect.Interfaces;
using Newtonsoft.Json.Linq;

namespace LocalConnect.Models
{
    public class Conversation
    {
        private readonly IChatClient _chatClient;

        public ObservableInvokableCollection<Message> Messages { get; }
        public Person Person { get; }

        public bool IsHolded { set; get; }

        public Conversation(Person person, IChatClient chatClient, RunOnUiThreadHandler uiThreadHandler)
        {
            _chatClient = chatClient;
            Person = person;
            Messages = new ObservableInvokableCollection<Message>(uiThreadHandler);
            chatClient.OnMessageReceived += HandleMessageReceive;
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
            if (IsHolded)
                throw new InvalidAsynchronousStateException("Chat is not started with any person");

            var msg = new OutcomeMessage(Person.PersonId, message, DateTime.Now);
            Messages.Add(msg);
            _chatClient.SendMessage(msg, Messages.IndexOf(msg));
        }

        public async Task FetchLastMessages(IDataProvider dataProvider)
        {
            var lastMessages = await dataProvider.FetchDataAsync($"lastMessagesWith/{Person.PersonId}");

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

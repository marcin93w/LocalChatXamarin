
using System;
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

namespace LocalConnect.Models
{
    public class Conversation
    {
        public ObservableInvokableCollection<Message> Messages { get; }
        public Person Person { get; }

        public bool IsHolded { set; get; }

        public Conversation(Person person, RunOnUiThreadHandler uiThreadHandler)
        {
            Person = person;
            Messages = new ObservableInvokableCollection<Message>(uiThreadHandler);
            ChatClient.Instance.OnMessageReceived += HandleOnMessageReceive;
        }

        private void HandleOnMessageReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
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
            ChatClient.Instance.SendMessage(msg, Messages.IndexOf(msg));
        }

        public async Task FetchLastMessages()
        {
            var lastMessages = await RestClient.Instance.FetchDataAsync($"lastMessagesWith/{Person.PersonId}");

            foreach (JsonValue message in lastMessages)
            {
                Message msg;
                if (message.GetValue("sender") == Person.PersonId)
                {
                    msg = new IncomeMessage(Person.PersonId, message.GetValue("text"), DateTime.Parse(message.GetValue("dateTime")));
                }
                else
                {
                    msg = new OutcomeMessage(Person.PersonId, message.GetValue("text"), DateTime.Parse(message.GetValue("dateTime")));
                }
            
                Messages.Insert(0, msg);            
            }
        }
    }
}


using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
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

        public Conversation(string personId, RunOnUiThreadHandler uiThreadHandler)
        {
            var peopleFinder = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
            Person = peopleFinder.GetPersonForId(personId);
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
    }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Helpers;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PersonChatViewModel : ViewModelBase, 
        IUiInvokable, IRestClientUsingViewModel, ISocketClientUsingViewModel
    {
        private Conversation _conversation;

#region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

#endregion

        public PersonViewModel Person { private set; get; }
        public ObservableCollection<Message> Messages => _conversation.Messages;
        public bool DataLoaded { private set; get; }

        public ISocketClient SocketClient { private get; set; }
        public IRestClient RestClient { private get; set; }

        public void Initialize(PersonViewModel person)
        {
            if (Person == null || Person.Id != person.Id || person.UnreadMessages.HasValue)
            {
                Person = person;
                _conversation = new Conversation(Person.Id, SocketClient, RunOnUiThread);
                DataLoaded = false;
                Person.ClearUnreadMessages();
            }
        }

        public async Task<bool> FetchDataAsync()
        {
            try
            {
                await Person.LoadDetailedData(RestClient);
                await _conversation.FetchLastMessages(RestClient);
                DataLoaded = true;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void StopChat()
        {
            _conversation.IsHolded = true;
        }
        public void ResumeChat()
        {
            _conversation.IsHolded = false;
        }

        public void SendMessage(string message)
        {
            _conversation.SendMessage(message);     
        }

        public string GetStatusText(OutcomeMessage message)
        {
            if (message.Sent)
                return "Sent";
            if (message.DeliverError)
                return "Error";
            if (message.Displayed)
                return "Displayed";
            return "Sending...";
        }
    }
}
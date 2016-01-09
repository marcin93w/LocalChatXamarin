using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Helpers;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PersonChatViewModel : ViewModelBase, IUiInvokable
    {
        private Conversation _conversation;

        private readonly ISocketClient _socketClient;
        private readonly IRestClient _restClient;

        public PersonChatViewModel(ISocketClient socketClient, IRestClient restClient)
        {
            _socketClient = socketClient;
            _restClient = restClient;
        }

        #region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

#endregion

        public PersonViewModel Person { private set; get; }
        public ObservableCollection<Message> Messages => _conversation.Messages;
        public bool DataLoaded { private set; get; }

        public void Initialize(PersonViewModel person)
        {
            if (Person == null || Person.Id != person.Id || person.UnreadMessages.HasValue)
            {
                Person = person;
                _conversation = new Conversation(Person.Id, _socketClient, RunOnUiThread);
                DataLoaded = false;
                Person.ClearUnreadMessages();
            }
        }

        public async Task<bool> FetchDataAsync()
        {
            try
            {
                await Person.LoadDetailedData(_restClient);
                await _conversation.FetchLastMessages(_restClient);
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

        public async Task<bool?> LoadOlderMessages()
        {
            try
            {
                return (await _conversation.FetchOlderMessages(_restClient)) ? true : (bool?) null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
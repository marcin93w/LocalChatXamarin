using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Helpers;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PersonViewModel : ViewModelBase, IUiInvokable, IDataFetchingViewModel, ISocketClientUsingViewModel
    {
        private Conversation _conversation;

#region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

#endregion

        public Person Person { private set; get; }
        public ObservableCollection<Message> Messages => _conversation.Messages;

        public ISocketClient SocketClient { private get; set; }
        public IDataProvider DataProvider { private get; set; }

        public event OnDataLoadEventHandler OnDataLoad;

        public void Initialize(Person person)
        {
            Person = person;
            _conversation = new Conversation(Person, SocketClient, RunOnUiThread);
        }

        public async void FetchDataAsync()
        {
            bool dataLoaded;
            string errorMsg = String.Empty;
            try
            {
                await Person.LoadDetailedData(DataProvider);
                await _conversation.FetchLastMessages(DataProvider);
                dataLoaded = true;
            }
            catch (Exception ex)
            {
                dataLoaded = false;
                errorMsg = ex.Message;
            }

            OnDataLoad?.Invoke(this, 
                new OnDataLoadEventArgs(dataLoaded ? null : "Error loading person data. " + errorMsg));
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
    }
}
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Android.Provider;
using GalaSoft.MvvmLight;
using LocalConnect.Android;
using LocalConnect.Models;
using LocalConnect.Interfaces;

namespace LocalConnect.ViewModel
{
    public class ChatViewModel : ViewModelBase, IUiInvokable, IDataFetchingViewModel
    {
        private Conversation _conversation;

#region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

#endregion

        public Person Person { private set; get; }
        public ObservableCollection<Message> Messages => _conversation.Messages;

        public event OnDataLoadEventHandler OnDataLoad;

        public void Initialize(Person person)
        {
            Person = person;
            _conversation = new Conversation(Person, RunOnUiThread);
        }

        public async void FetchDataAsync()
        {
            bool dataLoaded;
            string errorMsg = String.Empty;
            try
            {
                var personDataLoading = Person.LoadDetailedData();
                await _conversation.FetchLastMessages();
                dataLoaded = await personDataLoading;
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
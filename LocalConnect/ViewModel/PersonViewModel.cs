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

        public bool Initialize(string personId)
        {
            try
            {
                var peopleFinder = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
                Person = peopleFinder.GetPersonForId(personId);
                _conversation = new Conversation(Person, RunOnUiThread);
                return true;
            }
            catch (InvalidAsynchronousStateException)
            {
                return false;
            }
        }

        public async void FetchDataAsync()
        {
            bool dataLoaded;
            try
            {
                dataLoaded = await Person.LoadDetailedData();
            }
            catch (Exception)
            {
                dataLoaded = false;
                throw;
            }

            OnDataLoad?.Invoke(this, 
                new OnDataLoadEventArgs(dataLoaded ? null : "Error loading person data."));
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
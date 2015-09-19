using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;
using LocalConnect2.Models;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{
    public class ChatViewModel : ViewModelBase, IUiInvokableViewModel
    {
        private readonly ChatClient _chatService;
        private readonly IPeopleFinder _peopleFinder;

        private bool _isInitialized;

#region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

#endregion

        public Person Person { private set; get; }
        public ObservableCollection<Message> Messages { private set; get; }

        public ChatViewModel()
        {
            Messages = new ObservableCollection<Message>();
            _peopleFinder = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
            _chatService = ChatClient.Instance;
        }

        public void Connect(string userId)
        {
            _chatService.MessageReceived += HandleMessageReceive;
            _chatService.Connect(userId);
        }

        public bool StartChatWith(string userId)
        {
            try
            {
                Person = _peopleFinder.GetPersonForId(userId);
                _isInitialized = true;
                return true;
            }
            catch (InvalidAsynchronousStateException)
            {
                return false;
            }
        }

        public void EndChat()
        {
            _isInitialized = false;
            Messages = new ObservableCollection<Message>();
        }

        private void HandleMessageReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            if (_isInitialized && Person.UserId == messageReceivedEventArgs.Message.SenderId)
            {
                RunOnUiThread(() => Messages.Add(messageReceivedEventArgs.Message));
            }
        }

        public void SendMessage(string message)
        {
            if(!_isInitialized)
                throw new InvalidAsynchronousStateException("Chat is not started with any person");

            var msg = new OutcomeMessage(Person.UserId, message, DateTime.Now);
            _chatService.SendMessage(msg);
            Messages.Add(msg);
        }

    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect2.Models;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{
    public class ChatViewModel : ViewModelBase, IUiInvokableViewModel
    {
        private readonly ChatClient _chatService;

        #region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

        #endregion

        public Person Person { private set; get; }
        public ObservableCollection<Message> Messages { get; }

        public ChatViewModel()
        {
            Messages = new ObservableCollection<Message>();
            _chatService = ChatClient.Instance;
        }

        public void InitializeChatWith(Person person)
        {
            Person = person;
            _chatService.MessageReceived += HandleMessageReceive;
        }

        private void HandleMessageReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            RunOnUiThread(() => Messages.Add(messageReceivedEventArgs.Message));
        }

        public void SendMessage(string message)
        {
            _chatService.SendMessage(message);
            //Messages.Add(new MessageViewModel(message));
        }

    }
}
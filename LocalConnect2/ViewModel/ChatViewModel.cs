using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Runtime;
using GalaSoft.MvvmLight;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{
    public class ChatViewModel : ViewModelBase, IUiInvokableViewModel
    {
        private readonly ChatClient _chatService;
        public RunOnUiThreadHandler RunOnUiThread { get; set; }

        public ObservableCollection<MessageViewModel> Messages { set; get; }

        public ChatViewModel()
        {
            Messages = new ObservableCollection<MessageViewModel>();
            _chatService = new ChatClient();
            _chatService.Initialize();
            _chatService.MessageReceived += HandleMessageReceive;
        }

        private void HandleMessageReceive(object sender, MessageReceivedEventArgs messageReceivedEventArgs)
        {
            RunOnUiThread(() => Messages.Add(new MessageViewModel(messageReceivedEventArgs.Message)));
        }

        public void SendMessage(string message)
        {
            _chatService.SendMessage(message);
            Messages.Add(new MessageViewModel(message));
        }

    }
}
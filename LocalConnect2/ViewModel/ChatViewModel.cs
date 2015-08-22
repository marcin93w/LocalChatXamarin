using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.Runtime;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{
    public class ChatViewModel : UiInvokableViewModel
    {
        private readonly ChatService _chatService;

        public ObservableCollection<MessageViewModel> Messages { set; get; }

        public ChatViewModel(Action<Action> uiThreadHandler) : base(uiThreadHandler)
        {
            Messages = new ObservableCollection<MessageViewModel>();
            _chatService = new ChatService();
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
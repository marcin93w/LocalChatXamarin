using System.Collections.ObjectModel;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Interfaces;

namespace LocalConnect.ViewModel
{
    public class ChatViewModel : ViewModelBase, IUiInvokable
    {
        private Conversation _conversation;

#region IUiInvokableViewModel implementation

        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

#endregion

        public Person Person => _conversation.Person;
        public ObservableCollection<Message> Messages => _conversation.Messages;

        public bool StartChatWith(string personId)
        {
            try
            {
                _conversation = new Conversation(personId, RunOnUiThread);
                return true;
            }
            catch (InvalidAsynchronousStateException)
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
    }
}
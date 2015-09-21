using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect.ViewModel;
using Message = LocalConnect.Models.Message;

namespace LocalConnect.Android.Activities
{
    [Activity(Label = "ChatActivity",
        Theme = "@android:style/Theme.Black.NoTitleBar",
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class ChatActivity : Activity
    {
        private readonly ChatViewModel _chatViewModel;
        //private string _chatUserId;

        private TextView _messageTextView;

        public ChatActivity()
        {
            _chatViewModel = ViewModelLocator.Instance.GetViewModel<ChatViewModel>(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Chat);

            var chatUserId = Intent.GetStringExtra("UserId");
            if (!_chatViewModel.StartChatWith(chatUserId))
            {
                Finish();
            }

            var messagesList = FindViewById<ListView>(Resource.Id.MessagesList);
            messagesList.Adapter = _chatViewModel.Messages.GetAdapter(GetMessageView);

            _messageTextView = FindViewById<TextView>(Resource.Id.TextInput);
            var sendButton = FindViewById<Button>(Resource.Id.SendButton);
            sendButton.Click += SendMessageClick;
        }

        private View GetMessageView(int position, Message message, View convertView)
        {
            if (convertView == null)
            {
                convertView = LayoutInflater.Inflate(global::Android.Resource.Layout.ActivityListItem, null);
            }
            var text = convertView.FindViewById<TextView>(global::Android.Resource.Id.Text1);
            text.Text = message.Text;
            return convertView;
        }

        protected override void OnStart()
        {
            _chatViewModel.ResumeChat();
            base.OnStart();

        }

        protected override void OnStop()
        {
            _chatViewModel.StopChat();
            base.OnStop();
        }

        private void SendMessageClick(object sender, EventArgs args)
        {
            try
            {
                _chatViewModel.SendMessage(_messageTextView.Text);
                _messageTextView.Text = string.Empty;
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Your message was not send please try again", ToastLength.Short);
            }
        }
    }
}
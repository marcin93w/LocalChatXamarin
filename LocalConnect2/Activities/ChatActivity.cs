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
using LocalConnect2.ViewModel;

namespace LocalConnect2.Activities
{
    [Activity(Label = "ChatActivity",
        Theme = "@android:style/Theme.Black.NoTitleBar",
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class ChatActivity : Activity
    {
        private readonly ChatViewModel _chatViewModel;

        public ChatActivity()
        {
            _chatViewModel = ViewModelLocator.Instance.GetViewModel<ChatViewModel>(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Chat);

            var messagesList = FindViewById<ListView>(Resource.Id.MessagesList);
            messagesList.Adapter = _chatViewModel.Messages.GetAdapter(GetMessageView);

            var messageText = FindViewById<TextView>(Resource.Id.TextInput);
            var sendButton = FindViewById<Button>(Resource.Id.SendButton);
            sendButton.Click += (sender, args) =>
            {
                _chatViewModel.SendMessage(messageText.Text);
                messageText.Text = string.Empty;
            };
        }

        private View GetMessageView(int position, MessageViewModel message, View convertView)
        {
            if (convertView == null)
            {
                convertView = LayoutInflater.Inflate(Android.Resource.Layout.ActivityListItem, null);
            }
            var text = convertView.FindViewById<TextView>(Android.Resource.Id.Text1);
            text.Text = message.Content;
            return convertView;
        }
    }
}
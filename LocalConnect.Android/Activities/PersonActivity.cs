using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using Newtonsoft.Json;
using Message = LocalConnect.Models.Message;

namespace LocalConnect.Android.Activities
{
    [Activity(Label = "ChatActivity",
        WindowSoftInputMode = SoftInput.AdjustResize)]
    public class PersonActivity : Activity
    {
        private readonly ChatViewModel _personViewModel;

        private bool _moreInfoDisplayed;

        private TextView _messageTextView;

        public PersonActivity()
        {
            _personViewModel = ViewModelLocator.Instance.GetViewModel<ChatViewModel>(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Person);

            try
            {
                var person = JsonConvert.DeserializeObject<Person>(Intent.GetStringExtra("Person"));
                _personViewModel.Initialize(person);
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Wrong Intent data", ToastLength.Long);
                return;
            }

            var longDescription = FindViewById<TextView>(Resource.Id.LongDescription);
            _personViewModel.OnDataLoad += (sender, args) =>
            {
                if (args.IsSuccesful)
                {
                    longDescription.Text = _personViewModel.Person.LongDescription;
                }
                else
                {
                    Toast.MakeText(this, args.ErrorMessage, ToastLength.Long);
                }
            };

            _personViewModel.FetchDataAsync();

            var personName = FindViewById<TextView>(Resource.Id.PersonName);
            personName.Text = _personViewModel.Person.Name;

            var personShortDescription = FindViewById<TextView>(Resource.Id.ShortDescription);
            personShortDescription.Text = _personViewModel.Person.Description;

            var moreButton = FindViewById<ImageButton>(Resource.Id.MoreButton);
            moreButton.Click += ToggleMoreLessInfo;

            InitializeChat();
        }

        public int ConvertDpToPx(float dp)
        {
            return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, Resources.DisplayMetrics);
        }

        private void ToggleMoreLessInfo(object sender, EventArgs eventArgs)
        {
            var personImage = FindViewById<ImageView>(Resource.Id.PersonImage);
            var personImageLayoutParams = personImage.LayoutParameters;
            var longDescription = FindViewById(Resource.Id.LongDescription);
            var actionsPanel = FindViewById(Resource.Id.ActionsPanel);
            var moreButton = FindViewById<ImageButton>(Resource.Id.MoreButton);
            if (_moreInfoDisplayed)
            {
                personImageLayoutParams.Width = personImageLayoutParams.Height = ConvertDpToPx(50);
                personImage.LayoutParameters = personImageLayoutParams;
                longDescription.Visibility = ViewStates.Gone;
                actionsPanel.Visibility = ViewStates.Gone;
                _moreInfoDisplayed = false;
                moreButton.Rotation = 0;
            }
            else
            {
                personImageLayoutParams.Width = personImageLayoutParams.Height = ConvertDpToPx(100);
                personImage.LayoutParameters = personImageLayoutParams;
                longDescription.Visibility = ViewStates.Visible;
                actionsPanel.Visibility = ViewStates.Visible;
                _moreInfoDisplayed = true;
                moreButton.Rotation = 180;
            }

            var messagesList = FindViewById(Resource.Id.MessagesList);
            messagesList.PostInvalidate();
        }

        private void InitializeChat()
        {
            var messagesList = FindViewById<ListView>(Resource.Id.MessagesList);
            messagesList.Adapter = _personViewModel.Messages.GetAdapter(GetMessageView);
            messagesList.ItemClick += ToggleMessageInfo;

            _messageTextView = FindViewById<TextView>(Resource.Id.TextInput);
            var sendButton = FindViewById<View>(Resource.Id.SendButton);
            sendButton.Click += SendMessageClick;
        }

        private View GetMessageView(int position, Message message, View convertView)
        {
            if (convertView == null || !CanReuseConvertView(convertView, message))
            {
                if (message is IncomeMessage)
                    convertView = LayoutInflater.Inflate(Resource.Layout.IncomeMessageItem, null);
                else
                    convertView = LayoutInflater.Inflate(Resource.Layout.OutcomeMessageItem, null);
            }

            var text = convertView.FindViewById<TextView>(Resource.Id.MessageText);
            text.Text = message.Text;
            var dateTime = convertView.FindViewById<TextView>(Resource.Id.MessageDateTime);
            dateTime.Text = message.Time.ToString("G");
            var status = convertView.FindViewById<TextView>(Resource.Id.MessageStatus);

            return convertView;
        }

        private bool CanReuseConvertView(View convertView, Message message)
        {
            if (convertView.Id == Resource.Layout.IncomeMessageItem && message is IncomeMessage)
                return true;
            if (convertView.Id == Resource.Layout.OutcomeMessageItem && message is OutcomeMessage)
                return true;

            return false;
        }

        private void ToggleMessageInfo(object sender, AdapterView.ItemClickEventArgs e)
        {
            var dateTime = e.View.FindViewById<TextView>(Resource.Id.MessageDateTime);
            dateTime.Visibility = dateTime.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
            var status = e.View.FindViewById<TextView>(Resource.Id.MessageStatus);
            status.Visibility = status.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
        }

        protected override void OnStart()
        {
            _personViewModel.ResumeChat();
            base.OnStart();
        }

        protected override void OnStop()
        {
            _personViewModel.StopChat();
            base.OnStop();
        }

        private void SendMessageClick(object sender, EventArgs args)
        {
            try
            {
                _personViewModel.SendMessage(_messageTextView.Text);
                _messageTextView.Text = string.Empty;
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Your message was not send please try again", ToastLength.Short);
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using Square.Picasso;
using Message = LocalConnect.Models.Message;

namespace LocalConnect.Android.Views
{
    [Activity(WindowSoftInputMode = SoftInput.AdjustResize)]
    public class PersonChatActivity : Activity
    {
        private readonly PersonChatViewModel _personChatViewModel;
        private readonly PeopleViewModel _peopleViewModel;

        private bool _moreInfoDisplayed;

        private TextView _messageTextView;

        public PersonChatActivity()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetUiInvokableViewModel<PeopleViewModel>(this);
            _personChatViewModel = ViewModelLocator.Instance.GetUiInvokableViewModel<PersonChatViewModel>(this);
        }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Person);

            var person = _peopleViewModel.People.First(p => p.Id == Intent.GetStringExtra("PersonId"));

            _personChatViewModel.Initialize(person);

            Task<bool> conversationDataLoading = null;
            if(!_personChatViewModel.DataLoaded)
                conversationDataLoading = _personChatViewModel.FetchDataAsync();

            var personName = FindViewById<TextView>(Resource.Id.PersonName);
            personName.Text = _personChatViewModel.Person.Name;

            var personShortDescription = FindViewById<TextView>(Resource.Id.ShortDescription);
            personShortDescription.Text = _personChatViewModel.Person.ShortDescription;

            if (!string.IsNullOrEmpty(_personChatViewModel.Person.Avatar))
            {
                var personAvatar = FindViewById<ImageView>(Resource.Id.PersonImage);
                Picasso.With(this)
                    .Load(_personChatViewModel.Person.Avatar)
                    .Into(personAvatar);
            }

            var personInfoPanel = FindViewById<ViewGroup>(Resource.Id.personInfoPanel);
            personInfoPanel.Click += ToggleMoreLessInfo;

            var messagesList = FindViewById<ListView>(Resource.Id.MessagesList);
            InitializeChat(messagesList);

            var dataLoaded = true;
            if (conversationDataLoading != null)
            {
                if (!await conversationDataLoading)
                {
                    Toast.MakeText(ApplicationContext, "Could not connect to server", ToastLength.Long).Show();
                    dataLoaded = false;
                }
            }

            if (dataLoaded)
            {
                var longDescription = FindViewById<TextView>(Resource.Id.LongDescription);
                longDescription.Text = _personChatViewModel.Person.LongDescription;
                if(_personChatViewModel.Messages.Count > 0)
                    messagesList.AddHeaderView(LayoutInflater.Inflate(Resource.Layout.GetMoreMessages, null), null, true);
            }
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
            var moreButton = FindViewById<ImageView>(Resource.Id.MoreButton);
            if (_moreInfoDisplayed)
            {
                personImageLayoutParams.Width = personImageLayoutParams.Height = ConvertDpToPx(50);
                personImage.LayoutParameters = personImageLayoutParams;
                longDescription.Visibility = ViewStates.Gone;
                _moreInfoDisplayed = false;
                moreButton.Rotation = 0;
            }
            else
            {
                personImageLayoutParams.Width = personImageLayoutParams.Height = ConvertDpToPx(100);
                personImage.LayoutParameters = personImageLayoutParams;
                longDescription.Visibility = ViewStates.Visible;
                _moreInfoDisplayed = true;
                moreButton.Rotation = 180;
            }

            var messagesList = FindViewById(Resource.Id.MessagesList);
            messagesList.PostInvalidate();
        }

        private void InitializeChat(ListView messagesList)
        {
            messagesList.Adapter = _personChatViewModel.Messages.GetAdapter(GetMessageView);
            messagesList.ItemClick += OnMessagesListItemClick;

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
            dateTime.Text = message.DateTime.ToString("G");
            var status = convertView.FindViewById<TextView>(Resource.Id.MessageStatus);
            if (message is OutcomeMessage)
            {
                status.Text = _personChatViewModel.GetStatusText(message as OutcomeMessage);
                message.StatusChanged += (sender, args) =>
                    RunOnUiThread(() => status.Text = _personChatViewModel.GetStatusText(message as OutcomeMessage));
            }
            else
            {
                status.Visibility = ViewStates.Gone;
                status.Text = string.Empty;
            }

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

        private void OnMessagesListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position > 0 || e.View.FindViewById<TextView>(Resource.Id.MessagesLoadText) == null)
            {
                ToggleMessageInfo(e.View);
            }
            else 
            {
                LoadOlderMessages(e.View);
            }
        }

        private static void ToggleMessageInfo(View view)
        {
            var dateTime = view.FindViewById<TextView>(Resource.Id.MessageDateTime);
            dateTime.Visibility = dateTime.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
            var status = view.FindViewById<TextView>(Resource.Id.MessageStatus);
            status.Visibility = status.Visibility == ViewStates.Gone ? ViewStates.Visible : ViewStates.Gone;
        }

        private async void LoadOlderMessages(View view)
        {
            var textView = view.FindViewById<TextView>(Resource.Id.MessagesLoadText);
            textView.Text = "Loading...";
            bool? olderMessagesLoadingStatus = await _personChatViewModel.LoadOlderMessages();
            if (olderMessagesLoadingStatus == true)
            {
                textView.Text = "Load older messages";
            }
            else if (olderMessagesLoadingStatus == false)
            {
                textView.Text = "Error loading messages";
            }
            else
            {
                view.Visibility = ViewStates.Gone;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();
            _personChatViewModel.ResumeChat();
        }

        protected override void OnStop()
        {
            _personChatViewModel.StopChat();
            base.OnStop();
        }

        private void SendMessageClick(object sender, EventArgs args)
        {
            try
            {
                _personChatViewModel.SendMessage(_messageTextView.Text);
                _messageTextView.Text = string.Empty;
            }
            catch (Exception)
            {
                Toast.MakeText(this, "Your message was not send please try again", ToastLength.Short);
            }
        }
    }
}
using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using LocalConnect.ViewModel;
using Square.Picasso;

namespace LocalConnect.Android.Activities.Adapters
{
    public class PeopleListAdapter : ArrayAdapter<string>
    {
        private readonly PeopleViewModel _peopleViewModel;
        private Activity _activity;

        public PeopleListAdapter(Activity mainActivity, Context context, int textViewResourceId, PeopleViewModel peopleViewModel) 
            : base(context, textViewResourceId, peopleViewModel.People.Select(p => p.Name).ToArray())
        {
            this._peopleViewModel = peopleViewModel;
            _activity = mainActivity;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater) Context
                .GetSystemService(Context.LayoutInflaterService);
            var listItemView = inflater.Inflate(Resource.Layout.ListItem, parent, false);

            var name = listItemView.FindViewById<TextView>(Resource.Id.personName);
            var image = listItemView.FindViewById<ImageView>(Resource.Id.personImage);
            var desc = listItemView.FindViewById<TextView>(Resource.Id.personDesc);
            var locationDesc = listItemView.FindViewById<TextView>(Resource.Id.personLocationDesc);

            var person = _peopleViewModel.People[position];

            name.Text = person.Name;
            desc.Text = person.ShortDescription;
            locationDesc.Text = person.LocationDescription;

            SetupUnreadMessagesIndicator(listItemView, person.UnreadMessages);

            if (!string.IsNullOrEmpty(person.Avatar))
                LoadUserAvatar(image, person.Avatar);

            person.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "UnreadMessages")
                {
                     _activity.RunOnUiThread(() => SetupUnreadMessagesIndicator(listItemView, person.UnreadMessages));
                }
            };

            return listItemView;
        }

        private void SetupUnreadMessagesIndicator(View listItemView, int? unreadMessages)
        {
            var unreadMsgsPanel = listItemView.FindViewById<ViewGroup>(Resource.Id.unreadMessagePanel);
            var uneadMsgsCount = listItemView.FindViewById<TextView>(Resource.Id.unreadMessageCount);
            if (unreadMessages.HasValue)
            {
                uneadMsgsCount.Text = unreadMessages.Value.ToString();
                unreadMsgsPanel.Visibility = ViewStates.Visible;
                listItemView.SetBackgroundColor(Color.Argb(128, Color.DarkGreen.R, Color.DarkGreen.G, Color.DarkGreen.B));
            }
            else
            {
                unreadMsgsPanel.Visibility = ViewStates.Gone;
                listItemView.SetBackgroundColor(Color.Transparent);
            }
        }

        private void LoadUserAvatar(ImageView image, string imageUrl)
        {
            Picasso.With(Context)
               .Load(imageUrl)
               .Into(image);
        }
    }
}
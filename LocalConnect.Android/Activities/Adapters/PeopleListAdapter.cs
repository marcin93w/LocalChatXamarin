using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Views;
using Android.Widget;
using LocalConnect.ViewModel;
using Square.Picasso;

namespace LocalConnect.Android.Activities.Adapters
{
    public class PeopleListAdapter : ArrayAdapter<string>
    {
        private readonly PeopleViewModel _peopleViewModel;
        
        public PeopleListAdapter(Context context, int textViewResourceId, PeopleViewModel peopleViewModel) 
            : base(context, textViewResourceId, peopleViewModel.People.Select(p => p.Name).ToArray())
        {
            this._peopleViewModel = peopleViewModel;
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

            name.Text = _peopleViewModel.People[position].Name;
            desc.Text = _peopleViewModel.People[position].ShortDescription;
            locationDesc.Text = _peopleViewModel.GetLocationDescription(
                _peopleViewModel.People[position]);

            if(!string.IsNullOrEmpty(_peopleViewModel.People[position].Avatar))
                LoadUserAvatar(image, _peopleViewModel.People[position].Avatar);

            return listItemView;
        }

        private void LoadUserAvatar(ImageView image, string imageUrl)
        {
            Picasso.With(Context)
               .Load(imageUrl)
               .Into(image);
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using LocalConnect.ViewModel;

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

            name.Text = _peopleViewModel.People[position].Name;
            desc.Text = _peopleViewModel.People[position].ShortDescription;

            if(!string.IsNullOrEmpty(_peopleViewModel.People[position].Avatar))
                LoadUserAvatar(image, _peopleViewModel.People[position].Avatar);

            return listItemView;
        }

        private async void LoadUserAvatar(ImageView image, string imageUrl)
        {
            var imageBitmap = GetImageBitmapFromUrl(imageUrl);
            image.SetImageBitmap(await imageBitmap);
        }

        private async Task<Bitmap> GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = await webClient.DownloadDataTaskAsync(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }
}
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
using LocalConnect.Android.Activities.Adapters;
using LocalConnect.Helpers;
using LocalConnect.ViewModel;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect.Android.Activities
{
    public class ListViewFragment : Fragment
    {
        private readonly PeopleViewModel _peopleViewModel ;

        private ViewGroup _rootView;
        private ListView _list;

        public ListViewFragment()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>(Activity);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            _rootView = (ViewGroup)inflater.Inflate(
                    Resource.Layout.ListViewFragment, container, false);

            _list = _rootView.FindViewById<ListView>(Resource.Id.listView);
            _peopleViewModel.OnPeopleLoaded += OnPeopleLoad;
            _list.ItemClick += UserToChatSelected;

            return _rootView;
        }

        private void OnPeopleLoad(object sender, OnDataLoadEventArgs e)
        {
            _list.Adapter = new PeopleListAdapter(Activity, _rootView.Context,
                Resource.Layout.ListItem, _peopleViewModel);
        }

        private void UserToChatSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            var chatActivity = new Intent(Activity.ApplicationContext, typeof(PersonChatActivity));
            var person = _peopleViewModel.People[e.Position];
            chatActivity.PutExtra("PersonId", person.Id);
            StartActivity(chatActivity);
        }
    }
}
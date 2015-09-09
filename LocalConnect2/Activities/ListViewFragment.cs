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
using LocalConnect2.Activities.Adapters;
using LocalConnect2.Services;
using LocalConnect2.ViewModel;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect2.Activities
{
    public class ListViewFragment : Fragment
    {
        private readonly PeopleViewModel _peopleViewModel ;

        private ViewGroup _rootView;

        public ListViewFragment()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            _rootView = (ViewGroup)inflater.Inflate(
                    Resource.Layout.ListViewFragment, container, false);

            _peopleViewModel.OnDataLoad += OnDataLoad;
            _peopleViewModel.FetchData();

            return _rootView;
        }

        private void OnDataLoad(object sender, OnDataLoadEventArgs eventArgs)
        {
            if (eventArgs.IsSuccesful)
            {
                var list = _rootView.FindViewById<ListView>(Resource.Id.listView);
                list.Adapter = new PeopleListAdapter(_rootView.Context,
                    Resource.Layout.ListItem, _peopleViewModel);
                list.ItemClick += UserToChatSelected;

                var myName = _rootView.FindViewById<TextView>(Resource.Id.MeName);
                myName.Text = _peopleViewModel.MyName;
            }
            else
            {
                if (Activity != null)
                    Toast.MakeText(Activity, eventArgs.ErrorMessage, ToastLength.Long).Show();
            }
        }

        private void UserToChatSelected(object sender, AdapterView.ItemClickEventArgs e)
        {
            var chatActivity = new Intent(Activity.ApplicationContext, typeof(ChatActivity));
            chatActivity.PutExtra("User", e.Id);
            StartActivity(chatActivity);
        }
    }
}
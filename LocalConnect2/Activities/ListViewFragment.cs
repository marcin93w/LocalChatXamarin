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
using LocalConnect2.ViewModels;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect2.Activities
{
    public class ListViewFragment : Fragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            var rootView = (ViewGroup)inflater.Inflate(
                    Resource.Layout.ListViewFragment, container, false);

            var list = rootView.FindViewById<ListView>(Resource.Id.listView);
            list.Adapter = new PeopleListAdapter(rootView.Context, Resource.Layout.ListItem,
                new PeopleViewModel());

            return rootView;
        }
    }
}
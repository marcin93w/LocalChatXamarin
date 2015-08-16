using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect2.Activities
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback
    {
        private readonly ViewPager _viewPager;

        public MapViewFragment(ViewPager viewPager)
        {
            _viewPager = viewPager;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            var rootView = (ViewGroup)inflater.Inflate(
                    Resource.Layout.MapViewFragment, container, false);

            var mapFragment = (SupportMapFragment) ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            var backToListButton = rootView.FindViewById<Button>(Resource.Id.backToList);
            backToListButton.Click += (sender, args) => _viewPager.SetCurrentItem(0, true);

            return rootView;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            //TODO
        }
    }
}
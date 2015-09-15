using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using Java.Lang.Reflect;
using LocalConnect2.ViewModel;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect2.Activities
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback
    {
        private readonly ViewPager _viewPager;
        private readonly PeopleViewModel _peopleViewModel;
        private GoogleMap _map;

        public MapViewFragment(ViewPager viewPager)
        {
            _viewPager = viewPager;
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
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
            _map = googleMap;
            _map.MapType = GoogleMap.MapTypeHybrid;
            _peopleViewModel.OnDataLoad += OnDataLoad;
        }

        private void OnDataLoad(object sender, OnDataLoadEventArgs e)
        {
            if (e.IsSuccesful)
            {
                var bounds = new LatLngBounds.Builder();
                var peopleWithLocation = _peopleViewModel.People.Where(p => p.Location != null);
                foreach (var person in peopleWithLocation)
                {
                    var markerOptions = new MarkerOptions();
                    var point = new LatLng(person.Location.Y, person.Location.X);
                    markerOptions.SetPosition(point);
                    markerOptions.SetTitle(person.Name);
                    _map.AddMarker(markerOptions);
                    bounds.Include(point);
                }
                _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds.Build(), 100));
            }
        }
    }
}
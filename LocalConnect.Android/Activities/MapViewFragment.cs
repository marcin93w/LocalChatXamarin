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
using LocalConnect.ViewModel;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect.Android.Activities
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback
    {
        private readonly PeopleViewModel _peopleViewModel;
        private GoogleMap _map;

        public MapViewFragment()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            var rootView = (ViewGroup)inflater.Inflate(
                    Resource.Layout.MapViewFragment, container, false);

            var mapFragment = (SupportMapFragment) ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

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
                    var point = new LatLng(person.Location.Lat, person.Location.Lon);
                    markerOptions.SetPosition(point);
                    markerOptions.SetTitle(person.Name);
                    _map.AddMarker(markerOptions);
                    bounds.Include(point);
                }
                if(peopleWithLocation.Any())
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds.Build(), 100));
            }
        }
    }
}
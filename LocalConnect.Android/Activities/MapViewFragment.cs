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
using LocalConnect.Android.Activities.Services;
using LocalConnect.Helpers;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using Newtonsoft.Json;
using Fragment = Android.Support.V4.App.Fragment;

namespace LocalConnect.Android.Activities
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback
    {
        private readonly PeopleViewModel _peopleViewModel;
        private GoogleMap _map;
        private LocationUpdateServiceConnection _locationUpdateServiceConnection;

        private Dictionary<Person, Marker> _markers;

        public MapViewFragment()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>(Activity);
            _markers = new Dictionary<Person, Marker>();
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
                    var marker = _map.AddMarker(markerOptions);
                    _markers[person] = marker;
                    bounds.Include(point);
                }
                if(peopleWithLocation.Any())
                    _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds.Build(), 100));

                BindToLocationUpdateService();
            }
        }

        public override void OnDestroyView()
        {
            if (_locationUpdateServiceConnection != null)
                Activity.UnbindService(_locationUpdateServiceConnection);
            base.OnDestroyView();
        }

        private void BindToLocationUpdateService()
        {
            if (_locationUpdateServiceConnection == null || !_locationUpdateServiceConnection.IsConnected)
            {
                var startLocationUpdateServiceIntent = new Intent(Activity, typeof (LocationUpdateService));
                startLocationUpdateServiceIntent.PutExtra("DataProvider",
                    JsonConvert.SerializeObject(_peopleViewModel.RestClient));

                _locationUpdateServiceConnection = new LocationUpdateServiceConnection(null);
                _locationUpdateServiceConnection.ServiceConnected +=
                    (sender, args) =>
                    {
                        args.ServiceBinder.Service.LocationChanged += OnLocationChanged;
                        if (args.ServiceBinder.Service.Location != null)
                            AddOrChangeMyLocation(args.ServiceBinder.Service.Location);
                    };
                Activity.BindService(startLocationUpdateServiceIntent, _locationUpdateServiceConnection, Bind.AutoCreate);
            }
        }

        private void OnLocationChanged(object sender, CurrentLocationChangedEventArgs args)
        {
            AddOrChangeMyLocation(args.Location);
        }

        private void AddOrChangeMyLocation(Location location)
        {
            _peopleViewModel.Me.Location = location;

            var markerOptions = new MarkerOptions();
            var point = new LatLng(location.Lat, location.Lon);
            markerOptions.SetPosition(point);
            markerOptions.SetTitle(_peopleViewModel.Me.Name); 
            var marker = _map.AddMarker(markerOptions);

            if (_markers.ContainsKey(_peopleViewModel.Me))
            {
                _markers[_peopleViewModel.Me].Remove();
            }

            _markers[_peopleViewModel.Me] = marker;
        }
    }
}
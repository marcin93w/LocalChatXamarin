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
using Android.Views;
using Android.Widget;
using LocalConnect.Helpers;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using Fragment = Android.Support.V4.App.Fragment;
using AndroidRes = global::Android.Resource;

namespace LocalConnect.Android.Activities
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback
    {
        private PeopleViewModel _peopleViewModel;
        private GoogleMap _map;

        private Dictionary<string, Marker> _markers;

        private BitmapDescriptor _myLocationIcon;

        public MapViewFragment()
        {
            _markers = new Dictionary<string, Marker>();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container,
            Bundle savedInstanceState)
        {
            var rootView = (ViewGroup)inflater.Inflate(
                    Resource.Layout.MapViewFragment, container, false);

            _peopleViewModel = ViewModelLocator.Instance.GetUiInvokableViewModel<PeopleViewModel>(Activity);

            _myLocationIcon = BitmapDescriptorFactory.FromResource(AndroidRes.Drawable.IcMenuMyLocation);

            var mapFragment = (SupportMapFragment) ChildFragmentManager.FindFragmentById(Resource.Id.map);
            mapFragment.GetMapAsync(this);

            return rootView;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            _map = googleMap;
            _map.MapType = GoogleMap.MapTypeHybrid;
            _peopleViewModel.OnPeopleLoaded += OnPeopleLoaded;
        }

        private void OnPeopleLoaded(object sender, OnDataLoadEventArgs e)
        {
            if (e.IsSuccesful)
            {
                var bounds = new LatLngBounds.Builder(); //TODO change camera move from bounds to my location center with all people visible (probably logic in VM)

                var myPoint = new LatLng(_peopleViewModel.Me.Location.Lat, _peopleViewModel.Me.Location.Lon);
                AddOrChangeMyLocation(myPoint);
                bounds.Include(myPoint);

                var peopleWithLocation = _peopleViewModel.People.Where(p => p.Location != null);
                foreach (var person in peopleWithLocation)
                {
                    var markerOptions = new MarkerOptions();
                    var point = new LatLng(person.Location.Lat, person.Location.Lon);
                    markerOptions.SetPosition(point);
                    markerOptions.SetTitle(person.Name);
                    var marker = _map.AddMarker(markerOptions);
                    if (_markers.ContainsKey(person.Id))
                    {
                        _markers[person.Id].Remove();
                    }
                    _markers[person.Id] = marker;
                    bounds.Include(point);
                }

                _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds.Build(), 100));

                _peopleViewModel.MyLocationChanged += OnLocationChanged;
            }
        }

        private void OnLocationChanged(object sender, EventArgs args)
        {
            AddOrChangeMyLocation(new LatLng(_peopleViewModel.Me.Location.Lat, _peopleViewModel.Me.Location.Lon));
        }

        private void AddOrChangeMyLocation(LatLng point)
        {
            var markerOptions = new MarkerOptions();
            markerOptions.SetPosition(point);
            markerOptions.SetIcon(_myLocationIcon);
            var marker = _map.AddMarker(markerOptions);

            if (_markers.ContainsKey(_peopleViewModel.Me.PersonId))
            {
                _markers[_peopleViewModel.Me.PersonId].Remove();
            }

            _markers[_peopleViewModel.Me.PersonId] = marker;
        }
    }
}
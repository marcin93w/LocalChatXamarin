using System;
using System.Collections.Generic;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using LocalConnect.Helpers;
using LocalConnect.ViewModel;
using AndroidRes = global::Android.Resource;

namespace LocalConnect.Android.Views
{
    public class MapViewFragment : Fragment, IOnMapReadyCallback
    {
        private PeopleViewModel _peopleViewModel;
        private GoogleMap _map;

        private Dictionary<string, Marker> _markers;
        private Dictionary<string, Circle> _circles;

        private BitmapDescriptor _myLocationIcon;

        public MapViewFragment()
        {
            _markers = new Dictionary<string, Marker>();
            _circles = new Dictionary<string, Circle>();
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

                var myPoint = new LatLng(_peopleViewModel.Me.RealLocation.Lat, _peopleViewModel.Me.RealLocation.Lon);
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

                    var circle = _map.AddCircle(new CircleOptions()
                        .InvokeCenter(point)
                        .InvokeRadius(person.Location.Tolerance)
                        .InvokeStrokeWidth(1)
                        .InvokeStrokeColor(GetColorWithAlpha(Color.Green, 200))
                        .InvokeFillColor(GetColorWithAlpha(Color.AntiqueWhite, 80)));
                    if (_circles.ContainsKey(person.Id))
                    {
                        _circles[person.Id].Remove();
                    }
                    _circles[person.Id] = circle;
                }

                _map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds.Build(), 100));

                _peopleViewModel.MyLocationChanged += OnLocationChanged;
            }
        }

        private Color GetColorWithAlpha(Color color, int alpha)
        {
            return Color.Argb(alpha, color.R, color.G, color.B);
        }

        private void OnLocationChanged(object sender, EventArgs args)
        {
            AddOrChangeMyLocation(new LatLng(_peopleViewModel.Me.RealLocation.Lat, _peopleViewModel.Me.RealLocation.Lon));
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
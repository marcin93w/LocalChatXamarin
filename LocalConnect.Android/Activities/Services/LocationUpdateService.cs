using System;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Widget;
using Java.Util.Logging;
using LocalConnect.Android.Activities.Helpers;
using LocalConnect.Services;
using Newtonsoft.Json;
using Location = LocalConnect.Models.Location;

namespace LocalConnect.Android.Activities.Services
{
    public class CurrentLocationChangedEventArgs : EventArgs
    {
        public CurrentLocationChangedEventArgs(Location location)
        {
            Location = location;
        }

        public Location Location { get; }
    }
    public class LocationStatusChangedEventArgs : EventArgs
    {
        public LocationStatusChangedEventArgs(string provider, Availability status, bool isEnabled)
        {
            Provider = provider;
            Status = status;
            IsEnabled = isEnabled;
        }

        public bool IsEnabled { get; }
        public string Provider { get; }
        public Availability Status { get; }
    }

    public delegate void CurrentLocationChangedEventHandler(object sender, CurrentLocationChangedEventArgs args);

    public delegate void LocationStatusChangedEventHandler(object sender, LocationStatusChangedEventArgs args);


    [Service]
    public class LocationUpdateService : Service, ILocationListener
    {
        private const long LocationUpdateTimeInterval = 1000 * 60; //in miliseconds
        private const float LocationUpdateMinDistance = 10; //in meters

        private RestClient _dataProvider;
        private readonly LocationManager _locMgr = Application.Context.GetSystemService("location") as LocationManager;

        public Location Location { private set; get; }
        public bool LocationUpdateActive { private set; get; }

        public event CurrentLocationChangedEventHandler LocationChanged;
        public event LocationStatusChangedEventHandler LocationProviderStatusChanged;

        public override IBinder OnBind(Intent intent)
        {
            return new LocationUpdateServiceBinder(this);
        }

#pragma warning disable 672, 618 //xamarin bug https://bugzilla.xamarin.com/show_bug.cgi?id=29684
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            
            _dataProvider = new RestClient(new AuthTokenManager(ApplicationContext));

            if(!LocationUpdateActive)
                StartLocationUpdates();

            return StartCommandResult.NotSticky;
        }
#pragma warning restore 672, 618

        private void StartLocationUpdates ()
        {
            LocationUpdateActive = true;
            var locationCriteria = new Criteria
            {
                Accuracy = Accuracy.Fine,
                PowerRequirement = Power.NoRequirement
            };

            var locationProvider = _locMgr.GetBestProvider(locationCriteria, true);
            if (locationProvider != null)
            {
                var lastKnownLocation = _locMgr.GetLastKnownLocation(locationProvider)
                                        ?? _locMgr.GetLastKnownLocation(LocationManager.NetworkProvider);
                if (lastKnownLocation != null)
                {
                    Location = new Location(lastKnownLocation.Longitude, lastKnownLocation.Latitude);
                    SendLocationUpdate();
                }
                _locMgr.RequestLocationUpdates(locationProvider, LocationUpdateTimeInterval, LocationUpdateMinDistance,
                    this);
            }
            else
            {
                LocationUpdateActive = false;
            }
        }

        private async void SendLocationUpdate()
        {
            try
            {
                await _dataProvider.PostDataAsync("me/updateLocation", Location);
            }
            catch (Exception)
            {
                Logger.Global.Log(Level.All, "Error updating location");
            }
        }

        public void OnLocationChanged(global::Android.Locations.Location location)
        {
            Location = new Location(location.Longitude, location.Latitude);
            SendLocationUpdate();
            LocationChanged?.Invoke(this, new CurrentLocationChangedEventArgs(Location));
        }

        public void OnProviderDisabled(string provider)
        {
            LocationProviderStatusChanged?.Invoke(this, new LocationStatusChangedEventArgs(provider, Availability.OutOfService, false));
            LocationUpdateActive = false;
        }

        public void OnProviderEnabled(string provider)
        {
            if (!LocationUpdateActive)
                StartLocationUpdates();
            LocationProviderStatusChanged?.Invoke(this, new LocationStatusChangedEventArgs(provider, Availability.Available, true));
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            LocationProviderStatusChanged?.Invoke(this, new LocationStatusChangedEventArgs(provider, status, true));
        }
    }
}
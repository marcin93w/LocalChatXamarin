using Android.OS;

namespace LocalConnect.Android.Views.Services
{
    public class LocationUpdateServiceBinder : Binder
    {
        public LocationUpdateService Service { get; }

        public bool IsBound { get; set; }

        public LocationUpdateServiceBinder(LocationUpdateService service)
        {
            this.Service = service;
        }
    }
}
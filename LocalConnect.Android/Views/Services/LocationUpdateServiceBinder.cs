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
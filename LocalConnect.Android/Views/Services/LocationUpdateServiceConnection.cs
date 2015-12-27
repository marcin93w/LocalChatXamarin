using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace LocalConnect.Android.Views.Services
{
    public delegate void ServiceConnectedEventHandler(object sender, ServiceConnectedEventArgs args);
    public delegate void ServiceDisconnectedEventHandler(object sender, EventArgs args);

    public class ServiceConnectedEventArgs : EventArgs
    {
        public ServiceConnectedEventArgs(LocationUpdateServiceBinder serviceBinder)
        {
            ServiceBinder = serviceBinder;
        }

        public LocationUpdateServiceBinder ServiceBinder { get; }
    }

    public class LocationUpdateServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private LocationUpdateServiceBinder _binder;
        public event ServiceConnectedEventHandler ServiceConnected;
        public event ServiceDisconnectedEventHandler ServiceDisconnected;

        public bool IsConnected => _binder?.IsBound ?? false;

        public LocationUpdateServiceConnection(LocationUpdateServiceBinder binder)
        {
            if (binder != null)
            {
                this._binder = binder;
            }
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as LocationUpdateServiceBinder;

            if (serviceBinder != null)
            {
                this._binder = serviceBinder;
                this._binder.IsBound = true;
                
                this.ServiceConnected?.Invoke(this, new ServiceConnectedEventArgs(serviceBinder));
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this._binder.IsBound = false;
            this.ServiceDisconnected?.Invoke(this, EventArgs.Empty);
        }
    }
}
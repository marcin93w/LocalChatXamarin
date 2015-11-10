using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Java.Util.Logging;
using LocalConnect.Models;
using LocalConnect.Helpers;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PeopleViewModel : ViewModelBase, IRestClientUsingViewModel, ISocketClientUsingViewModel
    {
        private readonly People _people;

        public List<PersonViewModel> People { get; }
        public Me Me { get; }

        public event EventHandler MyLocationChanged
        {
            add { Me.LocationChanged += value; }
            remove { Me.LocationChanged -= value; }
        }

        private bool _dataLoaded;
        private string _errorMessage;

        private OnDataLoadEventHandler _onDataLoad;
        public IRestClient RestClient { get; set; }
        public ISocketClient SocketClient { private get; set; }

        public event OnDataLoadEventHandler OnDataLoad
        {
            add
            {
                if (_dataLoaded)
                {
                    value(this, new OnDataLoadEventArgs(_errorMessage));
                }
                _onDataLoad += value;
            }
            remove
            {
                _onDataLoad -= value;
            }
        }

        public PeopleViewModel()
        {
            _people = new People();
            People = new List<PersonViewModel>();
            Me = new Me();
        }

        public async void FetchDataAsync()
        {
            _errorMessage = null;
            bool authTokenMissing = false;
            try
            {
                var fetchPeopleTask = _people.FetchPeopleList(RestClient);
                var fetchMeTask = Me.FetchData(RestClient);

                await Task.WhenAll(fetchPeopleTask, fetchMeTask);

                People.AddRange(_people.PeopleList.ConvertAll(p => new PersonViewModel(p, Me)));
            }
            catch (Exception ex)
            {
                _errorMessage = ex.Message;
                authTokenMissing = (ex as ConnectionException)?.IsAuthTokenMissing ?? false;
            }
            finally
            {
                _dataLoaded = true;
                _onDataLoad?.Invoke(this, new OnDataLoadEventArgs(_errorMessage, authTokenMissing));
            }
        }

        public async Task SendLocationUpdate(Location location)
        {
            await Me.UpdateLocation(RestClient, location);
        }
    }
}
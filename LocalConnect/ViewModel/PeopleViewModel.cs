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

        public bool DataLoaded { get; private set; }
        public string ErrorMessage { get; set; }

        private OnDataLoadEventHandler _onDataLoad;
        public IRestClient RestClient { get; set; }
        public ISocketClient SocketClient { get; set; }

        public event OnDataLoadEventHandler OnPeopleLoaded
        {
            add
            {
                if (DataLoaded)
                {
                    value(this, new OnDataLoadEventArgs(ErrorMessage));
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

        public async Task<bool> FetchMyDataAsync()
        {
            try
            {
                await Me.FetchData(RestClient);
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
        }

        public async void FetchPeopleData()
        {
            bool authTokenMissing = false;
            try
            {
                await _people.FetchPeopleList(RestClient);
                People.Clear();
                People.AddRange(_people.PeopleList.ConvertAll(p => new PersonViewModel(p, Me)));
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                DataLoaded = true;
                _onDataLoad?.Invoke(this, new OnDataLoadEventArgs(ErrorMessage, authTokenMissing)); //TODO when authtoken expires
            }
        }

        public async Task SendLocationUpdate(Location location)
        {
            await Me.UpdateLocation(RestClient, location);
        }
    }
}
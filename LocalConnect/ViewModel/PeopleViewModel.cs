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
        private Task _peopleLoadingTask;

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
            SetUpMessagesListener();
            bool authTokenMissing = false;
            try
            {
                _peopleLoadingTask = _people.FetchPeopleList(RestClient);
                await _peopleLoadingTask;
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

        private void SetUpMessagesListener()
        {
            SocketClient.OnMessageReceived += OnMessageReceived;
        }

        private async void OnMessageReceived(object sender, MessageReceivedEventArgs receivedEventArgs)
        {
            await _peopleLoadingTask;
            var person = People.FirstOrDefault((p) => receivedEventArgs.Message.SenderId == p.Id);
            if (person != null)
            {
                person.UnreadMessages = (person.UnreadMessages ?? 0) + 1;
            }
            else
            {
                //fetch that person
            }
        }

        public async Task SendLocationUpdate(Location location)
        {
            await Me.UpdateLocation(RestClient, location);
        }
    }
}
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
    public class PeopleViewModel : ViewModelBase, IRestClientUsingViewModel, ISocketClientUsingViewModel, IUiInvokable
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

        private OnDataLoadEventHandler _onPeopleLoad;
        private Task _peopleLoadingTask;
        private bool _messagesListenerAssigned;

        public IRestClient RestClient { get; set; }
        public ISocketClient SocketClient { get; set; }
        public RunOnUiThreadHandler RunOnUiThread { private get; set; }

        public event OnDataLoadEventHandler OnPeopleLoaded
        {
            add
            {
                if (DataLoaded)
                {
                    value(this, new OnDataLoadEventArgs(ErrorMessage));
                }
                _onPeopleLoad += value;
            }
            remove
            {
                _onPeopleLoad -= value;
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
                _peopleLoadingTask = _people.FetchPeopleList(RestClient);
                SetUpMessagesListener();
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
                _onPeopleLoad?.Invoke(this, new OnDataLoadEventArgs(ErrorMessage, authTokenMissing)); //TODO when authtoken expires
            }
        }

        private void SetUpMessagesListener()
        {
            if (!_messagesListenerAssigned)
            {
                _messagesListenerAssigned = true;
                SocketClient.OnMessageReceived += OnMessageReceived;
            }
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
                var newPerson = await Person.LoadPerson(RestClient, receivedEventArgs.Message.SenderId);
                person = new PersonViewModel(newPerson, Me);
                _people.PeopleList.Add(newPerson);
                People.Add(person);
                RunOnUiThread(() => _onPeopleLoad?.Invoke(this, new OnDataLoadEventArgs()));
            }
        }

        public async Task SendLocationUpdate(Location location)
        {
            await Me.UpdateLocation(RestClient, location);
        }
    }
}
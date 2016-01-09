using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Java.Util.Logging;
using LocalConnect.Models;
using LocalConnect.Helpers;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PeopleViewModel : ViewModelBase, IUiInvokable
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

        public IRestClient RestClient { get; }
        public ISocketClient SocketClient { get; }
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

        public PeopleViewModel(IRestClient restClient, ISocketClient socketClient)
        {
            RestClient = restClient;
            SocketClient = socketClient;
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
            ErrorMessage = null;
            bool isUnauthorized = false;
            try
            {
                _peopleLoadingTask = _people.FetchPeopleList(RestClient);
                SetUpMessagesListener();
                await _peopleLoadingTask;

                var people = _people.PeopleList
                    .ConvertAll(p => new PersonViewModel(p, Me))
                    .OrderByDescending(p => p.UnreadMessages.HasValue)
                    .ThenBy(p => p.Distance);
                People.Clear();
                People.AddRange(people);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                if (ex is MissingAuthenticationTokenException)
                    isUnauthorized = true;
                if (((ex as WebException)?.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized)
                {
                    isUnauthorized = true;
                }
            }
            finally
            {
                DataLoaded = true;
                _onPeopleLoad?.Invoke(this, new OnDataLoadEventArgs(ErrorMessage, isUnauthorized));
            }
        }

        private void SetUpMessagesListener()
        {
            SocketClient.OnMessageReceived -= OnMessageReceived;
            SocketClient.OnMessageReceived += OnMessageReceived;
        }

        private async void OnMessageReceived(object sender, MessageReceivedEventArgs receivedEventArgs)
        {
            await _peopleLoadingTask;
            var person = FindPerson(receivedEventArgs.Message.SenderId);
            if (person == null)
            {
                var newPerson = await Person.LoadPerson(RestClient, receivedEventArgs.Message.SenderId);
                lock (_people)
                {
                    person = FindPerson(receivedEventArgs.Message.SenderId);
                    if (person == null)
                    {
                        person = new PersonViewModel(newPerson, Me);
                        person.UnreadMessages = 1;
                        _people.PeopleList.Add(newPerson);
                        People.Insert(0, person);
                        RunOnUiThread(() => _onPeopleLoad?.Invoke(this, new OnDataLoadEventArgs()));
                    }
                    else
                    {
                        person.UnreadMessages = (person.UnreadMessages ?? 0) + 1;
                    }
                }
            }
            else
            {
                person.UnreadMessages = (person.UnreadMessages ?? 0) + 1;
            }
        }

        private PersonViewModel FindPerson(string personId)
        {
            return People.FirstOrDefault(p => personId == p.Id);
        }

        public async Task SendLocationUpdate(Location location)
        {
            await Me.UpdateLocation(RestClient, location);
        }
    }
}
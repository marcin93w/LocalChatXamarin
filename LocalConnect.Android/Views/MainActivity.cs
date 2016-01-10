using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using LocalConnect.Android.Views.Adapters;
using LocalConnect.Android.Views.Helpers;
using LocalConnect.Android.Views.Services;
using LocalConnect.Helpers;
using LocalConnect.ViewModel;
using Newtonsoft.Json;
using Square.Picasso;
using AndroidRes = Android.Resource;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LocalConnect.Android.Views
{
    [Activity(MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private enum LoadingInfoState
        {
            LoadingData,
            CheckingLocation,
            LoadingPeople,
            GpsError,
            NetworkError,
            Disabled
        }

        private LocationUpdateServiceConnection _locationUpdateServiceConnection;
        private readonly PeopleViewModel _peopleViewModel;

        private ViewPager _viewPager;
        private ViewGroup _loadingInfoPanel;
        private ImageView _loadingInfoIcon;
        private TextView _loadingInfoTextView;

        private LoadingInfoState _loadingInfoState = LoadingInfoState.CheckingLocation;

        private Task<bool> _loadingMyDataTask;
        private bool _myDataNeedsReload;
        private IMenuItem _switchViewAction;

        public MainActivity()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetUiInvokableViewModel<PeopleViewModel>(this);
        }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            var myToolbar = FindViewById<Toolbar>(Resource.Id.ActionBar);
            SetSupportActionBar(myToolbar);
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = new MainViewsPagerAdapter(SupportFragmentManager,
                    new ListViewFragment(), new MapViewFragment());
            _viewPager.PageSelected += OnViewChanged;

            _loadingInfoPanel = FindViewById<ViewGroup>(Resource.Id.LoadingInfoPanel);
            _loadingInfoTextView = FindViewById<TextView>(Resource.Id.LoadingInfoText);
            _loadingInfoIcon = FindViewById<ImageView>(Resource.Id.LoadingInfoIcon);

            _peopleViewModel.OnPeopleLoaded += OnPeopleLoaded;

            if (!_peopleViewModel.RestClient.IsAuthenticated())
            {
                OpenLoginActivity();
                return;
            }

            
            if (savedInstanceState == null || !_peopleViewModel.DataLoaded || _myDataNeedsReload)
            {
                _loadingMyDataTask = _peopleViewModel.FetchMyDataAsync();

                CreateLocationUpdateService();
                BindToLocationUpdateService(true);
            }
            else
            {
                LoadingInfoState savedState;
                if(Enum.TryParse(savedInstanceState.GetString("LoadingInfoState"), false, out savedState))
                    ChangeLoadingInfoState(savedState);

                BindToLocationUpdateService(false);
            }

            if (!_peopleViewModel.SocketClient.IsConnected)
            {
                if (!_peopleViewModel.SocketClient.Connect())
                {
                    ChangeLoadingInfoState(LoadingInfoState.NetworkError);
                    return;
                }
            }

            if (_loadingMyDataTask != null && !await _loadingMyDataTask)
            {
                _myDataNeedsReload = true;
                ChangeLoadingInfoState(LoadingInfoState.NetworkError);
                return;
            }

            OnMyDataLoaded();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString("LoadingInfoState", _loadingInfoState.ToString());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_locationUpdateServiceConnection != null)
                ApplicationContext.UnbindService(_locationUpdateServiceConnection);
        }

        private void OnMyDataLoaded()
        {
            _myDataNeedsReload = false;
            var meImage = FindViewById<ImageView>(Resource.Id.MeImage);
            if (!string.IsNullOrEmpty(_peopleViewModel.Me.Avatar))
            {
                Picasso.With(this)
                    .Load(_peopleViewModel.Me.Avatar)
                    .Into(meImage);
            }
            meImage.Click += OpenMyProfileView;
        }

        private void CreateLocationUpdateService()
        {
            var startLocationUpdateServiceIntent = new Intent(this, typeof(LocationUpdateService));
            ApplicationContext.StartService(startLocationUpdateServiceIntent);
        }

        private void BindToLocationUpdateService(bool attachEvent)
        {
            if (_locationUpdateServiceConnection == null || !_locationUpdateServiceConnection.IsConnected)
            {
                var startLocationUpdateServiceIntent = new Intent(this, typeof(LocationUpdateService));

                _locationUpdateServiceConnection = new LocationUpdateServiceConnection(null);
                if(attachEvent)
                    _locationUpdateServiceConnection.ServiceConnected += OnLocationUpdateServiceConnected;
                ApplicationContext.BindService(startLocationUpdateServiceIntent, _locationUpdateServiceConnection, Bind.AutoCreate);
            }
        }

        private async void OnLocationUpdateServiceConnected(object sender, ServiceConnectedEventArgs args)
        {
            if (args.ServiceBinder.Service.Location == null)
            {
                ChangeLoadingInfoState(LoadingInfoState.GpsError);
            }
            else
            {
                if (_loadingMyDataTask != null && !await _loadingMyDataTask)
                    return;
                try
                {
                    await _peopleViewModel.SendLocationUpdate(args.ServiceBinder.Service.Location);
                }
                catch (Exception ex)
                {
                    ChangeLoadingInfoState(LoadingInfoState.NetworkError);
                    return;
                }
                FetchPeople();
            }
        }

        private void FetchPeople()
        {
            if (_peopleViewModel.Me.RealLocation != null)
            {
                ChangeLoadingInfoState(LoadingInfoState.LoadingPeople);
                _peopleViewModel.FetchPeopleData();
            }
            else
            {
                BindToLocationUpdateService(true);
            }
        }

        private void OnPeopleLoaded(object sender, OnDataLoadEventArgs e)
        {
            if (!e.IsSuccesful)
            {
                ChangeLoadingInfoState(LoadingInfoState.NetworkError);
            }
            else
            {
                ChangeLoadingInfoState(LoadingInfoState.Disabled);
            }
        }

        private void ChangeLoadingInfoState(LoadingInfoState state)
        {
            _loadingInfoState = state;

            _loadingInfoPanel.Visibility = ViewStates.Visible;
            _loadingInfoPanel.SetBackgroundResource(AndroidRes.Color.HoloGreenLight);
            _loadingInfoIcon.SetImageResource(AndroidRes.Drawable.IcMenuInfoDetails);
            switch (_loadingInfoState)
            {
                case LoadingInfoState.CheckingLocation:
                    break;
                case LoadingInfoState.LoadingData:
                    _loadingInfoTextView.Text = "Loading data from server...";
                    break;
                case LoadingInfoState.LoadingPeople:
                    _loadingInfoTextView.Text = "Searching nearby poeple...";
                    break;
                case LoadingInfoState.GpsError:
                    _loadingInfoPanel.SetBackgroundColor(Color.Red);
                    _loadingInfoIcon.SetImageResource(AndroidRes.Drawable.PresenceOffline);
                    _loadingInfoTextView.Text = "Could not determine location, please turn on GPS.";
                    break;
                case LoadingInfoState.NetworkError:
                    _loadingInfoPanel.SetBackgroundColor(Color.Red);
                    _loadingInfoIcon.SetImageResource(AndroidRes.Drawable.PresenceOffline);
                    _loadingInfoTextView.Text = "Error connecting to server, please check internet connection.";
                    break;
                case LoadingInfoState.Disabled:
                    _loadingInfoPanel.Visibility = ViewStates.Gone;
                    break;
            }
        }

        private async void DataRefreshRequested(object sender, EventArgs eventArgs)
        {
            if (!_peopleViewModel.SocketClient.IsConnected)
            {
                if (!_peopleViewModel.SocketClient.Connect())
                {
                    ChangeLoadingInfoState(LoadingInfoState.NetworkError);
                    return;
                }
            }
            if (_myDataNeedsReload)
            {
                ChangeLoadingInfoState(LoadingInfoState.LoadingData);
                if (!await _peopleViewModel.FetchMyDataAsync())
                {
                    ChangeLoadingInfoState(LoadingInfoState.NetworkError);
                    return;
                }
                OnMyDataLoaded();
            }
                
            FetchPeople();
        }

        private void OnSwitchViewCicked(object sender, EventArgs e)
        {
            if (_viewPager.CurrentItem == 0)
            {
                //is on list view
                _viewPager.SetCurrentItem(1, true);
            }
            else
            {
                //is on map view
                _viewPager.SetCurrentItem(0, true);
            }
        }

        private void OnViewChanged(object sender, ViewPager.PageSelectedEventArgs e)
        {
            if (_viewPager.CurrentItem == 1)
            {
                //is on map view
                _switchViewAction.SetIcon(Resource.Drawable.ic_view_list_white_36dp);
            }
            else
            {
                //is on list view
                _switchViewAction.SetIcon(AndroidRes.Drawable.IcDialogMap);
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.ActionMenu, menu);
            _switchViewAction = menu.FindItem(Resource.Id.ActionSwitchView);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.ActionLogout:
                    Logout();
                    break;
                case Resource.Id.ActionSwitchView:
                    OnSwitchViewCicked(null, EventArgs.Empty);
                    break;
                case Resource.Id.ActionRefresh:
                    DataRefreshRequested(null, EventArgs.Empty);
                    break;
                case Resource.Id.ActionSettings:
                    var intent = new Intent(ApplicationContext, typeof(SettingsActivity));
                    StartActivity(intent);
                    break;
            }
            return base.OnOptionsItemSelected(item);
        }

        private void Logout()
        {
            _peopleViewModel.SocketClient.Disconnect();
            var tokenManager = new SessionInfoManager(this);
            tokenManager.DeleteSessionInfo();
            ViewModelLocator.Instance.ResetViewModel<PeopleViewModel>();
            OpenLoginActivity();
        }

        private void OpenLoginActivity()
        {
            var loginActivity = new Intent(ApplicationContext, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }

        private void OpenMyProfileView(object sender, EventArgs e)
        {
            var myProfileActivity = new Intent(ApplicationContext, typeof(MyProfileActivity));
            myProfileActivity.PutExtra("Me", JsonConvert.SerializeObject(_peopleViewModel.Me));
            StartActivity(myProfileActivity);
        }
    }
}


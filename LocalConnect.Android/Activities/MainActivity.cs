using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using LocalConnect.Android.Activities.Adapters;
using LocalConnect.Android.Activities.Helpers;
using LocalConnect.Android.Activities.Services;
using LocalConnect.Helpers;
using LocalConnect.ViewModel;
using Newtonsoft.Json;
using Org.Apache.Http.Impl.Conn;
using Square.Picasso;
using AndroidRes = Android.Resource;

namespace LocalConnect.Android.Activities
{
    [Activity(MainLauncher = true)]
    public class MainActivity : FragmentActivity
    {
        private readonly PeopleViewModel _peopleViewModel;

        private ViewPager _viewPager;

        public MainActivity()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = new MainViewsPagerAdapter(SupportFragmentManager,
                    new ListViewFragment(), new MapViewFragment());
            _viewPager.PageSelected += OnViewChanged;

            var switchViewButton = FindViewById<ImageButton>(Resource.Id.SwitchViewButton);
            switchViewButton.Click += OnSwitchViewCicked;
            var settingsButton = FindViewById<ImageButton>(Resource.Id.MenuButton);
            settingsButton.Click += (sender, e) => OnSettingsClick(settingsButton);

            _peopleViewModel.OnDataLoad += OnDataLoad;

            if (bundle == null)
            {
                if (!_peopleViewModel.RestClient.IsAuthenticated())
                {
                    OpenLoginActivity();
                    return;
                }

                _peopleViewModel.FetchDataAsync();
            }
        }

        private void OnViewChanged(object sender, ViewPager.PageSelectedEventArgs e)
        {
            var switchViewButton = FindViewById<ImageButton>(Resource.Id.SwitchViewButton);
            if (_viewPager.CurrentItem == 1)
            {
                //is on map view
                switchViewButton.SetImageResource(Resource.Drawable.ic_view_list_white_36dp);
            }
            else
            {
                //is on list view
                switchViewButton.SetImageResource(AndroidRes.Drawable.IcDialogMap);
            }
        }

        private void OnDataLoad(object sender, OnDataLoadEventArgs e)
        {
            if (e.ApplicationNotInitialized)
            {
                OpenLoginActivity();
                return;
            }
            if (!e.IsSuccesful)
            {
                Toast.MakeText(this, e.ErrorMessage, ToastLength.Long).Show();
            }
            else
            {
                CreateLocationUpdateService();

                if (!string.IsNullOrEmpty(_peopleViewModel.Me.Avatar))
                {
                    var meImage = FindViewById<ImageView>(Resource.Id.MeImage);
                    Picasso.With(this)
                        .Load(_peopleViewModel.Me.Avatar)
                        .Into(meImage);
                }
            }
        }

        private void CreateLocationUpdateService()
        {
            var startLocationUpdateServiceIntent = new Intent(this, typeof(LocationUpdateService));
            StartService(startLocationUpdateServiceIntent);
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

        public void OnSettingsClick(View v)
        {
            PopupMenu popup = new PopupMenu(this, v);
            popup.MenuItemClick += PopupOnMenuItemClick;
            popup.MenuInflater.Inflate(Resource.Menu.SettingsMenu, popup.Menu);
            popup.Show();
        }

        private void PopupOnMenuItemClick(object sender, PopupMenu.MenuItemClickEventArgs menuItemClickEventArgs)
        {
            switch (menuItemClickEventArgs.Item.ItemId)
            {
                case Resource.Id.logout:
                    Logout();
                    break;
            }
        }

        private void Logout()
        {
            var tokenManager = new AuthTokenManager(this);
            tokenManager.DeleteAuthToken();
            ViewModelLocator.Instance.ResetViewModel<PeopleViewModel>();
            OpenLoginActivity();
        }

        private void OpenLoginActivity()
        {
            var loginActivity = new Intent(ApplicationContext, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }
    }
}


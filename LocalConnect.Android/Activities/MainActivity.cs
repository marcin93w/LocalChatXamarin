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
using LocalConnect.ViewModel;
using Org.Apache.Http.Impl.Conn;
using AndroidRes = Android.Resource;

namespace LocalConnect.Android.Activities
{
    [Activity]
    public class MainActivity : FragmentActivity
    {
        private readonly PeopleViewModel _peopleViewModel;

        private ViewPager _viewPager;

        public MainActivity()
        {
            _peopleViewModel = ViewModelLocator.Instance.GetViewModel<PeopleViewModel>();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            _peopleViewModel.OnDataLoad += OnDataLoad;
            _peopleViewModel.FetchDataAsync();

            _viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            _viewPager.Adapter = new MainViewsPagerAdapter(SupportFragmentManager,
                new ListViewFragment(), new MapViewFragment());
            _viewPager.PageSelected += OnViewChanged;

            var switchViewButton = FindViewById<ImageButton>(Resource.Id.SwitchViewButton);
            switchViewButton.Click += OnSwitchViewCicked;
            var settingsButton = FindViewById<ImageButton>(Resource.Id.MenuButton);
            settingsButton.Click += (sender, e) => OnSettingsClick(settingsButton);
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
            }
            if (!e.IsSuccesful)
            {
                Toast.MakeText(this, e.ErrorMessage, ToastLength.Long).Show();
            }
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
            DeleteAuthToken();
            OpenLoginActivity();
        }

        private void DeleteAuthToken()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var editor = prefs.Edit();
            editor.Remove("auth_token");
            editor.Apply();
        }

        private void OpenLoginActivity()
        {
            var loginActivity = new Intent(ApplicationContext, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }
    }
}


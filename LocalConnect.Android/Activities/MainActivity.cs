using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using LocalConnect.Android.Activities.Adapters;
using LocalConnect.ViewModel;
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

            var switchViewButton = FindViewById<ImageButton>(Resource.Id.SwitchViewButton);
            switchViewButton.Click += OnSwitchViewCicked;
        }

        private void OnDataLoad(object sender, OnDataLoadEventArgs e)
        {
            if (!e.IsSuccesful)
            {
                Toast.MakeText(this, e.ErrorMessage, ToastLength.Long).Show();
            }
        }

        private void OnSwitchViewCicked(object sender, EventArgs e)
        {
            var switchViewButton = FindViewById<ImageButton>(Resource.Id.SwitchViewButton);
            if (_viewPager.CurrentItem == 0)
            {
                //is on list view
                _viewPager.SetCurrentItem(1, true);
                switchViewButton.SetImageResource(Resource.Drawable.ic_view_list_white_36dp);
            }
            else
            {
                //is on map view
                _viewPager.SetCurrentItem(0, true);
                switchViewButton.SetImageResource(AndroidRes.Drawable.IcDialogMap);
            }
        }

        private void OpenLoginActivity()
        {
            var loginActivity = new Intent(ApplicationContext, typeof(LoginActivity));
            StartActivity(loginActivity);
            Finish();
        }
    }
}


using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Widget;
using LocalConnect.Android.Activities.Adapters;
using LocalConnect.ViewModel;

namespace LocalConnect.Android.Activities
{
    [Activity]
    public class MainActivity : FragmentActivity
    {
        private readonly PeopleViewModel _peopleViewModel;

        public ViewPager ViewPager { private set; get; }

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

            ViewPager = FindViewById<ViewPager>(Resource.Id.pager);
            ViewPager.Adapter = new MainViewsPagerAdapter(SupportFragmentManager,
                new ListViewFragment(), new MapViewFragment());
        }

        private void OnDataLoad(object sender, OnDataLoadEventArgs e)
        {
            if (e.IsSuccesful)
            {
                var myName = FindViewById<TextView>(Resource.Id.MeName);
                myName.Text = _peopleViewModel.MyName;
            }
            else
            {
                Toast.MakeText(this, e.ErrorMessage, ToastLength.Long).Show();
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


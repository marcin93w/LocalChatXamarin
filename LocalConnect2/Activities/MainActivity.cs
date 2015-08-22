using System;
using Android.App;
using Android.Gms.Maps;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;

namespace LocalConnect2.Activities
{
    [Activity(
        Label = "LocalConnect2", 
        MainLauncher = true, 
        Icon = "@drawable/icon", 
        Theme = "@android:style/Theme.Black.NoTitleBar")]
    public class MainActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var viewPager = FindViewById<ViewPager>(Resource.Id.pager);
            viewPager.Adapter = new MainViewsPagerAdapter(SupportFragmentManager,
                new ListViewFragment(), new MapViewFragment(viewPager));
        }
    }
}


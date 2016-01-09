using Android.Support.V4.App;

namespace LocalConnect.Android.Views.Adapters
{
    public class MainViewsPagerAdapter : FragmentPagerAdapter
    {
        private readonly ListViewFragment _listViewFragment;
        private readonly MapViewFragment _mapViewFragment;

        public MainViewsPagerAdapter(FragmentManager fm, 
            ListViewFragment listViewFragment, MapViewFragment mapViewFragment) : base(fm)
        {
            _listViewFragment = listViewFragment;
            _mapViewFragment = mapViewFragment;
        }

        public override int Count => 2;

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0: return _listViewFragment;
                case 1: return _mapViewFragment;
                default: return null;
            }
        }
    }
}
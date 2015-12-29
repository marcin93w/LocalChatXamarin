using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace LocalConnect.Android.Views
{
    [Activity(Label = "SettingsActivity", ParentActivity = typeof(MainActivity))]
    public class SettingsActivity : AppCompatActivity
    {
        private Binding<string, string> _peopleCountBinding;
        private TextView _peopleCountInput;
        private Spinner _locationDisruptionSpinner;

        public SettingsActivity()
        {
            SettingsViewModel = ViewModelLocator.Instance.GetViewModel<SettingsViewModel>(this);
        }

        private SettingsViewModel SettingsViewModel { get; }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Settings);

            var myToolbar = FindViewById<Toolbar>(Resource.Id.ActionBar);
            SetSupportActionBar(myToolbar);
            SupportActionBar.Title = GetString(Resource.String.Settings);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            var loadingPanel = FindViewById<ViewGroup>(Resource.Id.LoadingPanel);
            loadingPanel.Visibility = ViewStates.Visible;
            await SettingsViewModel.LoadSettings();

            _peopleCountInput = FindViewById<TextView>(Resource.Id.SettingsPoepleCountInput);
            _locationDisruptionSpinner = FindViewById<Spinner>(Resource.Id.LocationDisruptionSpinner);

            CreateBindings();

            //var reloadButton = FindViewById<Button>(Resource.Id.SettingsReload);
            //reloadButton.Click += async (sender, args) => await SettingsViewModel.LoadSettings();

            var saveButton = FindViewById<Button>(Resource.Id.SettingsSave);
            saveButton.Click += OnSaveClick;

            if (SettingsViewModel.LoadingError)
            {
                var errorPanel = FindViewById<ViewGroup>(Resource.Id.SettingsConnectionError);
                errorPanel.Visibility = ViewStates.Visible;
            }
            else
            {
                _peopleCountInput.Text = SettingsViewModel.PeopleDisplayCountString;
                if (SettingsViewModel.LocationDisruption != null)
                    _locationDisruptionSpinner.SetSelection(
                        SettingsViewModel.DisruptionValues.IndexOf(SettingsViewModel.LocationDisruption.Value));
            }
            loadingPanel.Visibility = ViewStates.Gone;
        }

        private void CreateBindings()
        {

            _peopleCountBinding = this.SetBinding(
              () => SettingsViewModel.PeopleDisplayCountString,
              _peopleCountInput, () => _peopleCountInput.Text,
              BindingMode.TwoWay);
            
            _locationDisruptionSpinner.Adapter = new ArrayAdapter(this, 
                global::Android.Resource.Layout.SimpleSpinnerItem, SettingsViewModel.DisruptionValues.ConvertAll(ConvertDistanceToString));
            _locationDisruptionSpinner.ItemSelected +=
                (sender, args) => SettingsViewModel.LocationDisruption = SettingsViewModel.DisruptionValues[args.Position];
        }

        private string ConvertDistanceToString(int distance)
        {
            return distance < 1000 ? distance + " m" : (distance/1000) + "km";
        }

        private async void OnSaveClick(object sender, EventArgs eventArgs)
        {
            var errorPanel = FindViewById<TextView>(Resource.Id.SettingsConnectionError);
            errorPanel.Visibility = ViewStates.Gone;
            var loadingPanel = FindViewById<ViewGroup>(Resource.Id.LoadingPanel);
            loadingPanel.Visibility = ViewStates.Visible;

            await SettingsViewModel.UpdateSettings();
            if (!SettingsViewModel.SendingError)
            {
                Finish();
            }
            else
            {
                errorPanel.Visibility = ViewStates.Visible;
                loadingPanel.Visibility = ViewStates.Gone;
            }
        }
    }
}
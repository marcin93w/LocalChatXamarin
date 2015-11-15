using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using Newtonsoft.Json;
using Square.Picasso;

namespace LocalConnect.Android.Activities
{
    [Activity]
    public class MyProfileActivity : Activity
    {
        private Binding<string, string> _nameBinding;
        private Binding<string, string> _surnameBinding;
        private Binding<string, string> _shortDescriptionBinding;
        private Binding<string, string> _longDescriptionBinding;

        public MyProfileActivity()
        {
            MyProfileViewModel = ViewModelLocator.Instance.GetViewModel<MyProfileViewModel>(this);
        }

        private MyProfileViewModel MyProfileViewModel { get; }

        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MyProfile);

            if (!MyProfileViewModel.IsInitialized)
            {
                var me = JsonConvert.DeserializeObject<Me>(Intent.GetStringExtra("Me"));
                MyProfileViewModel.Initialize(me);
            }
            Task<bool> dataLoading = null;
            if (!MyProfileViewModel.DataLoaded)
            {
                dataLoading = MyProfileViewModel.LoadData();
            }

            CreateBindings();

            var cancelButton = FindViewById<ImageButton>(Resource.Id.ProfileCancelButton);
            cancelButton.Click += (sender, args) => Finish();

            var saveButton = FindViewById<Button>(Resource.Id.ProfileSaveButton);
            saveButton.Click += OnSaveClick;

            var profileAvatar = FindViewById<ImageView>(Resource.Id.ProfileAvatar);
            Picasso.With(this)
                    .Load(MyProfileViewModel.Avatar)
                    .Into(profileAvatar);

            if (dataLoading != null)
            {
                if (!await dataLoading)
                {
                    var errorPanel = FindViewById<ViewGroup>(Resource.Id.ProfileErrorPanel);
                    errorPanel.Visibility = ViewStates.Visible;
                    var errorText = FindViewById<TextView>(Resource.Id.ProfileErrorText);
                    errorText.Text = "Could not connect to server";
                }
                else
                {
                    var longDescriptionInput = FindViewById<TextView>(Resource.Id.ProfileLongDescription);
                    longDescriptionInput.Text = MyProfileViewModel.LongDescription;
                }
            }
        }

        private void CreateBindings()
        {
            var nameInput = FindViewById<TextView>(Resource.Id.ProfileName);
            var surnameInput = FindViewById<TextView>(Resource.Id.ProfileSurname);
            var shortDescriptionInput = FindViewById<TextView>(Resource.Id.ProfileShortDescription);
            var longDescriptionInput = FindViewById<TextView>(Resource.Id.ProfileLongDescription);

            _nameBinding = this.SetBinding(
              () => MyProfileViewModel.FirstName,
              nameInput, () => nameInput.Text,
              BindingMode.TwoWay);

            _surnameBinding = this.SetBinding(
              () => MyProfileViewModel.Surname,
              surnameInput, () => surnameInput.Text,
              BindingMode.TwoWay);

            _shortDescriptionBinding = this.SetBinding(
              () => MyProfileViewModel.ShortDesription,
              shortDescriptionInput, () => shortDescriptionInput.Text,
              BindingMode.TwoWay);

            _longDescriptionBinding = this.SetBinding(
              () => MyProfileViewModel.LongDescription,
              longDescriptionInput, () => longDescriptionInput.Text,
              BindingMode.TwoWay);
        }

        private async void OnSaveClick(object sender, EventArgs eventArgs)
        {
            var errorPanel = FindViewById<ViewGroup>(Resource.Id.ProfileErrorPanel);
            errorPanel.Visibility = ViewStates.Gone;
            var loadingPanel = FindViewById<ViewGroup>(Resource.Id.LoadingPanel);
            loadingPanel.Visibility = ViewStates.Visible;

            if (await MyProfileViewModel.Save())
            {
                Finish();
            }
            else
            {
                errorPanel.Visibility = ViewStates.Visible;
                loadingPanel.Visibility = ViewStates.Gone;
                var errorText = FindViewById<TextView>(Resource.Id.ProfileErrorText);
                errorText.Text = "Could not connect to server";
            }
        }
    }
}
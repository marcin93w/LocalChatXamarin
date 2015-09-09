using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect2.ViewModel;

namespace LocalConnect2.Activities
{
    [Activity(Label = "Aplikacja Marcina",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Black.NoTitleBar")]
    public class LoginActivity : Activity
    {
        private readonly LoginViewModel _loginViewModel;

        private View _loadingPanel;

        public LoginActivity()
        {
            _loginViewModel = ViewModelLocator.Instance.GetViewModel<LoginViewModel>();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            _loadingPanel = FindViewById(Resource.Id.LoadingPanel);

            var loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += Login;
        }

        protected override void OnResume()
        {
            base.OnResume();

            _loadingPanel.Visibility = ViewStates.Gone;
        }

        private async void Login(object sender, EventArgs eventArgs)
        {
            _loadingPanel.Visibility = ViewStates.Visible;
            _loadingPanel.Clickable = true;

            var errorMessage = FindViewById<TextView>(Resource.Id.ErrorText);
            errorMessage.Visibility = ViewStates.Gone;

            var loginInput = FindViewById<TextView>(Resource.Id.LoginInput);
            var passwordInput = FindViewById<TextView>(Resource.Id.PasswordInput);

            _loginViewModel.Login = loginInput.Text;
            _loginViewModel.Password = passwordInput.Text;

            var authToken = await _loginViewModel.Authenticate();
            if (!string.IsNullOrEmpty(authToken))
            {
                SaveAuthToken(authToken);
                var mainActivity = new Intent(ApplicationContext, typeof(MainActivity));
                StartActivity(mainActivity);
            }
            else
            {
                errorMessage.Text = _loginViewModel.AuthenticationErrorMessage;
                errorMessage.Visibility = ViewStates.Visible;
                _loadingPanel.Visibility = ViewStates.Gone;
            }
        }

        private void SaveAuthToken(string authToken)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var editor = prefs.Edit();
            editor.PutString("auth_token", authToken);
            editor.Apply();
        }
    }
}
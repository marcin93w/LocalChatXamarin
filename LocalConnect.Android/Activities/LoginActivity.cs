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
using LocalConnect.Models;
using LocalConnect.Services;
using LocalConnect.ViewModel;

namespace LocalConnect.Android.Activities
{
    [Activity(Label = "LC Android",
        MainLauncher = true,
        Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private readonly LoginViewModel _loginViewModel;

        private View _loadingPanel;
        private View _initializingPanel;

        public LoginActivity()
        {
            _loginViewModel = ViewModelLocator.Instance.GetViewModel<LoginViewModel>();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            _loadingPanel = FindViewById(Resource.Id.LoadingPanel);
            _initializingPanel = FindViewById(Resource.Id.InitializingPanel);

            var loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += Login;

            CheckSavedAuthToken();
        }

        protected override void OnResume()
        {
            base.OnResume();

            _loadingPanel.Visibility = ViewStates.Gone;
        }

        private async void CheckSavedAuthToken()
        {
            var savedToken = ReadAuthToken();
            if (savedToken == null)
            {
                _initializingPanel.Visibility = ViewStates.Gone;
            }
            else
            {
                var loginData = await _loginViewModel.Authenticate(savedToken);
                if (loginData == null)
                {
                    _initializingPanel.Visibility = ViewStates.Gone;
                }
                else
                {
                    TakeToApp(loginData);
                }
            }
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

            var loginData = await _loginViewModel.Authenticate();
            if (loginData != null)
            {
                TakeToApp(loginData);
            }
            else
            {
                errorMessage.Text = _loginViewModel.AuthenticationErrorMessage;
                errorMessage.Visibility = ViewStates.Visible;
                _loadingPanel.Visibility = ViewStates.Gone;
            }
        }

        private void TakeToApp(LoginData loginData)
        {
            SaveAuthToken(loginData.Token);
            var mainActivity = new Intent(ApplicationContext, typeof(MainActivity));
            StartActivity(mainActivity);
            Finish();
        }

        private void SaveAuthToken(string authToken)
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            var editor = prefs.Edit();
            editor.PutString("auth_token", authToken);
            editor.Apply();
        }

        private string ReadAuthToken()
        {
            var prefs = PreferenceManager.GetDefaultSharedPreferences(ApplicationContext);
            return prefs.GetString("auth_token", null);
        }
    }
}
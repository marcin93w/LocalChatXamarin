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
    [Activity(MainLauncher = true)]
    public class LoginActivity : Activity
    {
        private LoginViewModel LoginViewModel { get; }

        private bool _isOnRegisterView;

        private View _loadingPanel;
        private View _initializingPanel;

        private Binding<string, string> _loginBinding;
        private Binding<string, string> _passwordBinding;
        private Binding<string, string> _repeatPasswordBinding;
        private Binding<string, string> _nameBinding;
        private Binding<string, string> _surnameBinding;

        public LoginActivity()
        {
            LoginViewModel = ViewModelLocator.Instance.GetViewModel<LoginViewModel>();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            _loadingPanel = FindViewById(Resource.Id.LoadingPanel);
            _initializingPanel = FindViewById(Resource.Id.InitializingPanel);

            var loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += LoginOrRegister;
            var registerToggleButton = FindViewById<Button>(Resource.Id.ToogleRegisterButton);
            registerToggleButton.Click += ToggleRegisterView;

            CreateBindings();

            CheckSavedAuthToken();
        }

        private void CreateBindings()
        {
            var loginInput = FindViewById<TextView>(Resource.Id.LoginInput);
            var passwordInput = FindViewById<TextView>(Resource.Id.PasswordInput);
            var repeatPasswordInput = FindViewById<TextView>(Resource.Id.repeatedPassword);
            //var mailInput = FindViewById<TextView>(Resource.Id.Mail);
            var nameInput = FindViewById<TextView>(Resource.Id.Name);
            var surnameInput = FindViewById<TextView>(Resource.Id.Surname);

            _loginBinding = this.SetBinding(
              () => LoginViewModel.Login,
              loginInput, () => loginInput.Text, 
              BindingMode.TwoWay);

            _passwordBinding = this.SetBinding(
              () => LoginViewModel.Password,
              passwordInput, () => passwordInput.Text,
              BindingMode.TwoWay);

            _repeatPasswordBinding = this.SetBinding(
              () => LoginViewModel.RepeatedPassword,
              repeatPasswordInput, () => repeatPasswordInput.Text,
              BindingMode.TwoWay);

            //_mailBinding = this.SetBinding(
            //  () => LoginViewModel.,
            //  mailInput, () => mailInput.Text,
            //  BindingMode.TwoWay);

            _nameBinding = this.SetBinding(
              () => LoginViewModel.FirstName,
              nameInput, () => nameInput.Text,
              BindingMode.TwoWay);

            _surnameBinding = this.SetBinding(
              () => LoginViewModel.Surname,
              surnameInput, () => surnameInput.Text,
              BindingMode.TwoWay);
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
                var loginData = await LoginViewModel.Authenticate(savedToken);
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

        private void ToggleRegisterView(object sender, EventArgs e)
        {
            var logInButton = FindViewById<Button>(Resource.Id.LoginButton);
            var registerToggleButton = FindViewById<Button>(Resource.Id.ToogleRegisterButton);
            var registerPanel = FindViewById<ViewGroup>(Resource.Id.RegistrationInfoPanel);

            if (_isOnRegisterView)
            {
                logInButton.Text = "Sign in";
                registerToggleButton.Text = "Register";
                registerPanel.Visibility = ViewStates.Gone;
                _isOnRegisterView = false;
            }
            else
            {
                logInButton.Text = "Sign up";
                registerToggleButton.Text = "Log in";
                registerPanel.Visibility = ViewStates.Visible;
                _isOnRegisterView = true;
            }

            var errorMessage = FindViewById<TextView>(Resource.Id.ErrorText);
            errorMessage.Visibility = ViewStates.Gone;
        }

        private async void LoginOrRegister(object sender, EventArgs eventArgs)
        {
            _loadingPanel.Visibility = ViewStates.Visible;
            _loadingPanel.Clickable = true;
            
            var errorMessage = FindViewById<TextView>(Resource.Id.ErrorText);
            errorMessage.Visibility = ViewStates.Gone;

            var sessionInfo = await (_isOnRegisterView ? LoginViewModel.Register() : LoginViewModel.Authenticate());

            if (sessionInfo != null)
            {
                TakeToApp(sessionInfo);
            }
            else
            {
                errorMessage.Text = LoginViewModel.ErrorMessage;
                errorMessage.Visibility = ViewStates.Visible;
                _loadingPanel.Visibility = ViewStates.Gone;
            }
        }

        private void TakeToApp(SessionInfo sessionInfo)
        {
            SaveAuthToken(sessionInfo.Token);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Service.Textservice;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect.Models;
using LocalConnect.Services;
using LocalConnect.ViewModel;
using Xamarin.Facebook;
using Xamarin.Facebook.AppEvents;
using Xamarin.Facebook.Login;
using Xamarin.Facebook.Login.Widget;

namespace LocalConnect.Android.Views
{
    [Activity]
    public class LoginActivity : Activity
    {
        private LoginViewModel LoginViewModel { get; }

        private bool _isOnRegisterView;
        private bool _isReturningFromFacebookLogin;

        private View _loadingPanel;
        private TextView _errorMessage;

        private Binding<string, string> _loginBinding;
        private Binding<string, string> _passwordBinding;
        private Binding<string, string> _repeatPasswordBinding;
        private Binding<string, string> _nameBinding;
        private Binding<string, string> _surnameBinding;
        private ICallbackManager _callbackManager;

        public LoginActivity()
        {
            LoginViewModel = ViewModelLocator.Instance.GetViewModel<LoginViewModel>(this);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            FacebookSdk.SdkInitialize(ApplicationContext);

            SetContentView(Resource.Layout.Login);

            _loadingPanel = FindViewById(Resource.Id.LoadingPanel);
            _errorMessage = FindViewById<TextView>(Resource.Id.ErrorText);

            var loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += LoginOrRegister;
            var registerToggleButton = FindViewById<Button>(Resource.Id.ToogleRegisterButton);
            registerToggleButton.Click += ToggleRegisterView;

            SetUpFacebookLogin();

            CreateBindings();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            _callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
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

        private void SetUpFacebookLogin()
        {
            _callbackManager = CallbackManagerFactory.Create();

            var fbLoginButton = FindViewById<LoginButton>(Resource.Id.FacebookLoginButton);
            fbLoginButton.SetReadPermissions("user_friends");
            fbLoginButton.Click += (sender, args) => _isReturningFromFacebookLogin = true;
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

            _errorMessage.Visibility = ViewStates.Gone;
        }

        protected override void OnPause()
        {
            base.OnPause();
            AppEventsLogger.DeactivateApp(this);
        }

        protected override async void OnResume()
        {
            base.OnResume();

            AppEventsLogger.ActivateApp(this);

            var facebookToken = AccessToken.CurrentAccessToken;
            if (!string.IsNullOrEmpty(facebookToken?.Token))
            {
                if (_isReturningFromFacebookLogin)
                {
                    _loadingPanel.Visibility = ViewStates.Visible;
                    var sessionInfo = await LoginViewModel.LoginFromFacebook(facebookToken.Token);
                    _isReturningFromFacebookLogin = false;
                    _loadingPanel.Visibility = ViewStates.Gone;
                    if (sessionInfo != null)
                    {
                        TakeToApp();
                    }
                    else
                    {
                        _errorMessage.Text = "Error, could not authenticate";
                        _errorMessage.Visibility = ViewStates.Visible;
                    }
                }
                else
                {
                    LoginManager.Instance.LogOut();
                }
            }
            else
            {
                if (_isReturningFromFacebookLogin)
                {
                    _errorMessage.Text = "Error, could not retreive login token";
                    _errorMessage.Visibility = ViewStates.Visible;
                    _loadingPanel.Visibility = ViewStates.Gone;
                    _isReturningFromFacebookLogin = false;
                }
            }

        }

        private async void LoginOrRegister(object sender, EventArgs eventArgs)
        {
            _loadingPanel.Visibility = ViewStates.Visible;
            _loadingPanel.Clickable = true;

            _errorMessage.Visibility = ViewStates.Gone;

            var sessionInfo = await (_isOnRegisterView ? LoginViewModel.Register() : LoginViewModel.Authenticate());

            if (sessionInfo != null)
            {
                TakeToApp();
            }
            else
            {
                _errorMessage.Text = LoginViewModel.ErrorMessage;
                _errorMessage.Visibility = ViewStates.Visible;
                _loadingPanel.Visibility = ViewStates.Gone;
            }
        }

        private void TakeToApp()
        {
            var mainActivity = new Intent(ApplicationContext, typeof(MainActivity));
            StartActivity(mainActivity);
            Finish();
        }

    }
}
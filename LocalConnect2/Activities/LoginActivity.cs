using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using LocalConnect2.ViewModel;

namespace LocalConnect2.Activities
{
    [Activity(Label = "LoginActivity",
        MainLauncher = true,
        Icon = "@drawable/icon",
        Theme = "@android:style/Theme.Black.NoTitleBar")]
    public class LoginActivity : Activity
    {
        private readonly LoginViewModel _loginViewModel;

        public LoginActivity()
        {
            _loginViewModel = ViewModelLocator.Instance.GetViewModel<LoginViewModel>();
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            var loginButton = FindViewById<Button>(Resource.Id.LoginButton);
            loginButton.Click += Login;
        }

        private async void Login(object sender, EventArgs eventArgs)
        {
            var loginInput = FindViewById<TextView>(Resource.Id.LoginInput);
            var passwordInput = FindViewById<TextView>(Resource.Id.PasswordInput);

            _loginViewModel.Login = loginInput.Text;
            _loginViewModel.Password = passwordInput.Text;

            if (await _loginViewModel.Authenticate())
            {
                var mainActivity = new Intent(ApplicationContext, typeof(MainActivity));
                StartActivity(mainActivity);
            }
            else
            {
                var errorMessage = FindViewById<TextView>(Resource.Id.ErrorText);
                errorMessage.Text = _loginViewModel.AuthenticationErrorMessage;
                errorMessage.Visibility = ViewStates.Visible;
            }
        }
    }
}
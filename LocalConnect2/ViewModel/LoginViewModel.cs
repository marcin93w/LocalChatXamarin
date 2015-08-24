using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public string Login { set; get; }
        public string Password { set; get; }
        
        public string AuthenticationErrorMessage { set; get; }

        public async Task<bool> Authenticate()
        {
            var restClient = new RestClient();
            try
            {
                var authenticated = await restClient.Login(Login, Password);

                if (!authenticated)
                {
                    AuthenticationErrorMessage = "Bad username or password";
                }

                return authenticated;
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse) ex.Response).StatusCode == HttpStatusCode.Unauthorized)
                {
                    AuthenticationErrorMessage = "Bad username or password";
                }
                else
                {
                    AuthenticationErrorMessage = "Can not connect to server. " + ex.Message;
                }
                return false;
            }
        }
    }
}
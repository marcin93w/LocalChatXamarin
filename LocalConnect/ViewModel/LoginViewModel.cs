using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{

    public class LoginViewModel : ViewModelBase
    {
        public string Login { set; get; }
        public string Password { set; get; }
        
        public string AuthenticationErrorMessage { set; get; }

        public IDataProvider DataProvider { private get; set; }

        public async Task<LoginData> Authenticate(string authToken = null)
        {
            try
            {
                LoginData loginData;
                if (authToken != null)
                {
                    loginData = await DataProvider.LoginWithToken(authToken);
                }
                else
                {
                    loginData = await DataProvider.Login(Login, Password);
                }

                if (loginData != null)
                {
                    AuthenticationErrorMessage = "Bad username or password";
                }

                return loginData;
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    var response = (HttpWebResponse) (ex as WebException).Response;
                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        AuthenticationErrorMessage = "Bad username or password";
                        return null;
                    }
                }
                
                AuthenticationErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }
    }
}
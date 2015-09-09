using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect2.Services;

namespace LocalConnect2.ViewModel
{
    public class LoginViewModel : ViewModelBase
    {
        public string Login { set; get; }
        public string Password { set; get; }
        
        public string AuthenticationErrorMessage { set; get; }

        public async Task<string> Authenticate(string authToken = null)
        {
            try
            {
                string newToken;
                if (authToken != null)
                {
                    newToken = await RestClient.Instance.LoginWithToken(authToken);
                }
                else
                {
                    newToken = await RestClient.Instance.Login(Login, Password);
                }

                if (string.IsNullOrEmpty(newToken))
                {
                    AuthenticationErrorMessage = "Bad username or password";
                }

                return newToken;
            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    var response = (HttpWebResponse) (ex as WebException).Response;
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        AuthenticationErrorMessage = "Bad username or password";
                }
                else
                    AuthenticationErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }
    }
}
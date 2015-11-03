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

    public class LoginViewModel : ViewModelBase, ISocketClientUsingViewModel
    {
        private readonly User _user = new User();

        public string Login
        {
            get { return _user.Username; }
            set { _user.Username = value; }  
        }
        public string Password
        {
            get { return _user.Password; }
            set { _user.Password = value; }
        }
        public string RepeatedPassword { set; get; }

        public string FirstName
        {
            get { return _user.Person.FirstName; }
            set { _user.Person.FirstName = value; }
        }
        public string Surname
        {
            get { return _user.Person.Surname; }
            set { _user.Person.Surname = value; }
        }

        public string ErrorMessage { set; get; }

        public IDataProvider DataProvider { private get; set; }
        public ISocketClient SocketClient { private get; set; }


        public async Task<SessionInfo> Authenticate(string authToken = null, bool isFacebookToken = false)
        {
            try
            {
                var sessionInfo = await _user.Login(DataProvider, authToken, isFacebookToken);

                if (sessionInfo == null)
                {
                    ErrorMessage = "Bad username or password";
                }
                else
                {
                    SocketClient.Connect(sessionInfo.PersonId);
                    DataProvider.AuthToken = sessionInfo.Token;
                }

                return sessionInfo;
            }
            catch (Exception ex)
            {
                var response = (HttpWebResponse) (ex as WebException)?.Response;
                if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    ErrorMessage = "Bad username or password";
                    return null;
                }

                ErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }

        public async Task<SessionInfo> Register()
        {
            if (Password != RepeatedPassword)
            {
                ErrorMessage = "Passwords not match";
                return null;
            }

            try
            {
                RegistrationInfo response = await _user.Register(DataProvider);

                if (!response.Registered)
                {
                    if (response.ErrorCode == 1)
                        ErrorMessage = $"User {Login} already exists";
                    else
                        ErrorMessage = "Something goes wrong, please try again";
                }
                else
                {
                    SocketClient.Connect(response.SessionInfo.PersonId);
                    DataProvider.AuthToken = response.SessionInfo.Token;
                }

                return response.SessionInfo;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }

        public async Task<SessionInfo> LoginFromFacebook(string facebookToken)
        {
            return await Authenticate(facebookToken, true);
        }
    }
}
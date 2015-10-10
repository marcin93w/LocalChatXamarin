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

    public class LoginViewModel : ViewModelBase, IChatClientUsingViewModel
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
        public IChatClient ChatClient { private get; set; }


        public async Task<SessionInfo> Authenticate(string authToken = null)
        {
            try
            {
                SessionInfo sessionInfo;
                if (authToken != null)
                {
                    sessionInfo = await DataProvider.LoginWithToken(authToken);
                }
                else
                {
                    sessionInfo = await DataProvider.Login(Login, Password);
                }

                if (sessionInfo == null)
                {
                    ErrorMessage = "Bad username or password";
                }
                else
                {
                    ChatClient.Connect(sessionInfo.PersonId);
                    DataProvider.UpdateAuthToken(sessionInfo.Token);
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
                    ChatClient.Connect(response.SessionInfo.PersonId);
                    DataProvider.UpdateAuthToken(response.SessionInfo.Token);
                }

                return response.SessionInfo;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }
    }
}
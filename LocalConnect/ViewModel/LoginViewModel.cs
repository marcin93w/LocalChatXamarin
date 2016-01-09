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
        public enum AuthenticationStatus
        {
            WrongCredentials,
            PasswordsNotMatch,
            ConnectionError,
            UserAlreadyExists,
            Ok
        }

        private readonly User _user = new User();
        private readonly IRestClient _restClient;

        public LoginViewModel(IRestClient restClient)
        {
            _restClient = restClient;
        }

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

        public AuthenticationStatus Status { private set; get; }
        public string ErrorMessage { set; get; }

        public async Task<SessionInfo> Authenticate()
        {
            try
            {
                var sessionInfo = await _user.Login(_restClient);

                if (sessionInfo == null)
                {
                    Status = AuthenticationStatus.WrongCredentials;
                    ErrorMessage = "Bad username or password";
                }
                else
                {
                    Status = AuthenticationStatus.Ok;
                }
                return sessionInfo;
            }
            catch (Exception ex)
            {
                Status = AuthenticationStatus.ConnectionError;
                ErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }

        public async Task<SessionInfo> Register()
        {
            if (Password != RepeatedPassword)
            {
                Status = AuthenticationStatus.PasswordsNotMatch;
                ErrorMessage = "Passwords not match";
                return null;
            }

            try
            {
                RegistrationInfo response = await _user.Register(_restClient);

                if (!response.Registered)
                {
                    if (response.ErrorCode == 1)
                    {
                        Status = AuthenticationStatus.UserAlreadyExists;
                        ErrorMessage = $"User {Login} already exists";
                    }
                    else
                    {
                        Status = AuthenticationStatus.ConnectionError;
                        ErrorMessage = "Something goes wrong, please try again";
                    }
                }
                else
                {
                    Status = AuthenticationStatus.Ok;
                }

                return response.SessionInfo;
            }
            catch (Exception ex)
            {
                Status = AuthenticationStatus.ConnectionError;
                ErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }

        public async Task<SessionInfo> LoginFromFacebook(string facebookToken)
        {
            try
            {
                var sessionInfo = await _user.LoginFromFacebook(_restClient, facebookToken);
                Status = AuthenticationStatus.Ok;
                return sessionInfo;
            }
            catch (Exception ex)
            {
                Status = AuthenticationStatus.ConnectionError;
                ErrorMessage = "Can not connect to server. " + ex.Message;
            }

            return null;
        }
    }
}
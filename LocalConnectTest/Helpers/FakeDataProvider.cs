using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnectTest.Helpers
{
    class FakeDataProvider : IDataProvider
    {
        public static string CorrectUsername = "asd";
        public static string CorrectPassword = "asd";

        public static string CorrectPersonId = "qwe";

        private string _authToken;

        public Task<T> FetchDataAsync<T>(string method)
        {
            throw new NotImplementedException();
        }

        public Task PostDataAsync<TPostType>(string method, TPostType postData)
        {
            throw new NotImplementedException();
        }

        public Task<TReturnType> PostDataAsync<TPostType, TReturnType>(string method, TPostType postData)
        {
            throw new NotImplementedException();
        }

        public Task<object> FetchDataAsync(string method)
        {
            throw new NotImplementedException();
        }

        public async Task<SessionInfo> Login(string username, string password)
        {
            if (username == CorrectUsername && password == CorrectPassword)
            {
                _authToken = "zxc";
                return new SessionInfo(_authToken, CorrectPersonId);
            }
            else
            {
                throw new WebException();
            }
        }

        public Task<RegistrationInfo> Register(User user)
        {
            throw new NotImplementedException();
        }

        public Task<SessionInfo> LoginWithFacebook(string facebookToken)
        {
            throw new NotImplementedException();
        }

        public bool IsAuthenticated()
        {
            throw new NotImplementedException();
        }
    }
}

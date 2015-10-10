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

        public async Task<T> FetchDataAsync<T>(string method)
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

        public async Task<SessionInfo> LoginWithToken(string authToken)
        {
            throw new NotImplementedException();
        }

        public void UpdateAuthToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}

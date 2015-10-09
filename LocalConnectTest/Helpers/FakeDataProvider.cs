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

        public Task<object> FetchDataAsync(string method)
        {
            throw new NotImplementedException();
        }

        public async Task<LoginData> Login(string username, string password)
        {
            if (username == CorrectUsername && password == CorrectPassword)
            {
                _authToken = "zxc";
                return new LoginData(_authToken, CorrectPersonId);
            }
            else
            {
                throw new WebException();
            }
        }

        public async Task<LoginData> LoginWithToken(string authToken)
        {
            throw new NotImplementedException();
        }
    }
}

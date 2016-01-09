using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using LocalConnect.Services;
using Moq;

namespace LocalConnectTest.Helpers
{
    class RestClientMock : IRestClient
    {
        public static string CorrectUsername = "asd";
        public static string CorrectPassword = "asd";

        public static string CorrectPersonId = "qwe";

        private string _authToken;

        public List<Person> People { private get; set; } 

        public async Task<T> FetchDataAsync<T>(string method)
        {
            if (typeof (T) == typeof (List<Person>))
            {
                return (T)(object)People;
            }

            if (typeof (T) == typeof (Person) && method == "me")
            {
                return (T)(object) new Me();
            }

            if (typeof (T) == typeof (Person) && method.StartsWith("people/"))
            {
                return (T) (object) new Person {PersonId = method.Replace("people/", "")};
            }

            return default(T);
        }
        
        public async Task PostDataAsync<TPostType>(string method, TPostType postData)
        {
            if (method == "me/updateLocation")
            {
                MyLocationOnServer = postData as Location;
            }
        }

        public Location MyLocationOnServer { get; private set; }

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
                return null;
            }
        }

        public async Task<RegistrationInfo> Register(User user)
        {
            if (user.Username == CorrectUsername)
            {
                return new RegistrationInfo { Registered = false, ErrorCode = 1 };
            }
            else
            {
                _authToken = "zxc";
                return new RegistrationInfo
                {
                    ErrorCode = 0,
                    SessionInfo = new SessionInfo(_authToken, CorrectPersonId),
                    Registered = true
                };
            }
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

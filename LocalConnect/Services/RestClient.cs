using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Helpers;
using LocalConnect.Models;
using Newtonsoft.Json;

namespace LocalConnect.Services
{
    public class RestClient : IRestClient
    {
        private string _url = "https://lc-fancydesign.rhcloud.com/api";
        private string _authenticationHeader;
        private readonly IAuthTokenManager _authTokenManager;

        public RestClient(IAuthTokenManager authTokenManager)
        {
            _authTokenManager = authTokenManager;
        }

        public async Task<SessionInfo> Login(string username, string password)
        {
            var url = Path.Combine(_url, "login");

            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Credentials = new NetworkCredential(username, password);

            SessionInfo sessionInfo;
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        sessionInfo = await DeserializeFromStream<SessionInfo>(stream);
                    }
                }
                else
                {
                    return null;
                }
            }

            _authenticationHeader = $"Bearer {sessionInfo.Token}";
            _authTokenManager.SaveAuthToken(sessionInfo.Token);

            return sessionInfo;
        }

        public async Task<RegistrationInfo> Register(User user)
        {
            var request = PrepareRequest("register", true);
            await PostRequestAsync(request, user);
            var registrationInfo = await ExecuteRequestAsync<RegistrationInfo>(request);

            _authenticationHeader = $"Bearer {registrationInfo.SessionInfo.Token}";
            _authTokenManager.SaveAuthToken(registrationInfo.SessionInfo.Token);

            return registrationInfo;
        }

        public async Task<SessionInfo> LoginWithFacebook(string facebookToken)
        {
            var request = PrepareRequest($"loginWithFacebook?access_token={facebookToken}", true);
            request.Method = "GET";

            var sessionInfo = await ExecuteRequestAsync<SessionInfo>(request);

            _authenticationHeader = $"Bearer {sessionInfo.Token}";
            _authTokenManager.SaveAuthToken(sessionInfo.Token);

            return sessionInfo;
        }

        public bool IsAuthenticated()
        {
            if (string.IsNullOrEmpty(_authenticationHeader))
            {
                var authToken = _authTokenManager.ReadAuthToken();
                if (string.IsNullOrEmpty(authToken))
                {
                    return false;
                }
                _authenticationHeader = "Bearer " + authToken;
            }

            return true;
        }

        public async Task<object> FetchDataAsync(string method)
        {
            return await FetchDataAsync<object>(method);
        }

        public async Task<T> FetchDataAsync<T>(string method)
        {
            var request = PrepareRequest(method);
            request.Method = "GET";

            return await ExecuteRequestAsync<T>(request);
        }

        public async Task PostDataAsync<TPostType>(string method, TPostType postData)
        {
            var request = PrepareRequest(method);
            await PostRequestAsync(request, postData);
            await request.GetResponseAsync();
        }

        public async Task<TReturnType> PostDataAsync<TPostType, TReturnType>(string method, TPostType postData)
        {
            var request = PrepareRequest(method);
            await PostRequestAsync(request, postData);
            return await ExecuteRequestAsync<TReturnType>(request);
        }

        private WebRequest PrepareRequest(string method, bool noAuthorization = false)
        {
            if (string.IsNullOrEmpty(_authenticationHeader) && !noAuthorization)
            {
                _authenticationHeader = "Bearer " + _authTokenManager.ReadAuthToken();

                if (string.IsNullOrEmpty(_authenticationHeader))
                    throw new MissingAuthenticationTokenException();
            }

            var url = Path.Combine(_url, method);
            
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            if(!noAuthorization)
                request.Headers[HttpRequestHeader.Authorization] = _authenticationHeader;

            return request;
        }

        private async Task PostRequestAsync<T>(WebRequest request, T postData)
        {
            request.Method = "POST";
            using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(streamWriter, postData);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private async Task<T> ExecuteRequestAsync<T>(WebRequest request)
        {
            using (var response = await request.GetResponseAsync())
            {
                using (var stream = response.GetResponseStream())
                {
                    return await DeserializeFromStream<T>(stream);
                }
            }
        }

        public async Task<T> DeserializeFromStream<T>(Stream stream)
        {
            var serializer = new JsonSerializer();

            using (var sr = new StreamReader(stream))
            using (var jsonTextReader = new JsonTextReader(sr))
            {
                return await Task.Run(() => serializer.Deserialize<T>(jsonTextReader));
            }
        }
    }
}
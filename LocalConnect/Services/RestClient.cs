using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using Newtonsoft.Json;

namespace LocalConnect.Services
{
    public class RestClient : IDataProvider
    {
        private string _url = "https://lc-fancydesign.rhcloud.com/api";
        private string _authenticationHeader;

        public async Task<LoginData> Login(string username, string password)
        {
            var url = Path.Combine(_url, "login");

            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Credentials = new NetworkCredential(username, password);

            return await SendLoginRequest(request);
        }

        public async Task<LoginData> LoginWithToken(string authToken)
        {
            var url = Path.Combine(_url, "loginWithToken");

            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Headers[HttpRequestHeader.Authorization] = $"Bearer {authToken}"; ;

            return await SendLoginRequest(request);
        }

        private async Task<LoginData> SendLoginRequest(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var loginData = await DeserializeFromStream<LoginData>(stream);
                        _authenticationHeader = $"Bearer {loginData.Token}";

                        return loginData;
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<object> FetchDataAsync(string method)
        {
            return await FetchDataAsync<object>(method);
        }

        public async Task<T> FetchDataAsync<T>(string method)
        {
            if (string.IsNullOrEmpty(_authenticationHeader))
            {
                throw new MissingAuthenticationTokenException();
            }

            var url = Path.Combine(_url, method);

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(new Uri(url));
                request.ContentType = "application/json";
                request.Method = "GET";
                request.Headers[HttpRequestHeader.Authorization] = _authenticationHeader;

                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        return await DeserializeFromStream<T>(stream);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ConnectionException(ex);
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
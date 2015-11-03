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

        public string AuthToken
        {
            get { return _authenticationHeader.Replace("Bearer ", string.Empty); }
            set { _authenticationHeader = $"Bearer {value}"; }
        }

        public async Task<SessionInfo> Login(string username, string password)
        {
            var url = Path.Combine(_url, "login");

            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Credentials = new NetworkCredential(username, password);

            return await SendLoginRequest(request);
        }

        private async Task<SessionInfo> SendLoginRequest(HttpWebRequest request)
        {
            using (var response = (HttpWebResponse)await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        return await DeserializeFromStream<SessionInfo>(stream);
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

        public async Task<T> FetchDataAsync<T>(string method, bool noAuthorization = false)
        {
            try
            {
                var request = PrepareRequest(method, noAuthorization);
                request.Method = "GET";

                return await ExecuteRequestAsync<T>(request);
            }
            catch (Exception ex)
            {
                throw new ConnectionException(ex);
            }
        }

        public async Task PostDataAsync<TPostType>(string method, TPostType postData, bool noAuthorization = false)
        {
            try
            {
                var request = PrepareRequest(method, noAuthorization);
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(streamWriter, postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                await request.GetResponseAsync();
            }
            catch (Exception ex)
            {
                throw new ConnectionException(ex);
            }
        }

        public async Task<TReturnType> PostDataAsync<TPostType, TReturnType>(string method, TPostType postData, bool noAuthorization = false)
        {
            try
            {
                var request = PrepareRequest(method, noAuthorization);
                request.Method = "POST";

                using (var streamWriter = new StreamWriter(await request.GetRequestStreamAsync()))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(streamWriter, postData);
                    streamWriter.Flush();
                    streamWriter.Close();
                }

                return await ExecuteRequestAsync<TReturnType>(request);
            }
            catch (Exception ex)
            {
                throw new ConnectionException(ex);
            }
        }

        private WebRequest PrepareRequest(string method, bool noAuthorization)
        {
            if (string.IsNullOrEmpty(_authenticationHeader) && !noAuthorization)
            {
                throw new MissingAuthenticationTokenException();
            }

            var url = Path.Combine(_url, method);
            
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            if(!noAuthorization)
                request.Headers[HttpRequestHeader.Authorization] = _authenticationHeader;

            return request;
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
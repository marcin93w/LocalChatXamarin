using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;

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
                        var jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                        Console.Out.WriteLine("Response: {0}", jsonDoc);

                        var personId = jsonDoc["personId"].GetValue();
                        var newToken = jsonDoc["token"].GetValue();
                        _authenticationHeader = $"Bearer {newToken}";

                        return new LoginData(newToken, personId);
                    }
                }
                else
                {
                    return null;
                }
            }
        } 

        public async Task<JsonValue> FetchDataAsync(string method)
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
                        var jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                        Console.Out.WriteLine("Response: {0}", jsonDoc);

                        return jsonDoc;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ConnectionException(ex);
            }
        }
    }

    public static class SystemJsonExtensions
    {
        public static string GetValue(this JsonValue jsonData, string field = null)
        {
            var jsonValue = field != null ? jsonData[field] : jsonData;
            return jsonValue.ToString().Replace("\"", string.Empty);
        }
    }
}
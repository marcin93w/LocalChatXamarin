using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LocalConnect2.Services
{
    public class RestClient
    {
        private string _url = "https://lc-fancydesign.rhcloud.com/api";
        private string _authenticationHeader;

        private static RestClient _instance;
        public static RestClient Instance => _instance ?? (_instance = new RestClient());

        private RestClient()
        {
        }

        public async Task<string> Login(string username, string password)
        {
            var url = Path.Combine(_url, "login");

            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            request.Credentials = new NetworkCredential(username, password);
            
            using (var response = (HttpWebResponse) await request.GetResponseAsync())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var jsonDoc = await Task.Run(() => JsonValue.Load(stream));
                        Console.Out.WriteLine("Response: {0}", jsonDoc);

                        var authToken = jsonDoc["token"].ToString().Replace("\"", string.Empty);
                        _authenticationHeader = $"Bearer {authToken}";

                        return authToken;
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
}
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
        private string _url = "http://192.168.0.10:1337/api";

        public async Task<bool> Login(string username, string password)
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

                        SaveAuthToken(jsonDoc["token"]);

                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        private void SaveAuthToken(string token)
        {
            
        }

        public async Task<JsonValue> FetchDataAsync(string method)
        {
            var url = Path.Combine(_url, method);
            
            var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";
            
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
    }
}
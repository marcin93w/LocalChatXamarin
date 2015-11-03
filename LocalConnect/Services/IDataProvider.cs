using System;
using System.Collections.Generic;
using System.Json;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface IDataProvider
    {
        Task<T> FetchDataAsync<T>(string method, bool noAuthorization = false);
        Task PostDataAsync<TPostType>(string method, TPostType postData, bool noAuthorization = false);
        Task<TReturnType> PostDataAsync<TPostType, TReturnType>(string method, TPostType postData, bool noAuthorization = false);
        Task<object> FetchDataAsync(string method);
        Task<SessionInfo> Login(string username, string password);
        string AuthToken { set; }
    }
}

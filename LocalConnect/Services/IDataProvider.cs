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
        Task<T> FetchDataAsync<T>(string method);
        Task<TReturnType> PostDataAsync<TPostType, TReturnType>(string method, TPostType postData);
        Task<object> FetchDataAsync(string method);
        Task<SessionInfo> Login(string username, string password);
        Task<SessionInfo> LoginWithToken(string authToken);
        void UpdateAuthToken(string token);
    }
}

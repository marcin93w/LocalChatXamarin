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
        Task<JsonValue> FetchDataAsync(string method);
        Task<LoginData> Login(string username, string password);
        Task<LoginData> LoginWithToken(string authToken);
    }
}

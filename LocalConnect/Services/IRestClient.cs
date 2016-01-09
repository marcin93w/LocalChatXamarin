using System.Threading.Tasks;
using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface IRestClient
    {
        Task<T> FetchDataAsync<T>(string method);
        Task PostDataAsync<TPostType>(string method, TPostType postData);
        Task<TReturnType> PostDataAsync<TPostType, TReturnType>(string method, TPostType postData);
        Task<object> FetchDataAsync(string method);
        Task<SessionInfo> Login(string username, string password);
        Task<RegistrationInfo> Register(User user);
        Task<SessionInfo> LoginWithFacebook(string facebookToken);
        bool IsAuthenticated();
    }
}

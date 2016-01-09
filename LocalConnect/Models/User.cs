using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class User
    {
        public User()
        {
            Person = new Person();
        }

        public string Username { set; get; }
        public string Password { set; get; }

        public Person Person { get; }

        public async Task<RegistrationInfo> Register(IRestClient restClient)
        {
            return await restClient.Register(this);
        }

        public async Task<SessionInfo> Login(IRestClient restClient)
        {
            return await restClient.Login(Username, Password);
        }

        public async Task<SessionInfo> LoginFromFacebook(IRestClient restClient, string facebookToken)
        {
            return await restClient.LoginWithFacebook(facebookToken);
        }
    }
}

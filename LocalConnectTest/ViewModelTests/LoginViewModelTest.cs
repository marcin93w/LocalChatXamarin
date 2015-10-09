using System.Net;
using System.Threading.Tasks;
using LocalConnect.Services;
using LocalConnect.ViewModel;
using LocalConnectTest.Helpers;
using NUnit.Framework;

namespace LocalConnectTest.ViewModelTests
{
    [TestFixture]
    public class LoginViewModelTest
    {
        private readonly FakeChatClient _chatClient = new FakeChatClient();
        private readonly FakeDataProvider _dataProvider = new FakeDataProvider();

        private LoginViewModel _loginViewModel;

        [SetUp]
        public void SetUp()
        {
            _loginViewModel = new LoginViewModel();
            _loginViewModel.ChatClient = _chatClient;
            _loginViewModel.DataProvider = _dataProvider;
        }

        [Test]
        public async Task CorrectAuthenticationTest()
        {
            _loginViewModel.Login = FakeDataProvider.CorrectUsername;
            _loginViewModel.Password = FakeDataProvider.CorrectPassword;
            var loginData = await _loginViewModel.Authenticate();

            Assert.AreEqual(FakeDataProvider.CorrectPersonId, loginData.PersonId);
        }

        [Test]
        public async Task IncorrectAuthenticationTest()
        {
            _loginViewModel.Login = FakeDataProvider.CorrectUsername;
            _loginViewModel.Password = "ppl";
            var loginData = await _loginViewModel.Authenticate();

            Assert.IsNull(loginData);
        }
    }
}

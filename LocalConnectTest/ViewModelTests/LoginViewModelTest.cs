using System;
using System.Net;
using System.Threading.Tasks;
using LocalConnect.Services;
using LocalConnect.ViewModel;
using LocalConnectTest.Helpers;
using Moq;
using NUnit.Framework;

namespace LocalConnectTest.ViewModelTests
{
    [TestFixture]
    public class LoginViewModelTest
    {
        private readonly FakeSocketClient _socketClient = new FakeSocketClient();
        private readonly FakeRestClient _restClient = new FakeRestClient();

        private LoginViewModel _loginViewModel;

        [SetUp]
        public void SetUp()
        {
            _loginViewModel = new LoginViewModel();
            _loginViewModel.RestClient = _restClient;
        }

        [Test]
        public async Task CorrectAuthenticationTest()
        {
            _loginViewModel.Login = FakeRestClient.CorrectUsername;
            _loginViewModel.Password = FakeRestClient.CorrectPassword;
            var loginData = await _loginViewModel.Authenticate();

            Assert.AreEqual(FakeRestClient.CorrectPersonId, loginData.PersonId);
        }

        [Test]
        public async Task IncorrectAuthenticationTest()
        {
            _loginViewModel.Login = FakeRestClient.CorrectUsername;
            _loginViewModel.Password = "ppl";
            var loginData = await _loginViewModel.Authenticate();

            Assert.IsNull(loginData);
        }

        [Test]
        public async Task ErrorDuringAuthenticationTest()
        {
            var mockDataProvider = new Mock<IRestClient>();
            mockDataProvider
                .Setup(e => e.Login(FakeRestClient.CorrectUsername, FakeRestClient.CorrectPassword))
                .ThrowsAsync(new Exception());

            _loginViewModel.RestClient = mockDataProvider.Object;
            _loginViewModel.Login = FakeRestClient.CorrectUsername;
            _loginViewModel.Password = FakeRestClient.CorrectPassword;
            var loginData = await _loginViewModel.Authenticate();

            Assert.IsNull(loginData);
        }

        //[Test]
        //public async Task ExistingUserRegisterOnServerTest()
        //{
        //    var loginViewModel = new LoginViewModel();
        //    loginViewModel.DataProvider = new RestClient();
        //    loginViewModel.ChatClient = _chatClient;

        //    loginViewModel.Login = string.Empty;
        //    loginViewModel.Password = FakeDataProvider.CorrectPassword;
        //    loginViewModel.RepeatedPassword = FakeDataProvider.CorrectPassword;
        //    loginViewModel.FirstName = "asd";
        //    loginViewModel.Surname = "qwe";

        //    var loginData = await loginViewModel.Register();
        //    Assert.IsNull(loginData);
        //}
    }
}

using System;
using System.Collections.Generic;
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
        private readonly SocketClientMock _socketClient = new SocketClientMock();
        private readonly RestClientMock _restClient = new RestClientMock();

        private LoginViewModel _loginViewModel;

        [SetUp]
        public void SetUp()
        {
            _loginViewModel = new LoginViewModel(_restClient);
        }

        [Test]
        public async Task CorrectAuthenticationTest()
        {
            _loginViewModel.Login = RestClientMock.CorrectUsername;
            _loginViewModel.Password = RestClientMock.CorrectPassword;
            var loginData = await _loginViewModel.Authenticate();

            Assert.AreEqual(RestClientMock.CorrectPersonId, loginData.UserId);
            Assert.AreEqual(LoginViewModel.AuthenticationStatus.Ok, _loginViewModel.Status);
        }

        [Test]
        public async Task IncorrectAuthenticationTest()
        {
            _loginViewModel.Login = RestClientMock.CorrectUsername;
            _loginViewModel.Password = "ppl";
            var loginData = await _loginViewModel.Authenticate();

            Assert.IsNull(loginData);
            Assert.AreEqual(LoginViewModel.AuthenticationStatus.WrongCredentials, _loginViewModel.Status);
        }

        [Test]
        public async Task ErrorDuringAuthenticationTest()
        {
            var mockDataProvider = new Mock<IRestClient>();
            mockDataProvider
                .Setup(e => e.Login(RestClientMock.CorrectUsername, RestClientMock.CorrectPassword))
                .ThrowsAsync(new Exception());

            _loginViewModel = new LoginViewModel(mockDataProvider.Object);
            _loginViewModel.Login = RestClientMock.CorrectUsername;
            _loginViewModel.Password = RestClientMock.CorrectPassword;
            var loginData = await _loginViewModel.Authenticate();

            Assert.IsNull(loginData);
            Assert.AreEqual(LoginViewModel.AuthenticationStatus.ConnectionError, _loginViewModel.Status);
        }

        [Test, Sequential]
        public async Task PasswordRepeatTest(
            [Values("a", "b", "abc", "")] string password, 
            [Values(true, false, false, false)] bool isMatch)
        {
            _loginViewModel.Password = "a";
            _loginViewModel.RepeatedPassword = password;

            var registerInfo = await _loginViewModel.Register();

            Assert.AreEqual(isMatch, 
                _loginViewModel.Status != LoginViewModel.AuthenticationStatus.PasswordsNotMatch);
        }

        [Test]
        public async Task ExistingUserRegisterTest()
        {
            _loginViewModel.Login = RestClientMock.CorrectUsername;
            _loginViewModel.Password = RestClientMock.CorrectPassword;
            _loginViewModel.RepeatedPassword = RestClientMock.CorrectPassword;
            _loginViewModel.FirstName = "asd";
            _loginViewModel.Surname = "qwe";

            var loginData = await _loginViewModel.Register();

            Assert.IsNull(loginData);
            Assert.AreEqual(LoginViewModel.AuthenticationStatus.UserAlreadyExists, _loginViewModel.Status);
        }

        [Test]
        public async Task CorrectRegisterTest()
        {
            _loginViewModel.Login = RestClientMock.CorrectUsername + "1";
            _loginViewModel.Password = RestClientMock.CorrectPassword;
            _loginViewModel.RepeatedPassword = RestClientMock.CorrectPassword;
            _loginViewModel.FirstName = "asd";
            _loginViewModel.Surname = "qwe";

            var loginData = await _loginViewModel.Register();

            Assert.IsNotNull(loginData);
            Assert.AreEqual(LoginViewModel.AuthenticationStatus.Ok, _loginViewModel.Status);
        }
    }
}

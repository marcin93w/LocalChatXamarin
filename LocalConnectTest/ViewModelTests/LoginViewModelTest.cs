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
        private readonly FakeDataProvider _dataProvider = new FakeDataProvider();

        private LoginViewModel _loginViewModel;

        [SetUp]
        public void SetUp()
        {
            _loginViewModel = new LoginViewModel();
            _loginViewModel.SocketClient = _socketClient;
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

        [Test]
        public async Task ErrorDuringAuthenticationTest()
        {
            var mockDataProvider = new Mock<IDataProvider>();
            mockDataProvider
                .Setup(e => e.Login(FakeDataProvider.CorrectUsername, FakeDataProvider.CorrectPassword))
                .ThrowsAsync(new Exception());

            _loginViewModel.DataProvider = mockDataProvider.Object;
            _loginViewModel.Login = FakeDataProvider.CorrectUsername;
            _loginViewModel.Password = FakeDataProvider.CorrectPassword;
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

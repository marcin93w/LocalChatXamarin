using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using LocalConnect.ViewModel;
using LocalConnectTest.Helpers;
using NUnit.Framework;

namespace LocalConnectTest.ViewModelTests
{
    [TestFixture]
    public class PersonChatViewModelTest
    {
        private PersonChatViewModel _personChatViewModel;
        private RestClientMock _restClient = new RestClientMock();
        private SocketClientMock _socketClient = new SocketClientMock();

        private Person _person;
        private Me _me;

        [SetUp]
        public void SetUp()
        {
            if(_personChatViewModel == null)
                _personChatViewModel = new PersonChatViewModel(_socketClient, _restClient);
            _personChatViewModel.RunOnUiThread = action => action?.Invoke();
            _person = new Person {PersonId = "b"};
            _me = new Me {PersonId = "a"};
            _personChatViewModel.Initialize(new PersonViewModel(_person, _me));
        }

        [Test]
        public void MessageReceiveTest()
        {
            _socketClient.InvokeMessageReceive(new IncomeMessage("id", _person.PersonId, "", DateTime.Now));
            Assert.That(_personChatViewModel.Messages.Last().MessageId, Is.EqualTo("id"));
        }

        [Test]
        public void MessageSendingTest()
        {
            _personChatViewModel.SendMessage("text");
            Assert.That(_personChatViewModel.Messages.Last().Text, Is.EqualTo("text"));
            Assert.True(_personChatViewModel.Messages.Last() is OutcomeMessage);
            Assert.That((_personChatViewModel.Messages.Last() as OutcomeMessage).ReceiverId, Is.EqualTo("b"));
            Assert.False((_personChatViewModel.Messages.Last() as OutcomeMessage).Sent);
            Assert.False((_personChatViewModel.Messages.Last() as OutcomeMessage).DeliverError);
        }
    }
}

using System;
using System.CodeDom;
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
    public class PeopleViewModelTest
    {
        private readonly Location _myLocation = new Location(20, 50);

        private readonly List<Person> _people = new List<Person>
        {
            new Person("name1", "surname1", "desc1", new JammedLocation(20, 50, 0), "1"),
            new Person("name2", "surname2", "desc2", new JammedLocation(20.1, 50, 10000), "2"),
            new Person("name3", "surname3", "desc3", new JammedLocation(20, 50.1, 500), "3"),
            new Person("name4", "surname4", "desc4", new JammedLocation(20, 50.2, 1000), "4"),
            new Person("name5", "surname5", "desc5", new JammedLocation(20.2, 50.1, 5000), "5"),
            new Person("name6", "surname6", "desc6", null, "6") { UnreadMessages = 3 }
        };

        private PeopleViewModel _peopleViewModel;
        private RestClientMock _restClient = new RestClientMock();
        private SocketClientMock _socketClient = new SocketClientMock();

        [SetUp]
        public void SetUp()
        {
            _restClient.People = _people;
            _socketClient.Disconnect();
            _peopleViewModel = new PeopleViewModel(_restClient, _socketClient);
            _peopleViewModel.FetchMyDataAsync();
            _peopleViewModel.SendLocationUpdate(_myLocation);
            _peopleViewModel.FetchPeopleData();
        }

        [Test]
        public void DataLoadingTest()
        {
            Assert.AreEqual(true, _peopleViewModel.DataLoaded);
            Assert.AreEqual(_myLocation, _peopleViewModel.Me.RealLocation);
            Assert.IsNotEmpty(_peopleViewModel.People);
        }

        [Test]
        public void LocationSendingTest()
        {
            //TODO chack distance changing while sending location
            Assert.AreEqual(_myLocation, _restClient.MyLocationOnServer);
        }

        [Test]
        public void PeopleSortingTest()
        {
            Assert.AreEqual("1", _peopleViewModel.People[1].Id);
            Assert.AreEqual("2", _peopleViewModel.People[2].Id);
            Assert.AreEqual("3", _peopleViewModel.People[3].Id);
            Assert.AreEqual("4", _peopleViewModel.People[5].Id);
            Assert.AreEqual("5", _peopleViewModel.People[4].Id);
            Assert.AreEqual("6", _peopleViewModel.People[0].Id);
        }

        [Test, Sequential]
        public void DistanceCalculatingTest(
            [Values("1", "2", "3", "4", "5", "6")] string id,
            [Values(0d, 7147d, 11120d, 22240d, 18100d, null)] double? distance)
        {
            var person = _peopleViewModel.People.First(p => p.Id == id);
            if (!distance.HasValue)
            {
                Assert.IsNull(person.Distance);
            }
            else
            {
                Assert.AreEqual(distance.Value, person.Distance.Value, 10.0);
            }
        }

        [Test]
        public void TypeOfDistanceTest()
        {
            Assert.AreEqual(PersonViewModel.DistanceType.Unknown, _peopleViewModel.People[0].TypeOfDistance);
            Assert.AreEqual(PersonViewModel.DistanceType.Precise, _peopleViewModel.People[1].TypeOfDistance);
            Assert.AreEqual(PersonViewModel.DistanceType.LowerThan, _peopleViewModel.People[2].TypeOfDistance);
            Assert.That(
                _peopleViewModel.People.Skip(3).Select(p => p.TypeOfDistance), 
                    Is.All.EqualTo(PersonViewModel.DistanceType.Between));
        }

        [Test, Sequential]
        public void DistanceDescriptionTest(
            [Values("1", "2", "3", "4", "5", "6")] string id,
            [Values("0 m", "17.1 km", "11.6 km", "23.2 km", "23.1 km", null)] string distanceDescription,
            [Values(null, null, "10.6 km", "21.2 km", "13.1 km" ,null)] string minDistanceDescription)
        {
            var person = _peopleViewModel.People.First(p => p.Id == id);
            Assert.AreEqual(distanceDescription, person.DistanceDescription);
            Assert.AreEqual(minDistanceDescription, person.MinDistanceDescription);
        }

        [Test, Sequential]
        public void MessageFromPersonFromList(
            [Values("1", "6")] string personId, [Values(0, 3)] int currentMessagesCount)
        {
            var person = _peopleViewModel.People.First(p => p.Id == personId);
            _socketClient.InvokeMessageReceive(new IncomeMessage("", personId, "", DateTime.Now));
            Assert.AreEqual(currentMessagesCount + 1, person.UnreadMessages);
            Assert.That(_peopleViewModel.People, Has.Count.EqualTo(6));
        }

        [Test]
        public void MessageFromPersonOutsideOfList()
        {
            var newPersonId = "newPerson";
            _socketClient.InvokeMessageReceive(new IncomeMessage("", newPersonId, "", DateTime.Now));

            Assert.That(_peopleViewModel.People.Select(p => p.Id), Has.Some.EqualTo(newPersonId));
            var person = _peopleViewModel.People.First(p => p.Id == newPersonId);
            Assert.That(person.UnreadMessages, Is.EqualTo(1));
        }
    }
}

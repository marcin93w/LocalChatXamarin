using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Models;
using LocalConnect.Services;
using LocalConnect.ViewModel;
using LocalConnectTest.Helpers;
using NUnit.Framework;

namespace LocalConnectTest.ViewModelTests
{
    [TestFixture]
    public class MyProfileViewModelTest
    {
        [Test]
        public async void ProfileUpdateTest()
        {
            var myProfileVm = new MyProfileViewModel();
            myProfileVm.RestClient = new RestClient(new FakeSessionInfoManager());
            myProfileVm.Initialize(new Me
            {
                FirstName = "asd",
                LongDescription = "asd",
                ShortDescription = "asd",
                Surname = "asd"
            });

            await myProfileVm.Save();
        }
    }
}

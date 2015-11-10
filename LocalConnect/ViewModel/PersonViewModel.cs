using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PersonViewModel : ViewModelBase
    {
        private readonly Person _person;
        private readonly Me _me;

        public PersonViewModel(Person person, Me me)
        {
            _person = person;
            if (me != null)
            {
                _me = me;
                CreateLocationDescription();
                _me.LocationChanged += (sender, args) => CreateLocationDescription();
            }
        }

        public string Id => _person.PersonId;
        public string Name => _person.Name;
        public string Avatar => _person.Avatar;
        public string ShortDescription => _person.ShortDescription;
        public string LongDescription => _person.LongDescription;
        public Location Location => _person.Location;
        public string LocationDescription { private set; get; }


        private void CreateLocationDescription()
        {
            var distance = _person.CalculateDistanceFrom(_me);
            if (distance.HasValue)
            {
                if (distance < 1000)
                    LocationDescription = $"{distance.Value:0} m from you";
                else
                    LocationDescription = $"{(distance.Value / 1000):0.0} km from you";
            }
            else
                LocationDescription = "Location unknown";
        }

        public async Task LoadDetailedData(IRestClient restClient)
        {
            await _person.LoadDetailedData(restClient);
        }
    }
}

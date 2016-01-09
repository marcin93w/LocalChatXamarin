using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using LocalConnect.Models;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{
    public class PersonViewModel : ViewModelBase
    {
        public enum DistanceType
        {
            Unknown, Precise, LowerThan, Between
        };

        private readonly Person _person;
        private readonly Me _me;

        public PersonViewModel(Person person, Me me)
        {
            _person = person;
            if (me != null)
            {
                _me = me;
                CalculateLocation();
                _me.LocationChanged += (sender, args) => CalculateLocation();
            }
        }

        public string Id => _person.PersonId;
        public string Name => _person.FirstName + " " + _person.Surname;
        public string Avatar => _person.Avatar;
        public string ShortDescription => _person.ShortDescription;
        public string LongDescription => _person.LongDescription;
        public JammedLocation Location => _person.Location;
        public int? UnreadMessages
        {
            get { return _person.UnreadMessages; }
            set
            {
                _person.UnreadMessages = value;
                RaisePropertyChanged<int?>();
            }
        }

        public double? Distance { private set; get; }
        public DistanceType TypeOfDistance { private set; get; }
        public string DistanceDescription { private set; get; }
        public string MinDistanceDescription { private set; get; }


        private void CalculateLocation()
        {
            var distance = _person.CalculateDistanceFrom(_me);
            if (distance.HasValue)
            {
                Distance = distance;
                if (Location.Tolerance < double.Epsilon)
                {
                    TypeOfDistance = DistanceType.Precise;
                    DistanceDescription = GetDistanceString(distance.Value);
                }
                else if (Distance < Location.Tolerance)
                {
                    TypeOfDistance = DistanceType.LowerThan;
                    DistanceDescription = GetDistanceString(distance.Value + Location.Tolerance);
                }
                else
                {
                    TypeOfDistance = DistanceType.Between;
                    DistanceDescription = GetDistanceString(distance.Value + Location.Tolerance);
                    MinDistanceDescription = GetDistanceString(distance.Value - Location.Tolerance);
                }
            }
            else
                TypeOfDistance = DistanceType.Unknown;
        }

        private string GetDistanceString(double distance)
        {
            return distance < 1000 ? $"{distance:0} m" : $"{(distance / 1000):0.0} km";
        }

        public async Task LoadDetailedData(IRestClient restClient)
        {
            await _person.LoadDetailedData(restClient);
        }

        public void ClearUnreadMessages()
        {
            UnreadMessages = null;
        }
    }
}

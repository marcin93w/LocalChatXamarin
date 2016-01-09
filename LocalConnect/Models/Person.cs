using System;
using System.Threading.Tasks;
using LocalConnect.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LocalConnect.Models
{
    [Serializable]
    public class Person
    {
        [JsonProperty("id")]
        public string PersonId { set; get; }
        public string FirstName { set; get; }
        public string Surname { set; get; }
        public string ShortDescription { set; get; }
        public JammedLocation Location { set; get; }
        public string Avatar { set; get; }
        public int? UnreadMessages { set; get; }


        public string LongDescription { set; get; }

        public Person()
        {
        }

        public Person(string firstName, string surname, string shortDescription, JammedLocation location, string personId)
        {
            FirstName = firstName;
            Surname = surname;
            ShortDescription = shortDescription;
            Location = location;
            PersonId = personId;
        }

        public static async Task<Person> LoadPerson(IRestClient restClient, string personId)
        {
            return await restClient.FetchDataAsync<Person>($"people/{personId}");
        }

        public async Task LoadDetailedData(IRestClient restClient)
        {
            var personData = (JContainer) await restClient.FetchDataAsync($"personDetails/{PersonId}");
            LongDescription = personData.Value<string>("longDescription");
        }

        public double? CalculateDistanceFrom(Me me)
        {
            if (Location == null || me.RealLocation == null)
                return null;

            return CalculateDistance(me.RealLocation, Location);
        }

        public double CalculateDistance(Location l1, Location l2)
        {
            //Haversine formula for calculating distance
            double R = 6371000;

            var latDist = (l2.Lat - l1.Lat) * Math.PI / 180;
            var lonDist = (l2.Lon - l1.Lon) * Math.PI / 180;
            var h1 = Math.Sin(latDist / 2) * Math.Sin(latDist / 2) +
                          Math.Cos(l1.Lat * Math.PI / 180) * Math.Cos(l2.Lat * Math.PI / 180) *
                          Math.Sin(lonDist / 2) * Math.Sin(lonDist / 2);
            var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));
            return R * h2;
        }
    }
}

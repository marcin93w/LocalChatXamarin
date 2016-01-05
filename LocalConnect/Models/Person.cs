using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
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

            var results = new float[1];
            global::Android.Locations.Location.DistanceBetween(
                me.RealLocation.Lat, me.RealLocation.Lon, Location.Lat, Location.Lon, results);

            return results[0];
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Android.Support.V4.View;
using Android.Widget;
using LocalConnect.Services;
using NetTopologySuite.Operation.Distance;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LocalConnect.Models
{
    [Serializable]
    public class Person
    {
        [JsonProperty("_id")]
        public string PersonId { set; get; }
        public string FirstName { set; get; }
        public string Surname { set; get; }
        public string ShortDescription { set; get; }
        public Location Location { set; get; }
        public string Avatar { set; get; }
        public int? UnreadMessages { set; get; }


        public string LongDescription { set; get; }

        public string Name => FirstName + " " + Surname;

        public Person()
        {
        }

        public Person(string firstName, string surname, string shortDescription, Location location, string personId)
        {
            FirstName = firstName;
            Surname = surname;
            ShortDescription = shortDescription;
            Location = location;
            PersonId = personId;
        }

        public async Task LoadDetailedData(IRestClient restClient)
        {
            var personData = (JContainer) await restClient.FetchDataAsync($"personDetails/{PersonId}");
            LongDescription = personData.Value<string>("longDescription");
        }

        public double? CalculateDistanceFrom(Person me)
        {
            if (Location == null || me.Location == null)
                return null;

            var results = new float[1];
            global::Android.Locations.Location.DistanceBetween(
                me.Location.Lat, me.Location.Lon, Location.Lat, Location.Lon, results);

            return results[0];
        }
    }
}

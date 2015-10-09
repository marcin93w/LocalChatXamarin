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
        [JsonProperty("_id")]
        public string PersonId { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string ShortDescription { get; }
        public Point Location { get; }


        public string LongDescription { private set; get; }

        public string Name => FirstName + " " + Surname;

        public Person(string firstName, string surname, string shortDescription, Point location, string personId)
        {
            FirstName = firstName;
            Surname = surname;
            ShortDescription = shortDescription;
            Location = location;
            PersonId = personId;
        }

        public async Task LoadDetailedData(IDataProvider dataProvider)
        {
            var personData = (JContainer) await dataProvider.FetchDataAsync($"personDetails/{PersonId}");
            LongDescription = personData.Value<string>("longDescription");
        }
    }
}

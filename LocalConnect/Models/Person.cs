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
        public string PersonId { set; get; }
        public string FirstName { set; get; }
        public string Surname { set; get; }
        public string ShortDescription { set; get; }
        public Point Location { set; get; }


        public string LongDescription { private set; get; }

        public string Name => FirstName + " " + Surname;

        public Person()
        {
        }

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

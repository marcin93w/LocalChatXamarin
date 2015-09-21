using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Threading.Tasks;
using LocalConnect.Services;
using NetTopologySuite.Geometries;

namespace LocalConnect.Models
{
    public class People
    {
        public List<Person> PeopleList { private set; get; }

        public async Task<bool> FetchPeopleList()
        {
            var jsonData = await RestClient.Instance.FetchDataAsync("people");

            PeopleList = jsonData.Cast<JsonValue>().Select(row =>
            {
                var location = row.ContainsKey("location")
                    ? new Point(row["location"]["lon"], row["location"]["lat"])
                    : null;
                return new Person(
                    row["firstname"],
                    row["surname"],
                    row["shortDescription"],
                    location,
                    row["_id"]);
            }).ToList();

            return PeopleList != null;
        }
    }
}
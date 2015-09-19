using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LocalConnect.Models;
using LocalConnect2.Services;
using NetTopologySuite.Geometries;
using Org.Json;

namespace LocalConnect2.Models
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
                    row["userId"]);
            }).ToList();

            return PeopleList != null;
        }
    }
}
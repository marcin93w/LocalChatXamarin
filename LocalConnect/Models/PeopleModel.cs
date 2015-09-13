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
using LocalConnect2.Services;
using Org.Json;

namespace LocalConnect2.Models
{
    public class Person
    {
        public string FirstName { get; }
        public string Surname { get; }
        public string Description { get; }

        public string Name => FirstName + " " + Surname;

        public Person(string firstName, string surname, string description)
        {
            FirstName = firstName;
            Surname = surname;
            Description = description;
        }
    }

    public class PeopleModel
    {
        public async Task<IEnumerable<Person>> FetchPeopleList()
        {
            var jsonData = await RestClient.Instance.FetchDataAsync("people");

            return from JsonValue row in jsonData
                select new Person(row["firstname"], row["surname"], row["shortDescription"]);
        }
    }
}
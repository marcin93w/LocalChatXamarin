using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LocalConnect.Services;
using NetTopologySuite.Geometries;

namespace LocalConnect.Models
{
    public class People
    {
        public List<Person> PeopleList { private set; get; }

        public People()
        {
            PeopleList = new List<Person>();
        }

        public async Task FetchPeopleList(IRestClient restClient)
        {
            PeopleList = await restClient.FetchDataAsync<List<Person>>("people");
        }
    }
}
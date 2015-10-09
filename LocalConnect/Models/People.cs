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

        public async Task FetchPeopleList(IDataProvider dataProvider)
        {
            PeopleList = await dataProvider.FetchDataAsync<List<Person>>("people");
        }
    }
}
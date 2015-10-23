using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class Me
    {
        public Person Person { private set; get; }

        public async Task FetchData(IDataProvider dataProvider)
        {
            Person = await dataProvider.FetchDataAsync<Person>("me");
        }
    }
}

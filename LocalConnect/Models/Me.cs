using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class Me
    {
        public Person Person { set; get; }

        public async Task FetchData(IRestClient restClient)
        {
            Person = await restClient.FetchDataAsync<Person>("me");
        }
    }
}

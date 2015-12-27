using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class Settings
    {
        public int LocationDisruption { get; set; }
        public int PeopleDisplayCount { get; set; }

        public static async Task<Settings> FetchSettings(IRestClient restClient)
        {
            return await restClient.FetchDataAsync<Settings>("me/settings");
        }

        public async Task SendUpdate(IRestClient restClient)
        {
            await restClient.PostDataAsync("me/settings/update", this);
        }
    }
}

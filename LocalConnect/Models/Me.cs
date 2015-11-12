using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class Me : Person
    {
        public event EventHandler LocationChanged;

        public async Task FetchData(IRestClient restClient)
        {
            var me = await restClient.FetchDataAsync<Person>("me");
            FirstName = me.FirstName;
            Surname = me.Surname;
            Avatar = me.Avatar;
            ShortDescription = me.ShortDescription;
            PersonId = me.PersonId;
        }

        public async Task UpdateLocation(IRestClient restClient, Location location)
        {
            await restClient.PostDataAsync("me/updateLocation", location);
            Location = location;
            LocationChanged?.Invoke(this, new EventArgs());
        }
    }
}

using System;
using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class Me : Person
    {
        public event EventHandler LocationChanged;

        public Location RealLocation { set; get; }

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
            RealLocation = location;
            LocationChanged?.Invoke(this, new EventArgs());
        }

        public async Task UpdateData(IRestClient restClient)
        {
            await restClient.PostDataAsync<Me>("me/update", this);
        }
    }
}

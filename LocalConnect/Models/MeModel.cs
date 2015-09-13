using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect2.Services;

namespace LocalConnect2.Models
{
    public class MeModel
    {
        public async Task<string> FetchMyName()
        {
            var jsonData = await RestClient.Instance.FetchDataAsync("me/name");

            return jsonData.GetValue("firstname") + " " + jsonData.GetValue("surname");
        }
    }
}
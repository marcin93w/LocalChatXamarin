using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Helpers;

namespace LocalConnectTest.Helpers
{
    public class FakeSessionInfoManager : ISessionInfoManager
    {
        public void SaveAuthToken(string authToken)
        {
            
        }

        public string ReadAuthToken()
        {
            return "asda";
        }

        public void DeleteSessionInfo()
        {
            
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Helpers;
using LocalConnect.Models;

namespace LocalConnectTest.Helpers
{
    public class FakeSessionInfoManager : ISessionInfoManager
    {
        public void SaveAuthToken(string authToken)
        {
            
        }

        public void SaveSessionInfo(SessionInfo sessionInfo)
        {
            throw new NotImplementedException();
        }

        public SessionInfo ReadSessionInfo()
        {
            throw new NotImplementedException();
        }

        public string ReadAuthToken()
        {
            return "asda";
        }

        public string ReadPersonId()
        {
            throw new NotImplementedException();
        }

        public void DeleteSessionInfo()
        {
            
        }
    }
}

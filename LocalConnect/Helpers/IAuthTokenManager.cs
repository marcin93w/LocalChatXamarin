using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Helpers
{
    public interface IAuthTokenManager
    {
        void SaveAuthToken(string authToken);
        string ReadAuthToken();
        void DeleteAuthToken();
    }
}

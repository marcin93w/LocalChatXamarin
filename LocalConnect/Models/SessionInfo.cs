using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class SessionInfo
    {
        public SessionInfo(string token, string userId)
        {
            Token = token;
            UserId = userId;
        }

        public string Token { set; get; }
        public string UserId { set; get; }
    }
}

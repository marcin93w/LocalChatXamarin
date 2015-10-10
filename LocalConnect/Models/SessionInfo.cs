using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class SessionInfo
    {
        public SessionInfo(string token, string personId)
        {
            Token = token;
            PersonId = personId;
        }

        public string Token { set; get; }
        public string PersonId { set; get; }
    }
}

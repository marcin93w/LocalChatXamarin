using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class LoginData
    {
        public LoginData(string token, string personId)
        {
            Token = token;
            PersonId = personId;
        }

        public string Token { get; }
        public string PersonId { get; }
    }
}

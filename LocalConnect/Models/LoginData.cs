using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class LoginData
    {
        public LoginData(string token, string userId)
        {
            Token = token;
            UserId = userId;
        }

        public string Token { get; }
        public string UserId { get; }
    }
}

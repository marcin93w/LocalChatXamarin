﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Services;

namespace LocalConnect.Models
{
    public class User
    {
        public User()
        {
            Person = new Person();
        }

        public string Username { set; get; }
        public string Password { set; get; }

        public Person Person { get; }

        public async Task<RegistrationInfo> Register(IDataProvider dataProvider)
        {
            return await dataProvider.PostDataAsync<User, RegistrationInfo>("register", this);
        }
    }
}

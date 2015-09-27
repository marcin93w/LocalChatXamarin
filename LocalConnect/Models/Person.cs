﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using LocalConnect.Services;
using Newtonsoft.Json;

namespace LocalConnect.Models
{
    [Serializable]
    public class Person
    {
        public string PersonId { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Description { get; }
        public Point Location { get; }


        public string LongDescription { private set; get; }

        public string Name => FirstName + " " + Surname;

        public Person(string firstName, string surname, string description, Point location, string personId)
        {
            FirstName = firstName;
            Surname = surname;
            Description = description;
            Location = location;
            PersonId = personId;
        }

        public async Task<bool> LoadDetailedData()
        {
            var personData = await RestClient.Instance.FetchDataAsync($"personDetails/{PersonId}");

            LongDescription = personData.GetValue("longDescription");

            return LongDescription != null;
        }
    }
}

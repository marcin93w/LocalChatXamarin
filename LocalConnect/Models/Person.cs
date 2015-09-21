﻿using System;
using System.Collections.Generic;
using System.Text;
using NetTopologySuite.Geometries;

namespace LocalConnect.Models
{
    public class Person
    {
        public string PersonId { get; }
        public string FirstName { get; }
        public string Surname { get; }
        public string Description { get; }
        public Point Location { get; }

        public string Name => FirstName + " " + Surname;

        public Person(string firstName, string surname, string description, Point location, string personId)
        {
            FirstName = firstName;
            Surname = surname;
            Description = description;
            Location = location;
            PersonId = personId;
        }
    }
}
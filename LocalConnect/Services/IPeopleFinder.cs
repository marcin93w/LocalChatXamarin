using System;
using System.Collections.Generic;
using System.Text;
using LocalConnect.Models;

namespace LocalConnect.Services
{
    public interface IPeopleFinder
    {
        Person GetPersonForId(string userId);
    }
}

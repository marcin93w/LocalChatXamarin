using System;
using System.Collections.Generic;
using System.Text;
using LocalConnect.Models;

namespace LocalConnect.Interfaces
{
    public interface IPeopleFinder
    {
        Person GetPersonForId(string personId);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LocalConnect.Services;

namespace LocalConnect.ViewModel
{

    public interface IRestClientUsingViewModel
    {
        IRestClient RestClient { set; }
    }

}
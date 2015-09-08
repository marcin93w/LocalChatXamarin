using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalConnect2.Services
{
    public class ConnectionException : Exception
    {
        public ConnectionException(Exception reason) : base("Connection to server failed", reason)
        {
        }

    }
}
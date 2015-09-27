using System;
using System.Collections.Generic;
using System.Text;

namespace LocalConnect.Models
{
    public class Point
    {
        public Point(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
        }

        public double Lon { get; }
        public double Lat { get; }
    }
}

﻿namespace LocalConnect.Models
{
    public class Location
    {
        public Location(double lon, double lat)
        {
            Lon = lon;
            Lat = lat;
        }

        public double Lon { get; }
        public double Lat { get; }
    }
}

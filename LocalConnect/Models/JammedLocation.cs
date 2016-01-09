namespace LocalConnect.Models
{
    public class JammedLocation : Location
    {
        public double Tolerance { get; }

        public JammedLocation(double lon, double lat, double disruption) : base(lon, lat)
        {
            Tolerance = disruption;
        }
    }
}

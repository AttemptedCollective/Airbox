namespace Airbox.Api.Core.Locations
{
    public class Location : ILocation
    {
        public Guid Id { get; private set; }

        public DateTimeOffset CreatedDateTimeOffset { get; private set; }

        public double Longitude { get; private set; }

        public double Latitude { get; private set; }

        public Location(double longitude, double latitude)
        {
            Id = Guid.NewGuid();
            CreatedDateTimeOffset = DateTimeOffset.UtcNow;
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}

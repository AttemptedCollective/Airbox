namespace Airbox.Api.Core.Locations
{
    public class UserLocation : ILocation
    {
        public Guid UserId { get; private set; }

        public Guid Id { get; private set; }

        public DateTimeOffset CreatedDateTimeOffset { get; private set; }

        public double Longitude { get; private set; }

        public double Latitude { get; private set; }

        public UserLocation(Guid userId, ILocation location)
        {
            UserId = userId;
            Id = location.Id;
            CreatedDateTimeOffset = location.CreatedDateTimeOffset;
            Longitude = location.Longitude ;
            Latitude = location.Latitude;
        }
    }
}

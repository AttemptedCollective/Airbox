namespace Airbox.Api.Core.Locations
{
    public interface ILocation
    {
        public Guid Id { get; }

        public DateTimeOffset CreatedDateTimeOffset { get; }

        public double Longitude { get; }

        public double Latitude { get; }
    }
}
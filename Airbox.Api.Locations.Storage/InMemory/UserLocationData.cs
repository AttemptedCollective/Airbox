using Airbox.Api.Core.Locations;

namespace Airbox.Api.Locations.Storage.InMemory
{
    internal class UserLocationData
    {
        // TODO Can ILocation be read only?

        private readonly List<ILocation> _locations;

        public IReadOnlyList<ILocation> Locations { get => _locations; }

        public ILocation LatestLocation { get; private set; }

        public UserLocationData(ILocation location)
        {
            _locations = new List<ILocation> { location };
            LatestLocation = location;
        }

        public void AddLocation(ILocation location)
        {
            _locations.Add(location);
            LatestLocation = location;
        }
    }
}

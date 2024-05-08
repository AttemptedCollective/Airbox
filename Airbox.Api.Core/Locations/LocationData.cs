namespace Airbox.Api.Core.Locations
{
    public class LocationData : ILocationData
    {
        /// <inheritdoc/>
        public Stack<ILocation> LocationHistory { get; private set; }

        /// <summary>
        /// Create a <see cref="LocationData"/>.
        /// </summary>
        public LocationData()
        {
            LocationHistory = new Stack<ILocation>();
        }

        /// <summary>
        /// Create a <see cref="LocationData"/>.
        /// </summary>
        /// <param name="initialLocation">The location to initialise the location history with.</param>
        public LocationData(ILocation initialLocation)
        {
            LocationHistory = new Stack<ILocation>();
            LocationHistory.Push(initialLocation);
        }

        /// <inheritdoc/>
        public ILocation? GetMostRecentLocation() => LocationHistory.Count > 0 ? LocationHistory.Peek() : null;

        /// <inheritdoc/>
        public bool TryGetRecentLocationWithinArea(IList<ILocation> areaPoints, out ILocation? mostRecentLocation)
        {
            throw new NotImplementedException();
        }
    }
}

namespace Airbox.Api.Core.Locations
{
    public interface ILocationData
    {
        /// <summary>
        /// Location history, organised as a stick for quick access to the most recent.
        /// </summary>
        public Stack<ILocation> LocationHistory { get; }

        /// <summary>
        /// Get the most recent location stored.
        /// </summary>
        /// <returns>The most recent location stored in the <see cref="LocationHistory"/>.</returns>
        public ILocation? GetMostRecentLocation();

        /// <summary>
        /// Try and get the most recent location stored if it is within a given defined geographical location.
        /// </summary>
        /// <param name="areaPoints">List of <see cref="ILocation"/> that the most recent stored location must be in.</param>
        /// <param name="mostRecentLocation">Out parameter that contains the most recent <see cref="ILocation"/>.
        /// This will be null if the location is not within the given geographical area defined by <paramref name="areaPoints"/>.</param>
        /// <returns>True if the most recent area is within the given geographical area, and false if it is not.</returns>
        public bool TryGetRecentLocationWithinArea(IList<ILocation> areaPoints, out ILocation? mostRecentLocation);
    }
}

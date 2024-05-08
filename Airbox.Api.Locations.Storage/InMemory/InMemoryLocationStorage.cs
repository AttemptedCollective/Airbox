using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;

namespace Airbox.Api.Locations.Storage.InMemory
{
    /// <summary>
    /// Implementation of the <see cref="ILocationStorage"/> that stores user location in memory.
    /// </summary>
    public class InMemoryLocationStorage : ILocationStorage
    {
        private readonly Dictionary<Guid, UserLocationData> _allUserLocationData = new Dictionary<Guid, UserLocationData>();

        /// <inheritdoc/>
        public void AddUserLocation(Guid userId, ILocation location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (!_allUserLocationData.ContainsKey(userId))
            {
                _allUserLocationData.Add(userId, new UserLocationData(location));
            }
            else
            {
                var userLocationData = _allUserLocationData[userId];
                userLocationData.AddLocation(location);
            }
        }
        /// <inheritdoc/>
        public IReadOnlyList<ILocation>? GetUserLocations(Guid userId)
        {
            if (_allUserLocationData.TryGetValue(userId, out var userLocationData) && userLocationData is not null)
            {
                var orderedData = userLocationData.Locations.OrderByDescending(_ => _.CreatedDateTimeOffset);
                return orderedData.ToList();
            }

            return null;
        }

        /// <inheritdoc/>
        public PagedList<ILocation>? GetPagedUserLocations(Guid userId, PageParameters pageParameters)
        {
            if (_allUserLocationData.TryGetValue(userId, out var userLocationData) && userLocationData is not null)
            {
                var orderedData = userLocationData.Locations.OrderByDescending(_ => _.CreatedDateTimeOffset);

                return orderedData.ToPagedList(pageParameters.PageSize, pageParameters.PageNumber);
            }

            return null;
        }

        /// <inheritdoc/>
        public ILocation? GetLatestUserLocation(Guid userId)
        {
            if (_allUserLocationData.TryGetValue(userId, out var userLocationData) && userLocationData is not null)
            {
                return userLocationData.LatestLocation;
            }

            return null;
        }

        /// <inheritdoc/>
        public IReadOnlyList<ILocation> GetLatestLocationsForAllUsers()
            => _allUserLocationData.Values.Select(userLocationData => userLocationData.LatestLocation).ToList();

        /// <inheritdoc/>
        public PagedList<ILocation> GetPagedLatestLocationsForAllUsers(PageParameters pageParameters)
            => _allUserLocationData.Values
                .Select(userLocationData => userLocationData.LatestLocation)
                .ToPagedList(pageParameters.PageSize, pageParameters.PageNumber);
    }
}

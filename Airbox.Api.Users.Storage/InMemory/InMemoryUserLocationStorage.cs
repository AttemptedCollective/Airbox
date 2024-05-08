using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;
using Airbox.Api.Core.Storage;
using Airbox.Api.Core.Users;
using System.Collections.Concurrent;

namespace Airbox.Api.Users.Storage.InMemory
{
    /// <summary>
    /// Implementation of the <see cref="IUserStorage"/> that stores user location in memory.
    /// </summary>
    public class InMemoryUserLocationStorage : IUserStorage, ILocationStorage
    {
        private readonly ConcurrentDictionary<Guid, IUser> _users = new ConcurrentDictionary<Guid, IUser>();
        private readonly ConcurrentDictionary<Guid, ILocationData> _userLocationData = new ConcurrentDictionary<Guid, ILocationData>();

        /// <inheritdoc/>
        public Task<bool> ContainsUser(Guid userId) => Task.FromResult(_users.ContainsKey(userId));

        /// <inheritdoc/>
        public Task AddUser(IUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            if (!_users.ContainsKey(user.Id))
            {
                _users.TryAdd(user.Id, user);
                _userLocationData.TryAdd(user.Id, new LocationData()); 
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task AddUserLocation(Guid userId, ILocation location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (_userLocationData.ContainsKey(userId))
            {
                _userLocationData.AddOrUpdate(
                    userId,
                    _ => new LocationData(location),
                    (_, locationData) => { locationData.LocationHistory.Push(location); return locationData; });
            }

            return Task.CompletedTask;
        }
        /// <inheritdoc/>
        public Task<IReadOnlyList<UserLocation>?> GetUserLocations(Guid userId)
        {
            if (_userLocationData.TryGetValue(userId, out var userLocationData) && userLocationData is not null)
            {
                return Task.FromResult<IReadOnlyList<UserLocation>?>(
                    userLocationData.LocationHistory
                    .Select(location => new UserLocation(userId, location))
                    .ToList());
            }

            return Task.FromResult<IReadOnlyList<UserLocation>?>(null);
        }

        /// <inheritdoc/>
        public Task<PagedList<UserLocation>?> GetPagedUserLocations(Guid userId, PageParameters pageParameters)
        {
            if (_userLocationData.TryGetValue(userId, out var userLocationData) && userLocationData is not null)
            {
                return Task.FromResult<PagedList<UserLocation>?>(
                    userLocationData.LocationHistory
                    .Select(location => new UserLocation(userId, location))
                    .ToPagedList(pageParameters.PageSize, pageParameters.PageNumber));
            }

            return Task.FromResult<PagedList<UserLocation>?>(null); ;
        }

        /// <inheritdoc/>
        public Task<UserLocation?> GetLatestUserLocation(Guid userId)
        {
            if (_userLocationData.TryGetValue(userId, out var userLocationData)
                && userLocationData is not null
                && TryGetMostRecentLocationData(userLocationData, out var location)
                && location is not null)
            {
                return Task.FromResult<UserLocation?>(new UserLocation(userId, location));
            }

            return Task.FromResult<UserLocation?>(null);
        }

        /// <inheritdoc/>
        public Task<IReadOnlyList<UserLocation>> GetLatestLocationsForAllUsers()
        {
            var allRecentLocations = new List<UserLocation>();

            foreach (var user in _users.Values)
            {
                if (_userLocationData.TryGetValue(user.Id, out var locationData)
                    && TryGetMostRecentLocationData(locationData, out var location)
                    && location is not null)
                {
                    allRecentLocations.Add(new UserLocation(user.Id, location));
                }
            }

            return Task.FromResult<IReadOnlyList<UserLocation>> (allRecentLocations);
        }

        /// <inheritdoc/>
        public async Task<PagedList<UserLocation>> GetPagedLatestLocationsForAllUsers(PageParameters pageParameters)
        {
            var allRecentLocations = await GetLatestLocationsForAllUsers().ConfigureAwait(false);

            return allRecentLocations.ToPagedList(pageParameters.PageSize, pageParameters.PageNumber);
        }

        private bool TryGetMostRecentLocationData(ILocationData locationData, out ILocation? location)
        {
            location = locationData.GetMostRecentLocation();
            return location is not null;
        }
    }
}

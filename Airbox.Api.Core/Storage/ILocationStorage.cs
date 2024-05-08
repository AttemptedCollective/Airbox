using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;
using Airbox.Api.Core.Users;

namespace Airbox.Api.Core.Storage
{
    public interface ILocationStorage
    {
        /// <summary>
        /// Add a new location for the given user.
        /// </summary>
        /// <param name="userId">The Id of the user the location is being added for.</param>
        /// <returns>The Id of the newly created location.</returns>
        /// <remarks>This always creates a new location and does not update an existing location.</remarks>
        public Task AddUserLocation(Guid userId, ILocation location);

        /// <summary>
        /// Get all of the locations saved for a given user.
        /// </summary>
        /// <param name="userId">The id of the user to retrieve locations for.</param>
        /// <returns>
        /// A task that when completed returns a list of locations stored for the user.
        /// Returns null if the user does not exist.
        /// </returns>
        public Task<IReadOnlyList<UserLocation>?> GetUserLocations(Guid userId);

        /// <summary>
        /// Get all of the locations saved for a given user.
        /// </summary>
        /// <param name="userId">The id of the user to retrieve locations for.</param>
        /// <param name="pageParameters">Paramters used to paginate the query.</param>
        /// <returns>
        /// A task that when completed returns a paged list of locations stored for the user.
        /// Returns null if the user does not exist.
        /// </returns>
        public Task<PagedList<UserLocation>?> GetPagedUserLocations(Guid userId, PageParameters pageParameters);

        /// <summary>
        /// Get the most recent location saved for a given user.
        /// </summary>
        /// <param name="userId">The id of the user to retrieve the most recent location.</param>
        /// <returns>
        /// A task that when completed returns a the most recent location stored for the user.
        /// Returns null if the user does not exist.
        /// </returns>
        public Task<UserLocation?> GetLatestUserLocation(Guid userId);

        /// <summary>
        /// Gets the most recent locations saved for all users.
        /// </summary>
        /// <returns>
        /// A task that when completed returns a list of users and their most recent locations.
        /// </returns>
        public Task<IReadOnlyList<UserLocation>> GetLatestLocationsForAllUsers();

        /// <summary>
        /// Gets the most recent locations saved for all users.
        /// </summary>
        /// <param name="pageParameters">Paramters used to paginate the query.</param>
        /// <returns>
        /// A task that when completed returns a paged list of users and their most recent locations.
        /// </returns>
        public Task<PagedList<UserLocation>> GetPagedLatestLocationsForAllUsers(PageParameters pageParameters);
    }
}

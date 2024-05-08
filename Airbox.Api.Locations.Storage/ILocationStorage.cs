using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;

namespace Airbox.Api.Locations.Storage
{
    public interface ILocationStorage
    {
        /// <summary>
        /// Add a new location for the given user.
        /// </summary>
        /// <param name="userId">The Id of the user the location is being added for.</param>
        /// <param name="longitude">The longitude of the location.</param>
        /// <param name="latitude">The latitude of the location.</param>
        /// <returns>The Id of the newly created location.</returns>
        /// <remarks>This always creates a new location and does not update an existing location.</remarks>
        public void AddUserLocation(Guid userId, ILocation location);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public IReadOnlyList<ILocation>? GetUserLocations(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageParameters"></param>
        /// <returns></returns>
        public PagedList<ILocation>? GetPagedUserLocations(Guid userId, PageParameters pageParameters);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public ILocation? GetLatestUserLocation(Guid userId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<ILocation> GetLatestLocationsForAllUsers();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageParameters"></param>
        /// <returns></returns>
        public PagedList<ILocation> GetPagedLatestLocationsForAllUsers(PageParameters pageParameters);
    }
}

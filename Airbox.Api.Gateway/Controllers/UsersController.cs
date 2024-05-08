using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;
using Airbox.Api.Core.Storage;
using Airbox.Api.Core.Users;
using Microsoft.AspNetCore.Mvc;

namespace API.Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserStorage _userStorage;
        private readonly ILocationStorage _locationStorage;

        public UsersController(IUserStorage userStorage, ILocationStorage locationStorage)
        {
            _userStorage = userStorage;
            _locationStorage = locationStorage;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IResult> AddUser([FromBody] User user)
        {
            await _userStorage.AddUser(user).ConfigureAwait(false);

            return Results.Created($"{user.Id}", user);
        }

        [HttpPost]
        [Route("{userId:guid}/locations")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesDefaultResponseType]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<IResult> AddLocationForUser(Guid userId, [FromBody] Location location)
        {
            if (!await _userStorage.ContainsUser(userId).ConfigureAwait(false))
            {
                return Results.NotFound();
            }

            await _locationStorage.AddUserLocation(userId, location).ConfigureAwait(false);

            return Results.Created($"{userId}/locations/{location.Id}", location);
        }

        [HttpGet]
        [Route("{userId:guid}/locations/all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Produces("application/json")]
        public async Task<IResult> GetAllLocationsForUser(Guid userId)
        {
            if (!await _userStorage.ContainsUser(userId).ConfigureAwait(false))
            {
                return Results.NotFound();
            }

            var userLocations = await _locationStorage.GetUserLocations(userId).ConfigureAwait(false);

            return userLocations is null ? Results.NotFound() : Results.Ok(userLocations);
        }

        [HttpGet]
        [Route("{userId:guid}/locations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Produces("application/json")]

        public async Task<IResult> GetPagedLocationsForUser(Guid userId, [FromQuery] PageParameters pageParameters)
        {
            if (!await _userStorage.ContainsUser(userId).ConfigureAwait(false))
            {
                return Results.NotFound();
            }

            var userLocations = await _locationStorage.GetPagedUserLocations(userId, pageParameters).ConfigureAwait(false);
            HttpContext.Response.AddPaginationHeader(userLocations);

            return userLocations is null ? Results.NotFound() : Results.Ok(userLocations);
        }

        [HttpGet]
        [Route("{userId:guid}/locations/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [Produces("application/json")]
        public async Task<IResult> GetLatestLocationForUser(Guid userId)
        {
            if (!await _userStorage.ContainsUser(userId).ConfigureAwait(false))
            {
                return Results.NotFound();
            }

            var latestUserLocation = await _locationStorage.GetLatestUserLocation(userId).ConfigureAwait(false);

            return latestUserLocation is null ? Results.NotFound() : Results.Ok(latestUserLocation);
        }

        [HttpGet]
        [Route("locations/all/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [Produces("application/json")]
        public async Task<IResult> GetLatestLocationForAllUsers()
        {
            var latestLocations = await _locationStorage.GetLatestLocationsForAllUsers().ConfigureAwait(false);

            return Results.Ok(latestLocations);
        }

        [HttpGet]
        [Route("locations/latest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [Produces("application/json")]
        public async Task<IResult> GetPagedLatestLocationForAllUsers([FromQuery] PageParameters pageParameters)
        {
            var latestLocations = await _locationStorage.GetPagedLatestLocationsForAllUsers(pageParameters).ConfigureAwait(false);
            
            HttpContext.Response.AddPaginationHeader(latestLocations);

            return Results.Ok(latestLocations);
        }
    }
}

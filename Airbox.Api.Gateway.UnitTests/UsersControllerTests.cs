using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;
using Airbox.Api.Core.Storage;
using Airbox.Api.Core.Users;
using API.Test.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;

namespace Airbox.APIGateway.UnitTests
{
    public class UsersControllerTests
    {
        private Mock<IUserStorage> _mockUserStorage;
        private Mock<ILocationStorage> _mockLocationStorage;
        private UsersController _usersController;

        [SetUp]
        public void Setup()
        {
            _mockUserStorage = new Mock<IUserStorage>();
            _mockUserStorage.Setup(_ => _.ContainsUser(It.IsAny<Guid>()))
                .Returns(Task.FromResult(true));

            _mockLocationStorage = new Mock<ILocationStorage>();
            _usersController = new UsersController(_mockUserStorage.Object, _mockLocationStorage.Object);
            _usersController.ControllerContext.HttpContext = new DefaultHttpContext();
        }

        [Test]
        public async Task UsersControllerTests_AddUser_CreateResultReturnedCorrectly()
        {
            var user = new User("testUser");
            _mockUserStorage.Setup(_ => _.AddUser(It.IsAny<User>()));

            var result = await _usersController.AddUser(user);

            _mockUserStorage.Verify(_ => _.AddUser(user));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Created<User>>());

            var createdResult = result as Created<User>;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.Value, Is.Not.Null);
            Assert.That(createdResult.Value.Id, Is.EqualTo(user.Id));
            Assert.That(createdResult.Location, Is.EqualTo($"{user.Id}"));
        }

        [Test]
        public async Task UsersControllerTests_AddLocationForUser_CreateResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var location = new Location(1, 2);

            _mockLocationStorage.Setup(_ => _.AddUserLocation(It.IsAny<Guid>(), It.IsAny<ILocation>()));

            var result = await _usersController.AddLocationForUser(userId, location);

            _mockLocationStorage.Verify(_ => _.AddUserLocation(userId, location));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Created<Location>>());

            var createdResult = result as Created<Location>;
            Assert.That(createdResult, Is.Not.Null);
            Assert.That(createdResult.Value, Is.Not.Null);
            Assert.That(createdResult.Value.Id, Is.EqualTo(location.Id));
            Assert.That(createdResult.Location, Is.EqualTo($"{userId}/locations/{location.Id}"));
        }

        [Test]
        public async Task UsersControllerTests_GetAllLocationsForUser_NotFoundResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();

            _mockLocationStorage
                .Setup(_ => _.GetUserLocations(It.IsAny<Guid>()))
                .Returns(Task.FromResult(default(IReadOnlyList<UserLocation>?)));

            var result = await _usersController.GetAllLocationsForUser(userId);

            _mockLocationStorage.Verify(_ => _.GetUserLocations(userId));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<NotFound>());
        }

        [Test]
        public async Task UsersControllerTests_GetAllLocationsForUser_OkResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var location = new Location(1, 2);
            var userLocation = new UserLocation(userId, location);

            _mockLocationStorage
                .Setup(_ => _.GetUserLocations(It.IsAny<Guid>()))
                .Returns(Task.FromResult<IReadOnlyList<UserLocation>?>(new List<UserLocation> { userLocation }));

            var result = await _usersController.GetAllLocationsForUser(userId);

            _mockLocationStorage.Verify(_ => _.GetUserLocations(userId));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Ok<IReadOnlyList<UserLocation>>>());
            var okResult = result as Ok<IReadOnlyList<UserLocation>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Does.Contain(userLocation));
        }

        [Test]
        public async Task UsersControllerTests_GetPagedLocationsForUser_NotFoundResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var pageParameters = new PageParameters()
            {
                PageNumber = 1,
                PageSize = 10,
            };

            _mockLocationStorage
                .Setup(_ => _.GetPagedUserLocations(It.IsAny<Guid>(), It.IsAny<PageParameters>()))
                .Returns(Task.FromResult(default(PagedList<UserLocation>)));

            var result = await _usersController.GetPagedLocationsForUser(userId, pageParameters);

            _mockLocationStorage.Verify(_ => _.GetPagedUserLocations(userId, pageParameters));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<NotFound>());
        }

        [Test]
        public async Task UsersControllerTests_GetPagedLocationsForUser_OkResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var location = new Location(1, 2);
            var userLocation = new UserLocation(userId, location);
            var pageParameters = new PageParameters()
            {
                PageNumber = 1,
                PageSize = 10,
            };

            _mockLocationStorage
                .Setup(_ => _.GetPagedUserLocations(It.IsAny<Guid>(), It.IsAny<PageParameters>()))
                .Returns(Task.FromResult<PagedList<UserLocation>?>(new PagedList<UserLocation>(new List<UserLocation> { userLocation }, 10, 1)));

            var result = await _usersController.GetPagedLocationsForUser(userId, pageParameters);

            _mockLocationStorage.Verify(_ => _.GetPagedUserLocations(userId, pageParameters));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Ok<PagedList<UserLocation>>>());
            var okResult = result as Ok<PagedList<UserLocation>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Does.Contain(userLocation));

            Assert.That(_usersController.HttpContext.Response.Headers, Does.ContainKey("pagination"));
        }

        [Test]
        public async Task UsersControllerTests_GetLatestLocationForUser_OkResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var location = new Location(1, 2);
            var userLocation = new UserLocation(userId, location);

            _mockLocationStorage
                .Setup(_ => _.GetLatestUserLocation(It.IsAny<Guid>()))
                .Returns(Task.FromResult<UserLocation?>(userLocation));

            var result = await _usersController.GetLatestLocationForUser(userId);

            _mockLocationStorage.Verify(_ => _.GetLatestUserLocation(userId));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Ok<UserLocation>>());
            var okResult = result as Ok<UserLocation>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.EqualTo(userLocation));
        }

        [Test]
        public async Task UsersControllerTests_GetLatestLocationForUser_NotFoundResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();

            _mockLocationStorage
                .Setup(_ => _.GetLatestUserLocation(It.IsAny<Guid>()))
                .Returns(Task.FromResult(default(UserLocation)));

            var result = await _usersController.GetLatestLocationForUser(userId);

            _mockLocationStorage.Verify(_ => _.GetLatestUserLocation(userId));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<NotFound>());
        }

        [Test]
        public async Task UsersControllerTests_GetLatestLocationsForAllUsers_OkResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var location = new Location(1, 2);
            var userLocation = new UserLocation(userId, location);

            _mockLocationStorage
                .Setup(_ => _.GetLatestLocationsForAllUsers())
                .Returns(Task.FromResult<IReadOnlyList<UserLocation>>(new List<UserLocation> { userLocation }));

            var result = await _usersController.GetLatestLocationForAllUsers();

            _mockLocationStorage.Verify(_ => _.GetLatestLocationsForAllUsers());
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Ok<IReadOnlyList<UserLocation>>>());
            var okResult = result as Ok<IReadOnlyList<UserLocation>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Does.Contain(userLocation));
        }

        [Test]
        public async Task UsersControllerTests_GetPagedLatestLocationForAllUsers_OkResultReturnedCorrectly()
        {
            var userId = Guid.NewGuid();
            var location = new Location(1, 2);
            var userLocation = new UserLocation(userId, location);
            var pageParameters = new PageParameters()
            {
                PageNumber = 1,
                PageSize = 10,
            };

            _mockLocationStorage
                .Setup(_ => _.GetPagedLatestLocationsForAllUsers(pageParameters))
                .Returns(Task.FromResult(new PagedList<UserLocation>(new List<UserLocation> { userLocation }, 10, 1)));

            var result = await _usersController.GetPagedLatestLocationForAllUsers(pageParameters);

            _mockLocationStorage.Verify(_ => _.GetPagedLatestLocationsForAllUsers(pageParameters));
            _mockLocationStorage.VerifyNoOtherCalls();

            Assert.That(result, Is.TypeOf<Ok<PagedList<UserLocation>>>());
            var okResult = result as Ok<PagedList<UserLocation>>;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Has.Count.EqualTo(1));
            Assert.That(okResult.Value, Does.Contain(userLocation));

            Assert.That(_usersController.HttpContext.Response.Headers, Does.ContainKey("pagination"));
        }
    }
}
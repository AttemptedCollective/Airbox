using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Pagination;
using Airbox.Api.Core.Users;
using Airbox.Api.Users.Storage.InMemory;
using Airbox.Api.Users.Storage.UnitTests;
using Newtonsoft.Json;

namespace Airbox.Locations.Storage.UnitTests
{
    [TestFixture]
    public class InMemoryUserLocationStorageTests
    {
        private static User _user1 = new User("TestUser1");
        private static User _user2 = new User("TestUser2");
        private static User _user3 = new User("TestUser3");

        private static Location _location1 = new Location(1, 2);
        private static Location _location2 = new Location(3, 4);
        private static List<Location> _locationList = new List<Location> { _location1, _location2 };

        private InMemoryUserLocationStorage _inMemoryUserLocationStorage;

        [SetUp]
        public void Setup()
        {
            _inMemoryUserLocationStorage = new InMemoryUserLocationStorage();
        }

        [Test]
        public void InMemoryLocationStorageTests_AddUserLocation_AddNullLocation_ThrowsException()
        {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. - null reference required for test
            // Cannot assume nullable is enabled in calling location
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _inMemoryUserLocationStorage.AddUserLocation(Guid.Empty, null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.That(exception.Message, Does.Contain("location").IgnoreCase);
        }

        [Test]
        public async Task InMemoryLocationStorageTests_GetLatestUserLocation_UserDoesNotExist_NullReturned()
        {
            var result = await _inMemoryUserLocationStorage.GetLatestUserLocation(Guid.Empty);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task InMemoryLocationStorageTests_GetUserLocations_UserDoesNotExist_NullReturned()
        {
            var result = await _inMemoryUserLocationStorage.GetUserLocations(Guid.Empty);
            Assert.That(result, Is.Null);
        }

        private static List<TestCaseData> _addUserLocationTestCases = new List<TestCaseData>
        {
            new TestCaseData(new List<Location>{ _location1}).SetName("{m}_SingleLocationAddedCorrectly"),
            new TestCaseData(new List<Location>{ _location1,  _location1}).SetName("{m}_MultipleLocationsAddedCorrectly")
        };

        [TestCaseSource(nameof(_addUserLocationTestCases))]
        public async Task InMemoryLocationStorageTests_AddUserLocation(List<Location> locationsToAdd)
        {
            await AddUsersForTesting();
            foreach (var location in locationsToAdd)
            {
                await _inMemoryUserLocationStorage.AddUserLocation(_user1.Id, location);
            }

            var userLocations = await _inMemoryUserLocationStorage.GetUserLocations(_user1.Id);
            Assert.That(userLocations, Has.Count.EqualTo(locationsToAdd.Count));

            var latestLocation = await _inMemoryUserLocationStorage.GetLatestUserLocation(_user1.Id);
            Assert.That(latestLocation, Is.EqualTo(new UserLocation(_user1.Id, locationsToAdd.Last())).Using(new JsonComparer()));
        }

        private static List<TestCaseData> _getAllLatestUserLocationTestCases = new List<TestCaseData>
        {
            new TestCaseData(new Dictionary<Guid, List<Location>>())
                .SetName("{m}_NoUsersExist_ReturnsEmptyList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } }
                }).SetName("{m}_SingleUserExists_ReturnSingleEntryListWithLatest"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } },
                    { _user3.Id, new List<Location> { _location1, _location2 } }
                }).SetName("{m}_MultipleUsersExist_ReturnMultipleEntryListWithLatest")
        };

        [TestCaseSource(nameof(_getAllLatestUserLocationTestCases))]
        public async Task InMemoryLocationStorageTests_GetLatestLocationsForAllUsers(Dictionary<Guid, List<Location>> locationsToAdd)
        {
            _inMemoryUserLocationStorage = new InMemoryUserLocationStorage();
            await AddUsersForTesting();

            foreach (var userLocations in locationsToAdd)
            {
                foreach (var location in userLocations.Value)
                {
                    await _inMemoryUserLocationStorage.AddUserLocation(userLocations.Key, location);
                }
            }

            Assert.That(await _inMemoryUserLocationStorage.GetLatestLocationsForAllUsers(), Has.Count.EqualTo(locationsToAdd.Count));
            Assert.Multiple(async () =>
            {
                foreach (var location in locationsToAdd.Select(_ => _.Value.Last()))
                {
                    var latestLocations = await _inMemoryUserLocationStorage.GetLatestLocationsForAllUsers();
                    Assert.That(latestLocations, Contains.Item(new UserLocation(_user1.Id, location)).Using(new JsonComparer()));
                }
            });
        }

        private static List<TestCaseData> _getPagedLatestUserLocationTestCases = new List<TestCaseData>
        {
            new TestCaseData(
                new Dictionary<Guid, List<Location>>(),
                new PageParameters(),
                new PagedList<UserLocation>(new List<UserLocation>(), 10, 1))
                .SetName("{m}_NoUsersExist_ReturnEmptyList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, _locationList }
                },
                new PageParameters(){ PageNumber = 1, PageSize = 1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2) }, 1, 1))
            .SetName("{m}_SingleUserExistsGetFirstPage_ReturnCorrectList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, _locationList }
                },
                new PageParameters(){ PageNumber = 20, PageSize = 1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2) }, 1, 20))
            .SetName("{m}_SingleUserExistsGetPageOutOfBounds_ReturnEmptyList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } }
                },
                new PageParameters(){ PageNumber = 1, PageSize = 1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2), new UserLocation(_user2.Id, _location2) }, 1, 1))
            .SetName("{m}_MultipleUsersExist_ReturnCorrectListForFirstPage"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } }
                },
                new PageParameters(){ PageNumber = 2, PageSize = 1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2), new UserLocation(_user2.Id, _location2) }, 1, 2))
            .SetName("{m}_MultipleUsersExist_ReturnCorrectListForSecondPage"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } }
                },
                new PageParameters(){ PageNumber = 20, PageSize = 1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2), new UserLocation(_user2.Id, _location2) }, 1, 20))
            .SetName("{m}_MultipleUsersExistGetPageOutOfBounds_ReturnEmptyList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } }
                },
                new PageParameters(){ PageNumber = -1, PageSize = 1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2), new UserLocation(_user2.Id, _location2) }, 1, 1))
            .SetName("{m}_OutOfBoundsNegativePageNumber_ReturnCorrectList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } }
                },
                new PageParameters(){ PageNumber = 1, PageSize = -1},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2), new UserLocation(_user2.Id, _location2) }, 1, 1))
            .SetName("{m}_OutOfBoundsNegativePageSize_ReturnCorrectList"),
            new TestCaseData(
                new Dictionary<Guid, List<Location>>{
                    { _user1.Id, new List<Location> { _location1, _location2 } },
                    { _user2.Id, new List<Location> { _location1, _location2 } }
                },
                new PageParameters(){ PageNumber = 1, PageSize = 100},
                new PagedList<UserLocation>(new List<UserLocation> { new UserLocation(_user1.Id, _location2), new UserLocation(_user2.Id, _location2) }, 20, 1))
            .SetName("{m}_OutOfBoundsPositivePageSize_ReturnCorrectList")
        };

        [TestCaseSource(nameof(_getPagedLatestUserLocationTestCases))]
        public async Task InMemoryLocationStorageTests_GetPagedLatestLocationsForAllUsers(
            Dictionary<Guid, List<Location>> locationsToAdd,
            PageParameters pageParameters,
            PagedList<UserLocation> expected)
        {
            await AddUsersForTesting();
            foreach (var userLocations in locationsToAdd)
            {
                foreach (var location in userLocations.Value)
                {
                    await _inMemoryUserLocationStorage.AddUserLocation(userLocations.Key, location);
                }
            }

            var actual = await _inMemoryUserLocationStorage.GetPagedLatestLocationsForAllUsers(pageParameters);

            Assert.That(actual, Has.Count.EqualTo(expected.Count));
            Assert.That(actual, Is.EqualTo(expected).Using(new JsonComparer()));
        }

        private async Task AddUsersForTesting()
        {
            await _inMemoryUserLocationStorage.AddUser(_user1);
            await _inMemoryUserLocationStorage.AddUser(_user2);
            await _inMemoryUserLocationStorage.AddUser(_user3);
        }
    }
}
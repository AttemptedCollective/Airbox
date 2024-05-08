using Airbox.Api.Core.Locations;
using Airbox.Api.Core.Users;
using Airbox.Api.Users.Storage.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airbox.Api.Users.Storage.UnitTests
{
    [TestFixture]
    internal class InMemoryUserLocationStorageUserTests
    {
        private static User _user = new User("TestUser");
        private InMemoryUserLocationStorage _inMemoryUserLocationStorage;

        [SetUp]
        public void Setup()
        {
            _inMemoryUserLocationStorage = new InMemoryUserLocationStorage();
        }

        [Test]
        public async Task InMemoryLocationStorage_AddUser_UserAddedCorrectly()
        {
            Assert.That(await _inMemoryUserLocationStorage.ContainsUser(_user.Id), Is.False);
            await _inMemoryUserLocationStorage.AddUser(_user);
            Assert.That(await _inMemoryUserLocationStorage.ContainsUser(_user.Id), Is.True);
        }
    }
}

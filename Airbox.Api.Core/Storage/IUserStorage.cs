using Airbox.Api.Core.Users;

namespace Airbox.Api.Core.Storage
{
    public interface IUserStorage
    {
        /// <summary>
        /// Check if the storage contains a user.
        /// </summary>
        /// <param name="userId">The id of the user to chekc for.</param>
        /// <returns>True if the user exists and false if they do not.</returns>
        public Task<bool> ContainsUser(Guid userId);

        /// <summary>
        /// Add a user to the storage.
        /// </summary>
        /// <param name="user">The user to add to the storage.</param>
        /// <returns>A task representing when the user has been added.</returns>
        public Task AddUser(IUser user);
    }
}

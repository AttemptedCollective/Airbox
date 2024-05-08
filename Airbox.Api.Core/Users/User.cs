namespace Airbox.Api.Core.Users
{
    public class User : IUser
    {
        /// <inheritdoc/>
        public Guid Id { get; private set; }

        /// <inheritdoc/>
        public string UserName { get; private set; }

        /// <summary>
        /// Create a <see cref="User"/>.
        /// </summary>
        /// <param name="userName">The username.</param>
        /// <remarks>An Id will be created automatically for the user.</remarks>
        public User(string userName)
        {
            Id = Guid.NewGuid();
            UserName = userName;
        }
    }
}

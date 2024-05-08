namespace Airbox.Api.Core.Users
{
    public interface IUser
    {
        public Guid Id { get; }

        public string UserName { get; }
    }
}

namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public class SystemUserIdentity : ICurrentIdentity
    {
        public SystemUserIdentity()
        {
            var user = new SystemUser();
            this.UserId = user.Id;
            this.Username = user.Username;
        }

        public Guid UserId { get; }

        public string? Username { get; }
    }
}

namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromNoAuth : ICurrentIdentity
    {
        public CurrentIdentityFromNoAuth(IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(httpContextAccessor);

            if (httpContextAccessor.HttpContext == null)
            {
                var user = new SystemUser();
                this.UserId = user.Id;
                this.Username = user.Username;
            }
            else
            {
                var user = new UnauthenticatedUser();
                this.UserId = user.Id;
                this.Username = user.Username;
            }
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}

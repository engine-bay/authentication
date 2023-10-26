namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromNoAuth : ICurrentIdentity
    {
        public CurrentIdentityFromNoAuth(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            this.UserId = Guid.Empty;
            this.Username = "NoAuthUser";
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}

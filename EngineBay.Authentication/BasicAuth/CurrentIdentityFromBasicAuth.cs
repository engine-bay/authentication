namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromBasicAuth : ICurrentIdentity
    {
        public CurrentIdentityFromBasicAuth(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            // TODO: Get user from database?
            this.Username = string.Empty;
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}

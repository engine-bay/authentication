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

            // TODO: Return nothing?
        }

        public string? Username { get; }

        public Guid UserId { get; }
    }
}

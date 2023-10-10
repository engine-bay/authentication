namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public class CurrentIdentityFromJwt : ICurrentIdentity
    {
        public CurrentIdentityFromJwt(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            // TODO: use the claims principal to get the current identity
            this.Username = httpContextAccessor.HttpContext?.User?.Identity?.Name;

            var values = default(StringValues);
            httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("User-ID", out values);
            this.UserId = Guid.Parse(values.First() ?? string.Empty);
        }

        public string? Username { get; }

        public Guid UserId { get; }
    }
}

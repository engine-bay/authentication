namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromJwt : ICurrentIdentity
    {
        public CurrentIdentityFromJwt(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            this.Username = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? throw new ArgumentException("Could not find user name from JWT claim");

            var idString = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Id)?.Value ?? throw new ArgumentException("Could not find user ID from JWT claim");
            this.UserId = Guid.Parse(idString);
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}

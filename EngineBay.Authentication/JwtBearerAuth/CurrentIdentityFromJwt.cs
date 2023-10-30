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

            var context = httpContextAccessor.HttpContext;

            if (context == null)
            {
                var user = new SystemUser();
                this.UserId = user.Id;
                this.Username = user.Username;
            }
            else if (string.IsNullOrEmpty(context.Request.Headers["Authorization"].ToString()))
            {
                var user = new UnauthenticatedUser();
                this.UserId = user.Id;
                this.Username = user.Username;
            }
            else
            {
                this.Username = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Name)?.Value ?? throw new ArgumentException("Could not find user name from JWT claim");

                var idString = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Id)?.Value ?? throw new ArgumentException("Could not find user ID from JWT claim");
                this.UserId = Guid.Parse(idString);
            }
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}

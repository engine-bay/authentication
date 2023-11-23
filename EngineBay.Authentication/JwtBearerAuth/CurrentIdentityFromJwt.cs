namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromJwt : ICurrentIdentity
    {
        private readonly GetCurrentUser getCurrentUser;

        private Guid? userId;

        public CurrentIdentityFromJwt(IHttpContextAccessor httpContextAccessor, GetCurrentUser getCurrentUser)
        {
            ArgumentNullException.ThrowIfNull(httpContextAccessor);
            ArgumentNullException.ThrowIfNull(getCurrentUser);

            this.getCurrentUser = getCurrentUser;

            var context = httpContextAccessor.HttpContext;

            if (context == null)
            {
                this.Username = DefaultAuthenticationConfigurationConstants.SystemUserName;
            }
            else if (string.IsNullOrEmpty(context.Request.Headers["Authorization"].ToString()))
            {
                var user = new UnauthenticatedUser();
                this.userId = user.Id;
                this.Username = user.Username;
            }
            else
            {
                this.Username = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.Name)?.Value ?? throw new ArgumentException("Could not find user name from JWT claim");
            }
        }

        public string Username { get; }

        public Guid UserId => this.GetUserIdAsync(CancellationToken.None).Result;

        public async Task<Guid> GetUserIdAsync(CancellationToken cancellation)
        {
            if (this.userId == null)
            {
                if (this.getCurrentUser is null)
                {
                    throw new ArgumentException(nameof(this.getCurrentUser) + " is null");
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, this.Username) }, "JWT"));
                var user = await this.getCurrentUser.Handle(claimsPrincipal, cancellation);

                this.userId = user.Id;
            }

            return this.userId.Value;
        }
    }
}

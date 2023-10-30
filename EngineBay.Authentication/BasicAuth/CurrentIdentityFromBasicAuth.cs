namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromBasicAuth : ICurrentIdentity
    {
        private readonly GetCurrentUser? getCurrentUser;

        private Guid? userId;

        public CurrentIdentityFromBasicAuth(IHttpContextAccessor httpContextAccessor, GetCurrentUser getCurrentUser)
        {
            if (httpContextAccessor == null)
            {
                throw new ArgumentNullException(nameof(httpContextAccessor));
            }

            if (getCurrentUser == null)
            {
                throw new ArgumentNullException(nameof(getCurrentUser));
            }

            var context = httpContextAccessor.HttpContext;

            if (context == null)
            {
                var user = new SystemUser();
                this.userId = user.Id;
                this.Username = user.Username;
            }
            else if (string.IsNullOrEmpty(context.Request.Headers["Authorization"].ToString()))
            {
                var user = new UnauthenticatedUser();
                this.userId = user.Id;
                this.Username = user.Username;
            }
            else
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                var token = authHeader.Substring("Basic ".Length).Trim();
                var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialstring.Split(':');
                this.Username = credentials[0];

                this.getCurrentUser = getCurrentUser;
            }
        }

        public Guid UserId => this.GetUserIdAsync(CancellationToken.None).Result;

        public string Username { get; }

        public async Task<Guid> GetUserIdAsync(CancellationToken cancellation)
        {
            if (this.userId == null)
            {
                if (this.getCurrentUser is null)
                {
                    throw new ArgumentException(nameof(this.getCurrentUser) + " is null");
                }

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, this.Username) }, "Basic"));
                var user = await this.getCurrentUser.Handle(claimsPrincipal, cancellation);

                this.userId = user.Id;
            }

            return this.userId.Value;
        }
    }
}

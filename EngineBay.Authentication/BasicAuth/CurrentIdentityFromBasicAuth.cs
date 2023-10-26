namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.AspNetCore.Http;

    public class CurrentIdentityFromBasicAuth : ICurrentIdentity
    {
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

            // var context = httpContextAccessor.HttpContext;

            // if (context == null)
            // {
            var user = new SystemUser();
            this.UserId = user.Id;
            this.Username = user.Username;

            // }
            // else
            // {
            //    var authHeader = context.Request.Headers["Authorization"].ToString() ?? throw new ArgumentException("Can't find auth header");
            //    var token = authHeader.Substring("Basic ".Length).Trim();
            //    var credentialstring = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            //    var credentials = credentialstring.Split(':');

            // var username = credentials[0];
            //    this.Username = username;

            // var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "Basic"));
            //    var user = getCurrentUser.Handle(claimsPrincipal, CancellationToken.None);

            // this.UserId = user.Result.Id;
            // }
        }

        public Guid UserId { get; }

        public string Username { get; }
    }
}

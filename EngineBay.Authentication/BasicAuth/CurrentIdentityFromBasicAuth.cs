namespace EngineBay.Authentication
{
    using System.Security.Claims;
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

            var name = httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) ?? throw new ArgumentException("Cannot find claims");
            this.Username = name.Value;

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[] { name }, "Basic"));
            var user = getCurrentUser.Handle(claimsPrincipal, CancellationToken.None);

            this.UserId = user.Result.Id;
        }

        public string Username { get; }

        public Guid UserId { get; }
    }
}

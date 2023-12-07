namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;

    public class CurrentIdentity : ICurrentIdentity
    {
        private readonly ClaimsPrincipal principal;

        public CurrentIdentity(IHttpContextAccessor httpContextAccessor)
        {
            ArgumentNullException.ThrowIfNull(httpContextAccessor);
            ArgumentNullException.ThrowIfNull(httpContextAccessor.HttpContext);

            this.principal = httpContextAccessor.HttpContext.User;
        }

        public Guid UserId
        {
            get
            {
                var claim = this.principal.FindFirst(x => x.Type == CustomClaimTypes.UserId);
                if (claim is not null)
                {
                    return Guid.Parse(claim.Value);
                }

                return Guid.Empty;
            }
        }

        public string Username
        {
            get
            {
                var username = string.Empty;

                if (this.principal?.Identity?.Name is not null)
                {
                    return this.principal.Identity.Name;
                }

                var claim = this.principal?.FindFirst(x => x.Type == CustomClaimTypes.Name);
                if (claim is not null)
                {
                    return claim.Value;
                }

                return string.Empty;
            }
        }

        public bool HasPermission(string permission)
        {
            return this.principal.Claims.Any(claim => claim.Type == CustomClaimTypes.Scope && claim.Value == permission);
        }
    }
}

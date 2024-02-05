namespace EngineBay.Authentication
{
    using System;
    using System.Globalization;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using EngineBay.Telemetry;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;

    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly ILogger<ClaimsTransformer> logger;
        private readonly AuthenticationQueryDbContext authenticationQueryDbContext;
        private readonly GetPermissionsByApplicationUserId getPermissionsByApplicationUserId;

        public ClaimsTransformer(
            ILogger<ClaimsTransformer> logger,
            AuthenticationQueryDbContext authenticationQueryDbContext,
            GetPermissionsByApplicationUserId getPermissionsByApplicationUserId)
        {
            this.logger = logger;
            this.authenticationQueryDbContext = authenticationQueryDbContext;
            this.getPermissionsByApplicationUserId = getPermissionsByApplicationUserId;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            using var activity = EngineBayActivitySource.Source.StartActivity("Transformer:" + "Authentication.Claims");

            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(principal.Identity);

            // This will run every time Authenticate is called so its better to create a new Principal
            var transformed = new ClaimsPrincipal();
            transformed.AddIdentities(principal.Identities);

            var nameClaim = principal.FindFirst(x => x.Type == CustomClaimTypes.Name);

            var username = string.Empty;

            if (principal.Identity.Name is not null)
            {
                username = principal.Identity.Name;
            }
            else if (nameClaim is not null)
            {
                username = nameClaim.Value;
            }
            else
            {
                // TODO: Discuss whether we want to return immediately, at least copy across remaining values, return the original, or throw an error
                return transformed;
            }

            var applicationUser =
                await this.authenticationQueryDbContext.ApplicationUsers.SingleOrDefaultAsync(
                    x => x.Username == username);

            if (applicationUser is null)
            {
                this.logger.UserDoesNotExist(username);
                return transformed;
            }

            var claims = new List<Claim>
            {
                new (CustomClaimTypes.Transformed, DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                new (CustomClaimTypes.UserId, applicationUser.Id.ToString()),
            };

            var permissions = await this.getPermissionsByApplicationUserId.Handle(applicationUser.Id, CancellationToken.None);
            claims.AddRange(permissions.Select(permission => new Claim(CustomClaimTypes.Scope, permission.Name)));

            transformed.AddIdentity(new ClaimsIdentity(claims));
            return transformed;
        }
    }
}
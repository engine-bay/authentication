namespace EngineBay.Authentication
{
    using System;
    using System.Globalization;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;

    public class ClaimsTransformer : IClaimsTransformation
    {
        private readonly ILogger<ClaimsTransformer> logger;
        private readonly AuthenticationQueryDbContext authenticationQueryDbContext;

        public ClaimsTransformer(ILogger<ClaimsTransformer> logger, AuthenticationQueryDbContext authenticationQueryDbContext)
        {
            this.logger = logger;
            this.authenticationQueryDbContext = authenticationQueryDbContext;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            ArgumentNullException.ThrowIfNull(principal);
            ArgumentNullException.ThrowIfNull(principal.Identity);

            // This will run every time Authenticate is called so its better to create a new Principal
            var transformed = new ClaimsPrincipal();
            transformed.AddIdentities(principal.Identities);

            var claim = principal.FindFirst(x => x.Type == CustomClaimTypes.Name);

            var username = string.Empty;

            if (principal.Identity.Name is not null)
            {
                username = principal.Identity.Name;
            }
            else if (claim is not null)
            {
                username = claim.Value;
            }
            else
            {
                return transformed;
            }

            var applicationUser = await this.authenticationQueryDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Username == username);

            if (applicationUser is null)
            {
                this.logger.UserDoesNotExistPIISafe(username);
                return transformed;
            }

#pragma warning disable CS8603,CS8604 // SQL will handle null references

            var scopeClaims = this.authenticationQueryDbContext.Permissions
                .Where(permission => permission.Groups
                                                .SelectMany(group => group.Roles)
                                                .SelectMany(role => role.Users)
                                                .Select(user => user.ApplicationUserId)
                                                .Contains(applicationUser.Id))
                .Select(permission => new Claim(CustomClaimTypes.Scope, permission.Name))
                .ToList();
#pragma warning restore CS8603, CS8604

            var claims = new List<Claim>
            {
                new Claim(CustomClaimTypes.Transformed, DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                new Claim(CustomClaimTypes.UserId, applicationUser.Id.ToString()),
            };
            claims.AddRange(scopeClaims);
            transformed.AddIdentity(new ClaimsIdentity(claims));
            return transformed;
        }
    }
}
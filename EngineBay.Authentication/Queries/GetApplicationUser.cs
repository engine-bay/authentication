namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class GetApplicationUser : IQueryHandler<ClaimsPrincipal, ApplicationUser>
    {
        private readonly ILogger<GetApplicationUser> logger;
        private readonly AuthenticationQueryDbContext authenticationQueryDbContext;

        public GetApplicationUser(ILogger<GetApplicationUser> logger, AuthenticationQueryDbContext authenticationQueryDbContext)
        {
            this.logger = logger;
            this.authenticationQueryDbContext = authenticationQueryDbContext;
        }

        /// <inheritdoc/>
        public async Task<ApplicationUser> Handle(ClaimsPrincipal user, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(user.Identity);

            var claim = user.FindFirst(x => x.Type == "name");

            var username = string.Empty;

            if (user.Identity.Name is not null)
            {
                username = user.Identity.Name;
            }
            else if (claim is not null)
            {
                username = claim.Value;
            }
            else
            {
                throw new ArgumentException(nameof(user.Identity.Name));
            }

            var applicationUser = await this.authenticationQueryDbContext.ApplicationUsers.SingleOrDefaultAsync(x => x.Username == username, cancellation);

            if (applicationUser is null)
            {
                this.logger.UserDoesNotExist(username);
                throw new ArgumentException(nameof(applicationUser));
            }

            return applicationUser;
        }
    }
}
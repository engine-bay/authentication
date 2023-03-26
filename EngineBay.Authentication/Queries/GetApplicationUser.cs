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
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Identity is null)
            {
                throw new ArgumentException(nameof(user.Identity));
            }

            var applicationUser = await this.authenticationQueryDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Username == user.Identity.Name, cancellation).ConfigureAwait(false);

            if (applicationUser is null)
            {
                this.logger.UserDoesNotExist();
                throw new ArgumentException(nameof(applicationUser));
            }

            return applicationUser;
        }
    }
}
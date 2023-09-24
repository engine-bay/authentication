namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Logging;
    using Microsoft.EntityFrameworkCore;

    public class GetCurrentUser : IQueryHandler<ClaimsPrincipal, ApplicationUserDto>
    {
        private readonly ILogger<GetCurrentUser> logger;
        private readonly AuthenticationQueryDbContext authenticationQueryDbContext;

        public GetCurrentUser(ILogger<GetCurrentUser> logger, AuthenticationQueryDbContext authenticationQueryDbContext)
        {
            this.logger = logger;
            this.authenticationQueryDbContext = authenticationQueryDbContext;
        }

        /// <inheritdoc/>
        public async Task<ApplicationUserDto> Handle(ClaimsPrincipal user, CancellationToken cancellation)
        {
            var shouldLogSensitiveData = LoggingConfiguration.IsSensitiveDataLoggingEnabled();
            if (user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (user.Identity is null)
            {
                throw new ArgumentException(nameof(user.Identity));
            }

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

            var applicationUser = await this.authenticationQueryDbContext.ApplicationUsers.FirstOrDefaultAsync(x => x.Username == username, cancellation).ConfigureAwait(false);

            if (applicationUser is null)
            {
                if (shouldLogSensitiveData)
                {
                    this.logger.UserDoesNotExist(username);
                }
                else
                {
                    this.logger.UserDoesNotExist();
                }

                throw new ArgumentException(nameof(applicationUser));
            }

            return new ApplicationUserDto(applicationUser);
        }
    }
}
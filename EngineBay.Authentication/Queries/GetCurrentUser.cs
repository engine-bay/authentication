namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.DemoModule;
    using EngineBay.Persistence;
    using EngineBay.Telemetry;
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
            using var activity = EngineBayActivitySource.Source.StartActivity(TracingActivityNameConstants.Handler + AuthActivityNameConstants.CurrentUserGet);

            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(user.Identity);

            var claim = user.FindFirst(x => x.Type == CustomClaimTypes.Name);

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

            return new ApplicationUserDto(applicationUser);
        }
    }
}
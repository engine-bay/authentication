namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class CreateUser : ICommandHandler<CreateUserDto, ApplicationUserDto>
    {
        private readonly ILogger<CreateBasicAuthUser> logger;
        private readonly AuthenticationWriteDbContext authenticationWriteDbContext;

        private readonly GetApplicationUser getApplicationUserQuery;

        public CreateUser(ILogger<CreateBasicAuthUser> logger, GetApplicationUser getApplicationUserQuery, AuthenticationWriteDbContext authenticationWriteDbContext)
        {
            this.logger = logger;
            this.getApplicationUserQuery = getApplicationUserQuery;
            this.authenticationWriteDbContext = authenticationWriteDbContext;
        }

        public async Task<ApplicationUserDto> Handle(CreateUserDto createUserDto, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation)
        {
            if (createUserDto is null)
            {
                throw new ArgumentNullException(nameof(createUserDto));
            }

            var newApplicationUser = new ApplicationUser()
            {
                Username = createUserDto.Username,
            };

            this.authenticationWriteDbContext.ApplicationUsers.Add(newApplicationUser);

            var systemUser = await this.authenticationWriteDbContext.ApplicationUsers.SingleOrDefaultAsync(applicationUser => applicationUser.Username == DefaultAuthenticationConfigurationConstants.SystemUserName, cancellation).ConfigureAwait(false);

            if (systemUser is null)
            {
                throw new ArgumentException(nameof(systemUser));
            }

            await this.authenticationWriteDbContext.SaveChangesAsync(systemUser, cancellation).ConfigureAwait(false);

            this.logger.RegisteredNewUser();

            return new ApplicationUserDto(newApplicationUser);
        }
    }
}
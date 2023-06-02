namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using BCrypt.Net;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class CreateBasicAuthUser : ICommandHandler<CreateBasicAuthUserDto, ApplicationUserDto>
    {
        private readonly ILogger<CreateBasicAuthUser> logger;
        private readonly AuthenticationWriteDbContext authenticationWriteDbContext;

        private readonly GetApplicationUser getApplicationUserQuery;

        public CreateBasicAuthUser(ILogger<CreateBasicAuthUser> logger, GetApplicationUser getApplicationUserQuery, AuthenticationWriteDbContext authenticationWriteDbContext)
        {
            this.logger = logger;
            this.getApplicationUserQuery = getApplicationUserQuery;
            this.authenticationWriteDbContext = authenticationWriteDbContext;
        }

        public async Task<ApplicationUserDto> Handle(CreateBasicAuthUserDto createBasicAuthUserDto, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation)
        {
            if (createBasicAuthUserDto is null)
            {
                throw new ArgumentNullException(nameof(createBasicAuthUserDto));
            }

            var systemUser = await this.authenticationWriteDbContext.ApplicationUsers.SingleOrDefaultAsync(applicationUser => applicationUser.Username == DefaultAuthenticationConfigurationConstants.SystemUserName, cancellation).ConfigureAwait(false);

            var newApplicationUser = new ApplicationUser()
            {
                Username = createBasicAuthUserDto.Username,
            };

            this.authenticationWriteDbContext.ApplicationUsers.Add(newApplicationUser);

            if (systemUser is null)
            {
                throw new ArgumentException(nameof(systemUser));
            }

            await this.authenticationWriteDbContext.SaveChangesAsync(systemUser, cancellation).ConfigureAwait(false);

            var hashedPassword = BCrypt.EnhancedHashPassword(createBasicAuthUserDto.Password);

            var basicAuthCredentials = new BasicAuthCredential()
            {
                ApplicationUser = newApplicationUser,
                PasswordHash = hashedPassword,
            };

            this.authenticationWriteDbContext.BasicAuthCredentials.Add(basicAuthCredentials);

            await this.authenticationWriteDbContext.SaveChangesAsync(systemUser, cancellation).ConfigureAwait(false);

            this.logger.RegisteredNewUser();

            return new ApplicationUserDto(newApplicationUser);
        }
    }
}
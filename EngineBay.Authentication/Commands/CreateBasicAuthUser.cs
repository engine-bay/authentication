namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using BCrypt.Net;
    using EngineBay.Core;
    using EngineBay.Persistence;

    public class CreateBasicAuthUser : ICommandHandler<CreateBasicAuthUserDto, ApplicationUserDto>
    {
        private readonly AuthenticationWriteDbContext authenticationWriteDbContext;
        private readonly GetCurrentUser getCurrentUserQuery;

        private readonly GetApplicationUser getApplicationUserQuery;

        public CreateBasicAuthUser(GetApplicationUser getApplicationUserQuery, GetCurrentUser getCurrentUserQuery, AuthenticationWriteDbContext authenticationWriteDbContext)
        {
            this.getCurrentUserQuery = getCurrentUserQuery;
            this.getApplicationUserQuery = getApplicationUserQuery;
            this.authenticationWriteDbContext = authenticationWriteDbContext;
        }

        public async Task<ApplicationUserDto> Handle(CreateBasicAuthUserDto createBasicAuthUserDto, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation)
        {
            if (createBasicAuthUserDto is null)
            {
                throw new ArgumentNullException(nameof(createBasicAuthUserDto));
            }

            var newApplicationUser = new ApplicationUser()
            {
                Name = createBasicAuthUserDto.Name,
            };

            this.authenticationWriteDbContext.ApplicationUsers.Add(newApplicationUser);

            var hashedPassword = BCrypt.EnhancedHashPassword(createBasicAuthUserDto.Password);

            var basicAuthCredentials = new BasicAuthCredential()
            {
                ApplicationUser = newApplicationUser,
                PasswordHash = hashedPassword,
            };

            this.authenticationWriteDbContext.BasicAuthCredentials.Add(basicAuthCredentials);

            var systemUser = new SystemUser();

            await this.authenticationWriteDbContext.SaveChangesAsync(systemUser, cancellation).ConfigureAwait(false);

            return await this.getCurrentUserQuery.Handle(claimsPrincipal, cancellation).ConfigureAwait(false);
        }
    }
}
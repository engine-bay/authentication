namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using BCrypt.Net;
    using EngineBay.Core;
    using EngineBay.Persistence;

    public class CreateBasicAuthUser : ICommandHandler<CreateBasicAuthUserDto, ApplicationUserDto>
    {
        private readonly ILogger<CreateBasicAuthUser> logger;
        private readonly AuthenticationWriteDbContext authenticationWriteDbContext;

        public CreateBasicAuthUser(
            ILogger<CreateBasicAuthUser> logger,
            AuthenticationWriteDbContext authenticationWriteDbContext)
        {
            this.logger = logger;
            this.authenticationWriteDbContext = authenticationWriteDbContext;
        }

        public async Task<ApplicationUserDto> Handle(
            CreateBasicAuthUserDto createBasicAuthUserDto,
            ClaimsPrincipal user,
            CancellationToken cancellation)
        {
            if (createBasicAuthUserDto is null)
            {
                throw new ArgumentNullException(nameof(createBasicAuthUserDto));
            }

            var newApplicationUser = new ApplicationUser()
            {
                Username = createBasicAuthUserDto.Username,
            };

            this.authenticationWriteDbContext.ApplicationUsers.Add(newApplicationUser);

            await this.authenticationWriteDbContext.SaveChangesAsync(cancellation).ConfigureAwait(false);

            var hashedPassword = BCrypt.EnhancedHashPassword(createBasicAuthUserDto.Password);

            var basicAuthCredentials = new BasicAuthCredential()
            {
                ApplicationUser = newApplicationUser,
                PasswordHash = hashedPassword,
            };

            this.authenticationWriteDbContext.BasicAuthCredentials.Add(basicAuthCredentials);

            await this.authenticationWriteDbContext.SaveChangesAsync(cancellation).ConfigureAwait(false);

            this.logger.RegisteredNewUser();

            return new ApplicationUserDto(newApplicationUser);
        }
    }
}
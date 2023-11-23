namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class CreateUser : ICommandHandler<CreateUserDto, ApplicationUserDto>
    {
        private readonly ILogger<CreateBasicAuthUser> logger;
        private readonly AuthenticationWriteDbContext authenticationWriteDbContext;

        public CreateUser(ILogger<CreateBasicAuthUser> logger, AuthenticationWriteDbContext authenticationWriteDbContext)
        {
            this.logger = logger;
            this.authenticationWriteDbContext = authenticationWriteDbContext;
        }

        public async Task<ApplicationUserDto> Handle(CreateUserDto createUserDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(createUserDto);

            var newApplicationUser = new ApplicationUser(createUserDto.Username);

            this.authenticationWriteDbContext.ApplicationUsers.Add(newApplicationUser);

            var systemUser = await this.authenticationWriteDbContext.ApplicationUsers.SingleOrDefaultAsync(applicationUser => applicationUser.Username == DefaultAuthenticationConfigurationConstants.SystemUserName, cancellation);

            if (systemUser is null)
            {
                throw new ArgumentException(nameof(systemUser));
            }

            await this.authenticationWriteDbContext.SaveChangesAsync(cancellation);

            this.logger.RegisteredNewUser();

            return new ApplicationUserDto(newApplicationUser);
        }
    }
}
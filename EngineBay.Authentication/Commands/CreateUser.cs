namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Persistence;

    public class CreateUser : ICommandHandler<CreateUserDto, ApplicationUserDto>
    {
        private readonly AuthenticationWriteDbContext authenticationWriteDbContext;
        private readonly GetCurrentUser getCurrentUserQuery;

        private readonly GetApplicationUser getApplicationUserQuery;

        public CreateUser(GetApplicationUser getApplicationUserQuery, GetCurrentUser getCurrentUserQuery, AuthenticationWriteDbContext authenticationWriteDbContext)
        {
            this.getCurrentUserQuery = getCurrentUserQuery;
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
                Name = createUserDto.Name,
            };

            this.authenticationWriteDbContext.ApplicationUsers.Add(newApplicationUser);

            var systemUser = new SystemUser();

            await this.authenticationWriteDbContext.SaveChangesAsync(systemUser, cancellation).ConfigureAwait(false);

            return await this.getCurrentUserQuery.Handle(claimsPrincipal, cancellation).ConfigureAwait(false);
        }
    }
}
namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetUserWithPermissions : IQueryHandler<Guid, AuthUserDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetUserWithPermissions(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<AuthUserDto> Handle(Guid applicationUserId, CancellationToken cancellation)
        {
            var authUser = await this.authDb.AuthUsers.Include(authUser => authUser.Roles)
                               .ThenInclude(role => role.Groups)
                               .ThenInclude(group => group.Permissions)
                               .SingleOrDefaultAsync(
                                   authUser => authUser.ApplicationUserId == applicationUserId,
                                   cancellation) ??
                throw new NotFoundException($"No AuthUser with ApplicationUserId ${applicationUserId} found.");

            return new AuthUserDto(authUser);
        }
    }
}
namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetUserRoles : IQueryHandler<Guid, UserRoleDto>
    {
        private readonly AuthorizationQueryDbContext authDb;

        public GetUserRoles(AuthorizationQueryDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<UserRoleDto> Handle(Guid query, CancellationToken cancellation)
        {
            var user = await this.authDb.UserRoles.Include(authUser => authUser.Roles)
                           .SingleOrDefaultAsync(authUser => authUser.Id == query, cancellation) ??
                       throw new NotFoundException($"No AuthUser with Id ${query} found.");

            return new UserRoleDto(user);
        }
    }
}

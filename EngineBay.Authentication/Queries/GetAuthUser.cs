namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetAuthUser : IQueryHandler<Guid, AuthUserDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetAuthUser(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<AuthUserDto> Handle(Guid query, CancellationToken cancellation)
        {
            var user = await this.authDb.AuthUsers.Include(authUser => authUser.Roles)
                           .FirstOrDefaultAsync(authUser => authUser.Id == query, cancellation) ??
                       throw new NotFoundException($"No AuthUser with Id ${query} found.");

            return new AuthUserDto(user);
        }
    }
}

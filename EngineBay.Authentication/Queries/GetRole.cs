namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetRole : IQueryHandler<Guid, RoleDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetRole(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<RoleDto> Handle(Guid query, CancellationToken cancellation)
        {
            var role = await this.authDb.Roles
                .Include(role => role.Groups)
                .FirstOrDefaultAsync(role => role.Id == query, cancellation) ??
                throw new NotFoundException($"No Role with Id ${query} found.");

            return new RoleDto(role);
        }
    }
}

namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetPermissionByName : IQueryHandler<string, PermissionDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetPermissionByName(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<PermissionDto> Handle(string query, CancellationToken cancellation)
        {
            var user = await this.authDb.Permissions.SingleOrDefaultAsync(p => p.Name == query, cancellation) ??
                       throw new NotFoundException($"No Permission with Name ${query} found.");

            return new PermissionDto(user);
        }
    }
}

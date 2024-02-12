namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetPermissionsByApplicationUserId : IQueryHandler<Guid, IEnumerable<PermissionDto>>
    {
        private readonly AuthorizationQueryDbContext authDb;

        public GetPermissionsByApplicationUserId(AuthorizationQueryDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(Guid applicationUserId, CancellationToken cancellation)
        {
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8603 // Possible null reference return.
            var permissions = await this.authDb.Permissions
                .Where(permission => permission.Groups
                    .SelectMany(group => group.Roles)
                    .SelectMany(role => role.Users)
                    .Select(user => user.ApplicationUserId)
                    .Contains(applicationUserId))
                .ToListAsync(cancellation);
#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8604 // Possible null reference argument.

            return permissions.Select(permission => new PermissionDto(permission));
        }
    }
}
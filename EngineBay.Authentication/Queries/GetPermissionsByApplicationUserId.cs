namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetPermissionsByApplicationUserId : IQueryHandler<Guid, IEnumerable<PermissionDto>>
    {
        private readonly AuthenticationDbContext authDb;

        public GetPermissionsByApplicationUserId(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(Guid applicationUserId, CancellationToken cancellation)
        {
            var permissions = await this.authDb.Permissions
                .Where(permission => permission.Groups != null && permission.Groups.Any(
                    group => group.Roles != null && group.Roles.Any(
                        role => role.Users != null && role.Users.Any(
                            user => user.ApplicationUserId == applicationUserId))))
                .ToListAsync(cancellation);

            return permissions.Select(permission => new PermissionDto(permission));
        }
    }
}
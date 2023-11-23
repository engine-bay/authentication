namespace EngineBay.Authentication
{
    using EngineBay.Core;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class GetPermission : IQueryHandler<Guid, PermissionDto>
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        private readonly AuthenticationDbContext authDb;

        public GetPermission(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<PermissionDto> Handle(Guid query, CancellationToken cancellation)
        {
            var user = await this.authDb.Permissions.FindAsync(new object[] { query }, cancellation) ?? throw new NotFoundException($"No Permission with Id ${query} found.");

            return new PermissionDto(user);
        }
    }
}

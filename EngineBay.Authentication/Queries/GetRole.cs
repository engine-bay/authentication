namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public class GetRole : IQueryHandler<Guid, RoleDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetRole(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<RoleDto> Handle(Guid query, CancellationToken cancellation)
        {
            var role = await this.authDb.Roles.FindAsync(new object[] { query }, cancellation) ?? throw new NotFoundException($"No Role with Id ${query} found.");

            return new RoleDto(role);
        }
    }
}

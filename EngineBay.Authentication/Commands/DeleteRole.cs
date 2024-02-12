namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public class DeleteRole : ICommandHandler<Guid, RoleDto>
    {
        private readonly AuthorizationWriteDbContext authDb;

        public DeleteRole(AuthorizationWriteDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<RoleDto> Handle(Guid id, CancellationToken cancellation)
        {
            var role = await this.authDb.Roles.FindAsync(new object[] { id }, cancellation) ?? throw new NotFoundException($"No Role with Id ${id} found.");

            this.authDb.Roles.Remove(role);
            await this.authDb.SaveChangesAsync(cancellation);

            return new RoleDto(role);
        }
    }
}
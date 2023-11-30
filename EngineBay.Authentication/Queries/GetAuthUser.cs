namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public class GetAuthUser : IQueryHandler<Guid, AuthUserDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetAuthUser(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<AuthUserDto> Handle(Guid query, CancellationToken cancellation)
        {
            var user = await this.authDb.AuthUsers.FindAsync(new object[] { query }, cancellation) ?? throw new NotFoundException($"No AuthUser with Id ${query} found.");

            return new AuthUserDto(user);
        }
    }
}

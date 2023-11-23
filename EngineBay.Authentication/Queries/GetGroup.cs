namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public class GetGroup : IQueryHandler<Guid, GroupDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetGroup(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<GroupDto> Handle(Guid query, CancellationToken cancellation)
        {
            var user = await this.authDb.Groups.FindAsync(new object[] { query }, cancellation) ?? throw new NotFoundException($"No Group with Id ${query} found.");

            return new GroupDto(user);
        }
    }
}

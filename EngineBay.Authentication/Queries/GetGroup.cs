namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetGroup : IQueryHandler<Guid, GroupDto>
    {
        private readonly AuthorizationQueryDbContext authDb;

        public GetGroup(AuthorizationQueryDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<GroupDto> Handle(Guid query, CancellationToken cancellation)
        {
            var group = await this.authDb.Groups
                            .Include(group => group.Permissions)
                            .SingleOrDefaultAsync(group => group.Id == query, cancellation) ??
                        throw new NotFoundException($"No Group with Id ${query} found.");

            return new GroupDto(group);
        }
    }
}

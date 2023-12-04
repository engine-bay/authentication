namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class GetGroup : IQueryHandler<Guid, GroupDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetGroup(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<GroupDto> Handle(Guid query, CancellationToken cancellation)
        {
            var user = await this.authDb.Groups
                           .Include(group => group.Roles)
                           .FirstOrDefaultAsync(group => group.Id == query, cancellation) ??
                       throw new NotFoundException($"No Group with Id ${query} found.");

            return new GroupDto(user);
        }
    }
}

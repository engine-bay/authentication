namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.DemoModule;
    using EngineBay.Telemetry;
    using Microsoft.EntityFrameworkCore;

    public class GetUserRoles : IQueryHandler<Guid, UserRoleDto>
    {
        private readonly AuthenticationDbContext authDb;

        public GetUserRoles(AuthenticationDbContext authDb)
        {
            this.authDb = authDb;
        }

        public async Task<UserRoleDto> Handle(Guid query, CancellationToken cancellation)
        {
            using var activity = EngineBayActivitySource.Source.StartActivity(TracingActivityNameConstants.Handler + AuthActivityNameConstants.UserRoleGet);

            var user = await this.authDb.UserRoles.Include(authUser => authUser.Roles)
                           .SingleOrDefaultAsync(authUser => authUser.Id == query, cancellation) ??
                       throw new NotFoundException($"No AuthUser with Id ${query} found.");

            return new UserRoleDto(user);
        }
    }
}

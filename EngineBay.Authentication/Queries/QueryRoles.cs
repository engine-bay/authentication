namespace EngineBay.Authentication
{
    using System.Globalization;
    using System.Linq.Expressions;
    using EngineBay.Core;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;

    public class QueryRoles : PaginatedQuery<Role>, IQueryHandler<PaginationParameters, PaginatedDto<RoleDto>>
    {
        private readonly AuthenticationDbContext dbContext;

        public QueryRoles(AuthenticationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PaginatedDto<RoleDto>> Handle(PaginationParameters query, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(query);

            var expression = this.dbContext.Roles.AsExpandable();
            var format = new DateTimeFormatInfo();
            var limit = query.Limit;
            var skip = limit > 0 ? query.Skip : 0;
            var total = await expression.CountAsync(cancellation);

            Expression<Func<Role, string?>> sortByPredicate = query.SortBy switch
            {
                nameof(Role.Id) => role => role.Id.ToString(),
                nameof(Role.Name) => role => role.Name,
                nameof(Role.Description) => role => role.Description,
                nameof(Role.CreatedAt) => role => role.CreatedAt.ToString(format),
                nameof(Role.LastUpdatedAt) => role => role.LastUpdatedAt.ToString(format),
                _ => throw new ArgumentException($"Role SortBy type {query.SortBy} not found"),
            };

            expression = this.Sort(expression, sortByPredicate, query);
            expression = this.Paginate(expression, query);

            var roles = limit > 0 ? await expression.ToListAsync(cancellation) : new List<Role>();

            var roleDtos = roles.Select(role => new RoleDto(role));
            return new PaginatedDto<RoleDto>(total, skip, limit, roleDtos);
        }
    }
}

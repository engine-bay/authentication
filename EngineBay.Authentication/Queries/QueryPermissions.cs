namespace EngineBay.Authentication
{
    using System.Globalization;
    using System.Linq.Expressions;
    using EngineBay.Core;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;

    public class QueryPermissions : PaginatedQuery<Permission>, IQueryHandler<PaginationParameters, PaginatedDto<PermissionDto>>
    {
        private readonly AuthorizationQueryDbContext dbContext;

        public QueryPermissions(AuthorizationQueryDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PaginatedDto<PermissionDto>> Handle(PaginationParameters query, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(query);

            var expression = this.dbContext.Permissions.AsExpandable();
            var format = new DateTimeFormatInfo();
            var limit = query.Limit;
            var skip = limit > 0 ? query.Skip : 0;
            var total = await expression.CountAsync(cancellation);

            Expression<Func<Permission, string?>> sortByPredicate = query.SortBy switch
            {
                nameof(Permission.Id) => permission => permission.Id.ToString(),
                nameof(Permission.Name) => permission => permission.Name,
                nameof(Permission.CreatedAt) => permission => permission.CreatedAt.ToString(format),
                nameof(Permission.LastUpdatedAt) => permission => permission.LastUpdatedAt.ToString(format),
                _ => throw new ArgumentException($"Permission SortBy type {query.SortBy} not found"),
            };

            expression = this.Sort(expression, sortByPredicate, query);
            expression = this.Paginate(expression, query);

            var permissions = limit > 0 ? await expression.ToListAsync(cancellation) : new List<Permission>();

            var permissionDtos = permissions.Select(permission => new PermissionDto(permission));
            return new PaginatedDto<PermissionDto>(total, skip, limit, permissionDtos);
        }
    }
}

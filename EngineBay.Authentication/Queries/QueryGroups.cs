namespace EngineBay.Authentication
{
    using System.Globalization;
    using System.Linq.Expressions;
    using EngineBay.Core;
    using LinqKit;
    using Microsoft.EntityFrameworkCore;

    public class QueryGroups : PaginatedQuery<Group>, IQueryHandler<PaginationParameters, PaginatedDto<GroupDto>>
    {
        private readonly AuthenticationDbContext dbContext;

        public QueryGroups(AuthenticationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PaginatedDto<GroupDto>> Handle(PaginationParameters query, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(query);

            var expression = this.dbContext.Groups.AsExpandable();
            var format = new DateTimeFormatInfo();
            var limit = query.Limit;
            var skip = limit > 0 ? query.Skip : 0;
            var total = await expression.CountAsync(cancellation);

            Expression<Func<Group, string?>> sortByPredicate = query.SortBy switch
            {
                nameof(Group.Id) => group => group.Id.ToString(),
                nameof(Group.Name) => group => group.Name,
                nameof(Group.Description) => group => group.Description,
                nameof(Group.CreatedAt) => group => group.CreatedAt.ToString(format),
                nameof(Group.LastUpdatedAt) => group => group.LastUpdatedAt.ToString(format),
                _ => throw new ArgumentException($"Group SortBy type {query.SortBy} not found"),
            };

            expression = this.Sort(expression, sortByPredicate, query);
            expression = this.Paginate(expression, query);

            var groups = limit > 0 ? await expression.ToListAsync(cancellation) : new List<Group>();

            var groupDtos = groups.Select(group => new GroupDto(group));
            return new PaginatedDto<GroupDto>(total, skip, limit, groupDtos);
        }
    }
}

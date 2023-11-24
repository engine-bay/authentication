namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public static class AuthorizationEndpoints
    {
        private const string RoleBasePath = "/roles";
        private const string GroupBasePath = "/groups";
        private const string PermissionBasePath = "/permissions ";

        public static void MapEndpoints(RouteGroupBuilder endpoints)
        {
            endpoints.MapGet(
                RoleBasePath,
                async (QueryRoles handler, int? skip, int? limit, string? sortBy, SortOrderType? sortOrder, CancellationToken cancellation) =>
                {
                    var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                    var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                    return Results.Ok(paginatedDtos);
                })
                .WithTags(ApiGroupNameConstants.Role);

            endpoints.MapGet(
                RoleBasePath + "/{id:guid}",
                (GetRole handler, Guid id, CancellationToken cancellation) =>
                {
                    var result = handler.Handle(id, cancellation);
                    return Results.Ok(result);
                })
                .WithTags(ApiGroupNameConstants.Role);

            endpoints.MapPost(
                RoleBasePath,
                () =>
                {
                    return Results.NotFound();
                })
                .WithTags(ApiGroupNameConstants.Role);

            endpoints.MapPut(
                RoleBasePath + "/{id:guid}",
                () =>
                {
                    return Results.NotFound();
                })
                .WithTags(ApiGroupNameConstants.Role);

            endpoints.MapDelete(
                RoleBasePath + "/{id:guid}",
                () =>
                {
                    return Results.NotFound();
                })
                .WithTags(ApiGroupNameConstants.Role);

            endpoints.MapGet(
                GroupBasePath,
                async (QueryGroups handler, int? skip, int? limit, string? sortBy, SortOrderType? sortOrder, CancellationToken cancellation) =>
                {
                    var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                    var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                    return Results.Ok(paginatedDtos);
                })
                .WithTags(ApiGroupNameConstants.Group);

            endpoints.MapGet(
                GroupBasePath + "/{id:guid}",
                (GetGroup handler, Guid id, CancellationToken cancellation) =>
                {
                    var result = handler.Handle(id, cancellation);
                    return Results.Ok(result);
                })
                .WithTags(ApiGroupNameConstants.Group);

            endpoints.MapGet(
                PermissionBasePath,
                async (QueryPermissions handler, int? skip, int? limit, string? sortBy, SortOrderType? sortOrder, CancellationToken cancellation) =>
                {
                    var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                    var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                    return Results.Ok(paginatedDtos);
                })
                .WithTags(ApiGroupNameConstants.Permission);

            endpoints.MapGet(
                PermissionBasePath + "/{id:guid}",
                (GetPermission handler, Guid id, CancellationToken cancellation) =>
                {
                    var result = handler.Handle(id, cancellation);
                    return Results.Ok(result);
                })
                .WithTags(ApiGroupNameConstants.Permission);
        }
    }
}

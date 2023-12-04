namespace EngineBay.Authentication
{
    using EngineBay.Core;

    public static class AuthorizationEndpoints
    {
        private const string RoleBasePath = "/roles";
        private const string GroupBasePath = "/groups";
        private const string PermissionBasePath = "/permissions";

        public static void MapEndpoints(RouteGroupBuilder endpoints)
        {
            endpoints.MapGet(
                    RoleBasePath,
                    async (
                        QueryRoles handler,
                        int? skip,
                        int? limit,
                        string? sortBy,
                        SortOrderType? sortOrder,
                        CancellationToken cancellation) =>
                    {
                        var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                        var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                        return Results.Ok(paginatedDtos);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(PermissionConstants.QueryRoles);

            endpoints.MapGet(
                    RoleBasePath + "/{id:guid}",
                    async (GetRole handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(PermissionConstants.GetRoles);

            endpoints.MapPost(
                    RoleBasePath,
                    async (CreateRole handler, CreateOrUpdateRoleDto createRoleDto, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(createRoleDto, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(PermissionConstants.CreateRoles);

            endpoints.MapPut(
                    RoleBasePath + "/{id:guid}",
                    async (
                        UpdateRole handler,
                        Guid id,
                        CreateOrUpdateRoleDto createRoleDto,
                        CancellationToken cancellation) =>
                    {
                        var command = new UpdateRoleCommand(id, createRoleDto.Name)
                        {
                            Description = createRoleDto.Description,
                            GroupIds = createRoleDto.GroupIds,
                        };
                        var result = await handler.Handle(command, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(PermissionConstants.UpdateRoles);

            endpoints.MapDelete(
                    RoleBasePath + "/{id:guid}",
                    async (DeleteRole handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(PermissionConstants.DeleteRoles);

            endpoints.MapGet(
                    GroupBasePath,
                    async (
                        QueryGroups handler,
                        int? skip,
                        int? limit,
                        string? sortBy,
                        SortOrderType? sortOrder,
                        CancellationToken cancellation) =>
                    {
                        var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                        var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                        return Results.Ok(paginatedDtos);
                    })
                .WithTags(ApiGroupNameConstants.Group)
                .RequireAuthorization(PermissionConstants.QueryGroups);

            endpoints.MapGet(
                    GroupBasePath + "/{id:guid}",
                    async (GetGroup handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Group)
                .RequireAuthorization(PermissionConstants.GetGroups);

            endpoints.MapGet(
                    PermissionBasePath,
                    async (
                        QueryPermissions handler,
                        int? skip,
                        int? limit,
                        string? sortBy,
                        SortOrderType? sortOrder,
                        CancellationToken cancellation) =>
                    {
                        var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                        var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                        return Results.Ok(paginatedDtos);
                    })
                .WithTags(ApiGroupNameConstants.Permission)
                .RequireAuthorization(PermissionConstants.QueryPermissions);

            endpoints.MapGet(
                    PermissionBasePath + "/{id:guid}",
                    async (GetPermission handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Permission)
                .RequireAuthorization(PermissionConstants.GetPermissions);
        }
    }
}

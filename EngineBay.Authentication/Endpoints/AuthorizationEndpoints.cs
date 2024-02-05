namespace EngineBay.Authentication
{
    using EngineBay.Core;
    using EngineBay.DemoModule;
    using EngineBay.Telemetry;

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
                        using var activity = EngineBayActivitySource.Source.StartActivity(TracingActivityNameConstants.Endpoint + AuthActivityNameConstants.RolesQuery);
                        var paginationParameters = new PaginationParameters(skip, limit, sortBy, sortOrder);

                        var paginatedDtos = await handler.Handle(paginationParameters, cancellation);
                        return Results.Ok(paginatedDtos);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(ModuleClaims.QueryRoles);

            endpoints.MapGet(
                    RoleBasePath + "/{id:guid}",
                    async (GetRole handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(ModuleClaims.GetRoles);

            endpoints.MapPost(
                    RoleBasePath,
                    async (CreateRole handler, CreateOrUpdateRoleDto createRoleDto, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(createRoleDto, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(ModuleClaims.CreateRoles);

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
                            GroupNames = createRoleDto.GroupNames,
                        };
                        var result = await handler.Handle(command, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(ModuleClaims.UpdateRoles);

            endpoints.MapDelete(
                    RoleBasePath + "/{id:guid}",
                    async (DeleteRole handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Role)
                .RequireAuthorization(ModuleClaims.DeleteRoles);

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
                .RequireAuthorization(ModuleClaims.QueryGroups);

            endpoints.MapGet(
                    GroupBasePath + "/{id:guid}",
                    async (GetGroup handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Group)
                .RequireAuthorization(ModuleClaims.GetGroups);

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
                .RequireAuthorization(ModuleClaims.QueryPermissions);

            endpoints.MapGet(
                    PermissionBasePath + "/{id:guid}",
                    async (GetPermission handler, Guid id, CancellationToken cancellation) =>
                    {
                        var result = await handler.Handle(id, cancellation);
                        return Results.Ok(result);
                    })
                .WithTags(ApiGroupNameConstants.Permission)
                .RequireAuthorization(ModuleClaims.GetPermissions);
        }
    }
}

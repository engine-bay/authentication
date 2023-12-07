namespace EngineBay.Authentication
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.EntityFrameworkCore;

    public class AuthenticationModule : BaseModule, IDatabaseModule
    {
        private readonly string[] permissions =
        {
            PermissionConstants.QueryRoles,
            PermissionConstants.GetRoles,
            PermissionConstants.CreateRoles,
            PermissionConstants.UpdateRoles,
            PermissionConstants.DeleteRoles,
            PermissionConstants.QueryGroups,
            PermissionConstants.GetGroups,
            PermissionConstants.QueryPermissions,
            PermissionConstants.GetPermissions,
        };

        public override IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            // Register commands
            services.AddTransient<CreateUser>();
            services.AddTransient<CreateBasicAuthUser>();
            services.AddTransient<CreateAuthUser>();
            services.AddTransient<CreateRole>();
            services.AddTransient<UpdateRole>();
            services.AddTransient<DeleteRole>();
            services.AddTransient<CreateGroup>();
            services.AddTransient<CreatePermission>();

            // Register queries
            services.AddTransient<GetApplicationUser>();
            services.AddTransient<GetCurrentUser>();
            services.AddTransient<VerifyBasicAuthCredentials>();
            services.AddTransient<GetAuthUser>();
            services.AddTransient<GetRole>();
            services.AddTransient<QueryRoles>();
            services.AddTransient<GetGroup>();
            services.AddTransient<QueryGroups>();
            services.AddTransient<GetPermission>();
            services.AddTransient<QueryPermissions>();
            services.AddTransient<GetPermissionsByApplicationUserId>();

            // Register validators
            services.AddTransient<IValidator<CreateAuthUserDto>, CreateAuthUserDtoValidator>();
            services.AddTransient<IValidator<CreateOrUpdateRoleDto>, CreateRoleDtoValidator>();
            services.AddTransient<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();
            services.AddTransient<IValidator<CreateGroupDto>, CreateGroupDtoValidator>();
            services.AddTransient<IValidator<CreatePermissionDto>, CreatePermissionDtoValidator>();

            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    JwtBearerAuthenticationConfiguration.Configure(services);
                    break;
                case AuthenticationTypes.Basic:
                    Console.WriteLine("Warning: Basic authentication has been configured. The system is insecure.");
                    BasicAuthenticationConfiguration.Configure(services);
                    break;
                default:
                    Console.WriteLine("Warning: no authentication has been configured. The system is insecure.");
                    break;
            }

            services.AddScoped<ICurrentIdentity, CurrentIdentity>();

            // register persistence services
            var databaseConfiguration =
                new CQRSDatabaseConfiguration<AuthenticationDbContext, AuthenticationQueryDbContext,
                    AuthenticationWriteDbContext>();
            databaseConfiguration.RegisterDatabases(services);

            // Register authz policies
            services.AddAuthorization(
                options =>
                {
                    Array.ForEach(
                        this.permissions,
                        permission => options.AddPolicy(
                            permission,
                            policy => policy.RequireClaim(CustomClaimTypes.Scope, permission)));
                });

            return services;
        }

        public override RouteGroupBuilder MapEndpoints(RouteGroupBuilder endpoints)
        {
            AuthorizationEndpoints.MapEndpoints(endpoints);

            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

#pragma warning disable ASP0022 // allow duplicate routes for each of the authentication providers
            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    endpoints.MapPost(
                        "/register",
                        async (CreateUserDto createUserDto, CreateUser command, CancellationToken cancellation) =>
                        {
                            var applicationUserDto = await command.Handle(createUserDto, cancellation);

                            return Results.Ok(applicationUserDto);
                        }).AllowAnonymous();

                    break;
                case AuthenticationTypes.Basic:
                    endpoints.MapPost(
                        "/register",
                        async (
                            CreateBasicAuthUserDto createBasicAuthUserDto,
                            CreateBasicAuthUser command,
                            CancellationToken cancellation) =>
                        {
                            var applicationUserDto = await command.Handle(createBasicAuthUserDto, cancellation);

                            return Results.Ok(applicationUserDto);
                        }).AllowAnonymous();

                    break;
                case AuthenticationTypes.None:
                    endpoints.MapPost(
                        "/register",
                        (CancellationToken cancellation) => { throw new NotImplementedException(); }).AllowAnonymous();
                    break;
            }
#pragma warning restore ASP0022

            endpoints.MapGet(
                "/userInfo",
                async (GetCurrentUser query, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation) =>
                {
                    var applicationUserDto = await query.Handle(claimsPrincipal, cancellation);

                    return Results.Ok(applicationUserDto);
                }).RequireAuthorization();

            return endpoints;
        }

        public override WebApplication AddMiddleware(WebApplication app)
        {
            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();
            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                case AuthenticationTypes.Basic:
                    app.UseAuthentication();
                    app.UseAuthorization();

                    break;
            }

            return app;
        }

        public IReadOnlyCollection<IModuleDbContext> GetRegisteredDbContexts(
            DbContextOptions<ModuleWriteDbContext> dbOptions)
        {
            return new IModuleDbContext[] { new AuthenticationDbContext(dbOptions) };
        }

        public override void SeedDatabase(string seedDataPath, IServiceProvider serviceProvider)
        {
            var permissionDtos = Array.ConvertAll(this.permissions, permission => new CreatePermissionDto(permission));
            this.LoadSeedData<CreatePermissionDto, PermissionDto, CreatePermission>(permissionDtos, serviceProvider);

            this.LoadSeedData<CreateGroupDto, GroupDto, CreateGroup>(seedDataPath, "*.groups.json", serviceProvider);
            this.LoadSeedData<CreateOrUpdateRoleDto, RoleDto, CreateRole>(seedDataPath, "*.roles.json", serviceProvider);

            this.LoadSeedData<ApplicationUser, AuthenticationWriteDbContext>(seedDataPath, "*.users.json", serviceProvider);
            this.LoadSeedData<BasicAuthCredential, AuthenticationWriteDbContext>(seedDataPath, "*.basicauth.json", serviceProvider);

            this.LoadSeedData<CreateAuthUserDto, AuthUserDto, CreateAuthUser>(seedDataPath, "*.authusers.json", serviceProvider);
        }
    }
}
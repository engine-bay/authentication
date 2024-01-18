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
            ModuleClaims.QueryRoles,
            ModuleClaims.GetRoles,
            ModuleClaims.CreateRoles,
            ModuleClaims.UpdateRoles,
            ModuleClaims.DeleteRoles,
            ModuleClaims.QueryGroups,
            ModuleClaims.GetGroups,
            ModuleClaims.QueryPermissions,
            ModuleClaims.GetPermissions,
        };

        public override IServiceCollection RegisterPolicies(IServiceCollection services)
        {
            foreach (var permission in this.permissions)
            {
                services.AddAuthorizationBuilder().AddPolicy(permission, policy =>
                    policy.RequireClaim(CustomClaimTypes.Scope, permission));
            }

            return base.RegisterPolicies(services);
        }

        public override IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

            // Application User
            services.AddTransient<CreateApplicationUser>();
            services.AddTransient<GetApplicationUser>();

            // Authorization
            services.AddTransient<CreateUserRole>();
            services.AddTransient<CreateRole>();
            services.AddTransient<UpdateRole>();
            services.AddTransient<DeleteRole>();
            services.AddTransient<CreateGroup>();
            services.AddTransient<CreatePermission>();
            services.AddTransient<GetUserRoles>();
            services.AddTransient<GetRole>();
            services.AddTransient<QueryRoles>();
            services.AddTransient<GetGroup>();
            services.AddTransient<QueryGroups>();
            services.AddTransient<GetPermission>();
            services.AddTransient<QueryPermissions>();
            services.AddTransient<GetPermissionsByApplicationUserId>();
            services.AddTransient<IValidator<CreateUserRoleDto>, CreateUserRoleDtoValidator>();
            services.AddTransient<IValidator<CreateOrUpdateRoleDto>, CreateRoleDtoValidator>();
            services.AddTransient<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();
            services.AddTransient<IValidator<CreateGroupDto>, CreateGroupDtoValidator>();
            services.AddTransient<IValidator<CreatePermissionDto>, CreatePermissionDtoValidator>();

            // Authentication
            services.AddTransient<GetCurrentUser>();
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();

            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    JwtBearerAuthenticationConfiguration.Configure(services);
                    break;
                case AuthenticationTypes.Basic:
                    Console.WriteLine("Warning: Basic authentication has been configured. The system is insecure.");
                    services.AddTransient<CreateBasicAuthUser>();
                    services.AddTransient<VerifyBasicAuthCredentials>();
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
                        async (CreateUserDto createUserDto, CreateApplicationUser command, CancellationToken cancellation) =>
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
            DataBaseSeeder.LoadSeedData<CreatePermissionDto, PermissionDto, CreatePermission>(permissionDtos, serviceProvider);

            DataBaseSeeder.LoadSeedData<CreateGroupDto, GroupDto, CreateGroup>(seedDataPath, "*.groups.json", serviceProvider);
            DataBaseSeeder.LoadSeedData<CreateOrUpdateRoleDto, RoleDto, CreateRole>(seedDataPath, "*.roles.json", serviceProvider);

            DataBaseSeeder.LoadSeedData<ApplicationUser, AuthenticationWriteDbContext>(seedDataPath, "*.users.json", serviceProvider);
            DataBaseSeeder.LoadSeedData<BasicAuthCredential, AuthenticationWriteDbContext>(seedDataPath, "*.basicauth.json", serviceProvider);

            DataBaseSeeder.LoadSeedData<CreateUserRoleDto, UserRoleDto, CreateUserRole>(seedDataPath, "*.authusers.json", serviceProvider);
        }
    }
}
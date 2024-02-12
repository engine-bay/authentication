namespace EngineBay.Authentication
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using EngineBay.Authentication.Cookies;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.AspNetCore.Authentication;

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

        public AuthenticationModule()
        {
            this.AuthenticationType = AuthenticationConfiguration.GetAuthenticationMethod();
        }

        public AuthenticationTypes AuthenticationType { get; init; }

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
            services.AddScoped<ICurrentIdentity, CurrentIdentity>();

            // Application User
            services.AddTransient<CreateApplicationUser>();
            services.AddTransient<GetApplicationUser>();
            services.AddTransient<IClaimsTransformation, ClaimsTransformer>();
            services.AddTransient<GetCurrentUser>();

            // Identity

            // Custom Identity
            services.AddTransient<CreateBasicAuthUser>();
            services.AddTransient<VerifyBasicAuthCredentials>();
            var authenticationDbConfiguration =
                new CQRSDatabaseConfiguration<AuthenticationDbContext, AuthenticationQueryDbContext,
                    AuthenticationWriteDbContext>();
            authenticationDbConfiguration.RegisterDatabases(services);

            // Authorization

            // Custom Authorization
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

            var authorizationDbConfiguration =
                new CQRSDatabaseConfiguration<AuthorizationDbContext, AuthorizationQueryDbContext,
                    AuthorizationWriteDbContext>();
            authorizationDbConfiguration.RegisterDatabases(services);

            // JWT Bearer
            if (this.AuthenticationType == AuthenticationTypes.JwtBearer)
            {
                JwtBearerAuthenticationConfiguration.Configure(services);
            }

            // Basic Auth
            if (this.AuthenticationType == AuthenticationTypes.Basic)
            {
                Console.WriteLine("Warning: Basic authentication has been configured. The system is insecure.");
                BasicAuthenticationConfiguration.Configure(services);
            }

            // APS.NET Cookie Auth
            if (this.AuthenticationType == AuthenticationTypes.Cookies)
            {
                services.AddTransient<IValidator<SignInCredentials>, SignInCredentialsValidator>();
                services.AddTransient<SignIn>();
                CookieAuthenticationConfiguration.Configure(services);
            }

            return services;
        }

        public override RouteGroupBuilder MapEndpoints(RouteGroupBuilder endpoints)
        {
            AuthorizationEndpoints.MapEndpoints(endpoints);

#pragma warning disable ASP0022 // allow duplicate routes for each of the authentication providers
            switch (this.AuthenticationType)
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
                case AuthenticationTypes.Cookies:
                    CookieAuthEndpoints.MapEndpoints(endpoints);
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

        public IReadOnlyCollection<IModuleDbContext> GetRegisteredDbContexts(IDbContextOptionsFactory dbContextOptionsFactory)
        {
            ArgumentNullException.ThrowIfNull(dbContextOptionsFactory);

            var moduleDbContexts = new List<IModuleDbContext>();
            moduleDbContexts.Add(new AuthenticationDbContext(dbContextOptionsFactory.GetDbContextOptions<ModuleWriteDbContext>()));
            moduleDbContexts.Add(new AuthorizationDbContext(dbContextOptionsFactory.GetDbContextOptions<ModuleWriteDbContext>()));

            return moduleDbContexts;
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
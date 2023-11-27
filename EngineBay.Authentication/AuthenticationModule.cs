namespace EngineBay.Authentication
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;

    public class AuthenticationModule : BaseModule, IDatabaseModule
    {
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

            // Register validators
            services.AddTransient<IValidator<CreateAuthUserDto>, CreateAuthUserDtoValidator>();
            services.AddTransient<IValidator<CreateOrUpdateRoleDto>, CreateRoleDtoValidator>();
            services.AddTransient<IValidator<UpdateRoleCommand>, UpdateRoleCommandValidator>();
            services.AddTransient<IValidator<CreateGroupDto>, CreateGroupDtoValidator>();
            services.AddTransient<IValidator<CreatePermissionDto>, CreatePermissionDtoValidator>();

            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    JwtBearerAuthenticationConfiguration.Configure(services);
                    services.AddScoped<ICurrentIdentity, CurrentIdentityFromJwt>();
                    break;
                case AuthenticationTypes.Basic:
                    Console.WriteLine("Warning: Basic authentication has been configured. The system is insecure.");
                    BasicAuthenticationConfiguration.Configure(services);
                    services.AddScoped<ICurrentIdentity, CurrentIdentityFromBasicAuth>();
                    break;
                default:
                    services.AddScoped<ICurrentIdentity, CurrentIdentityFromNoAuth>();
                    Console.WriteLine("Warning: no authentication has been configured. The system is insecure.");
                    break;
            }

            // register persistence services
            var databaseConfiguration = new CQRSDatabaseConfiguration<AuthenticationDbContext, AuthenticationQueryDbContext, AuthenticationWriteDbContext>();
            databaseConfiguration.RegisterDatabases(services);

            return services;
        }

        public override RouteGroupBuilder MapEndpoints(RouteGroupBuilder endpoints)
        {
            AuthorizationEndpoints.MapEndpoints(endpoints);

            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    endpoints.MapPost("/register", async (CreateUserDto createUserDto, CreateUser command, CancellationToken cancellation) =>
                    {
                        var applicationUserDto = await command.Handle(createUserDto, cancellation);

                        return Results.Ok(applicationUserDto);
                    }).AllowAnonymous();

                    break;
                case AuthenticationTypes.Basic:
                    endpoints.MapPost("/register", async (CreateBasicAuthUserDto createBasicAuthUserDto, CreateBasicAuthUser command, CancellationToken cancellation) =>
                    {
                        var applicationUserDto = await command.Handle(createBasicAuthUserDto, cancellation);

                        return Results.Ok(applicationUserDto);
                    }).AllowAnonymous();

                    break;
                case AuthenticationTypes.None:
                    endpoints.MapPost("/register", (CancellationToken cancellation) =>
                    {
                        throw new NotImplementedException();
                    }).AllowAnonymous();
                    break;
            }

            endpoints.MapGet("/userInfo", async (GetCurrentUser query, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation) =>
            {
                var applicationUserDto = await query.Handle(claimsPrincipal, cancellation);

                return Results.Ok(applicationUserDto);
            }).RequireAuthorization();

            return endpoints;
        }

        public override WebApplication AddMiddleware(WebApplication app)
        {
            app.UseAuthorization();

            return app;
        }

        public IReadOnlyCollection<IModuleDbContext> GetRegisteredDbContexts(DbContextOptions<ModuleWriteDbContext> dbOptions)
        {
            return new IModuleDbContext[] { new AuthenticationDbContext(dbOptions) };
        }
    }
}
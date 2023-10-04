namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using EngineBay.Core;
    using EngineBay.Persistence;

    public class AuthenticationModule : IModule
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            // Register commands
            services.AddTransient<CreateUser>();
            services.AddTransient<CreateBasicAuthUser>();

            // Register queries
            services.AddTransient<GetApplicationUser>();
            services.AddTransient<GetCurrentUser>();
            services.AddTransient<VerifyBasicAuthCredentials>();

            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    JwtBearerAuthenticationConfiguration.Configure(services);
                    break;
                case AuthenticationTypes.Basic:
                    Console.WriteLine("Warning: no Basic authentication has been configured. The system is insecure.");
                    BasicAuthenticationConfiguration.Configure(services);
                    break;
                case AuthenticationTypes.None:
                    Console.WriteLine("Warning: no authentication has been configured. The system is insecure.");
                    break;
            }

            services.AddScoped<ICurrentIdentity, CurrentIdentity>();

            // register persistence services
            var databaseConfiguration = new CQRSDatabaseConfiguration<AuthenticationDbContext, AuthenticationQueryDbContext, AuthenticationWriteDbContext>();
            databaseConfiguration.RegisterDatabases(services);

            return services;
        }

        /// <inheritdoc/>
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            var authenticationType = AuthenticationConfiguration.GetAuthenticationMethod();

            switch (authenticationType)
            {
                case AuthenticationTypes.JwtBearer:
                    endpoints.MapPost("/register", async (CreateUserDto createUserDto, CreateUser command, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation) =>
                    {
                        var applicationUserDto = await command.Handle(createUserDto, claimsPrincipal, cancellation).ConfigureAwait(false);

                        return Results.Ok(applicationUserDto);
                    }).RequireAuthorization();

                    break;
                case AuthenticationTypes.Basic:
                    endpoints.MapPost("/register", async (CreateBasicAuthUserDto createBasicAuthUserDto, CreateBasicAuthUser command, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation) =>
                    {
                        var applicationUserDto = await command.Handle(createBasicAuthUserDto, claimsPrincipal, cancellation).ConfigureAwait(false);

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
                var applicationUserDto = await query.Handle(claimsPrincipal, cancellation).ConfigureAwait(false);

                return Results.Ok(applicationUserDto);
            }).RequireAuthorization();

            return endpoints;
        }

        public WebApplication AddMiddleware(WebApplication app)
        {
            app.UseAuthorization();

            return app;
        }
    }
}
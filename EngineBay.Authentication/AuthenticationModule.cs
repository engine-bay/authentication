namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using Microsoft.IdentityModel.Tokens;

    public class AuthenticationModule : IModule
    {
        /// <inheritdoc/>
        public IServiceCollection RegisterModule(IServiceCollection services, IConfiguration configuration)
        {
            // Register commands
            services.AddTransient<CreateUser>();

            // Register queries
            services.AddTransient<GetApplicationUser>();
            services.AddTransient<GetCurrentUser>();

            // register authentication services
            services.AddAuthentication().AddJwtBearer("Bearer", options =>
            {
                var algorithm = AuthenticationConfiguration.GetAlgorithm();
                var secretKey = AuthenticationConfiguration.GetSigningKey();
                byte[] key;

                switch (algorithm)
                {
                    case SigningAlgorithmsTypes.HS256:
                        var hmac256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                        key = hmac256.Key;
                        break;
                    case SigningAlgorithmsTypes.HS512:
                        var hmac512 = new HMACSHA512(Encoding.UTF8.GetBytes(secretKey));
                        key = hmac512.Key;
                        break;
                    default:
                        throw new ArgumentException("Unsupported Signing Algorithm");
                }

                var signingKey = new SymmetricSecurityKey(key);

                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = AuthenticationConfiguration.ShouldValidateIssuerSigningKey(),
                    TryAllIssuerSigningKeys = AuthenticationConfiguration.ShouldValidateIssuerSigningKey(),
                    IssuerSigningKey = signingKey,
                    IssuerSigningKeys = new List<SecurityKey>() { signingKey },
                    RequireSignedTokens = AuthenticationConfiguration.ShouldValidateSignedTokens(),
                    ValidAlgorithms = AuthenticationConfiguration.GetAlgorithms(),

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = AuthenticationConfiguration.ShouldValidateIssuer(),
                    ValidIssuers = AuthenticationConfiguration.GetIssuers(),

                    // Validate the JWT Audience (aud) claim
                    RequireAudience = AuthenticationConfiguration.ShouldValidateAudience(),
                    ValidateAudience = AuthenticationConfiguration.ShouldValidateAudience(),
                    ValidAudiences = AuthenticationConfiguration.GetAudiences(),

                    // Validate the token expiry
                    ValidateLifetime = AuthenticationConfiguration.ShouldValidateExpiry(),
                    RequireExpirationTime = AuthenticationConfiguration.ShouldValidateExpiry(),

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.Zero,
                };
            });
            services.AddAuthorization();

            // register persistence services
            var databaseConfiguration = new CQRSDatabaseConfiguration<AuthenticationDbContext, AuthenticationQueryDbContext, AuthenticationWriteDbContext>();
            databaseConfiguration.RegisterDatabases(services);

            return services;
        }

        /// <inheritdoc/>
        public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/register", async (CreateUserDto createUserDto, CreateUser command, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation) =>
           {
               var applicationUserDto = await command.Handle(createUserDto, claimsPrincipal, cancellation).ConfigureAwait(false);

               return Results.Ok(applicationUserDto);
           }).RequireAuthorization();

            endpoints.MapGet("/userInfo", async (GetCurrentUser query, ClaimsPrincipal claimsPrincipal, CancellationToken cancellation) =>
            {
                var applicationUserDto = await query.Handle(claimsPrincipal, cancellation).ConfigureAwait(false);

                return Results.Ok(applicationUserDto);
            }).RequireAuthorization();

            return endpoints;
        }
    }
}
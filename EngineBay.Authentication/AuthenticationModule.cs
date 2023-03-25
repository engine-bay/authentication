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

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAlgorithms = AuthenticationConfiguration.GetAlgorithms(),

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.Zero,
                };

                if (AuthenticationConfiguration.ShouldValidateIssuerSigningKey())
                {
                    // The signing key must match!
                    tokenValidationParameters.ValidateIssuerSigningKey = true;
                    tokenValidationParameters.TryAllIssuerSigningKeys = true;
                    tokenValidationParameters.IssuerSigningKey = signingKey;
                    tokenValidationParameters.IssuerSigningKeys = new List<SecurityKey>() { signingKey };
                }

                if (AuthenticationConfiguration.ShouldValidateSignedTokens())
                {
                    tokenValidationParameters.RequireSignedTokens = true;
                }

                if (AuthenticationConfiguration.ShouldValidateIssuer())
                {
                    // Validate the JWT Issuer (iss) claim
                    tokenValidationParameters.ValidateIssuer = true;
                    tokenValidationParameters.ValidIssuers = AuthenticationConfiguration.GetIssuers();
                }

                if (AuthenticationConfiguration.ShouldValidateAudience())
                {
                    // Validate the JWT Audience (aud) claim
                    tokenValidationParameters.RequireAudience = true;
                    tokenValidationParameters.ValidateAudience = true;
                    tokenValidationParameters.ValidAudiences = AuthenticationConfiguration.GetAudiences();
                }

                if (AuthenticationConfiguration.ShouldValidateExpiry())
                {
                    // Validate the token expiry
                    tokenValidationParameters.ValidateLifetime = true;
                    tokenValidationParameters.RequireExpirationTime = true;
                }

                options.TokenValidationParameters = tokenValidationParameters;
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
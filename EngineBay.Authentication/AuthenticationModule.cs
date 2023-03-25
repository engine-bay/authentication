namespace EngineBay.Authentication
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using EngineBay.Core;
    using EngineBay.Persistence;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
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
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
#pragma warning disable CA5404 // we allow disabling of important validation here since we're trying to make it configurable
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

                var signingKey = new SymmetricSecurityKey(key)
                {
                    KeyId = algorithm.ToString(),
                };

                var tokenValidationParameters = new TokenValidationParameters()
                {
                    ValidAlgorithms = AuthenticationConfiguration.GetAlgorithms(),
                    ValidateActor = false,
                    ValidateTokenReplay = false,

                    // If you want to allow a certain amount of clock drift, set that here:
                    ClockSkew = TimeSpan.FromSeconds(5.0),
                };

                if (AuthenticationConfiguration.ShouldValidateIssuerSigningKey())
                {
                    // The signing key must match!
                    tokenValidationParameters.ValidateIssuerSigningKey = true;
                    tokenValidationParameters.TryAllIssuerSigningKeys = true;
                    tokenValidationParameters.IssuerSigningKey = signingKey;
                    tokenValidationParameters.IssuerSigningKeys = new List<SecurityKey>() { signingKey };
                }
                else
                {
                    tokenValidationParameters.ValidateIssuerSigningKey = false;
                    tokenValidationParameters.TryAllIssuerSigningKeys = false;
                    tokenValidationParameters.SignatureValidator = (token, parameters) =>
                    {
                        var jwt = new JwtSecurityToken(token);

                        return jwt;
                    };
                }

                if (AuthenticationConfiguration.ShouldValidateSignedTokens())
                {
                    tokenValidationParameters.RequireSignedTokens = true;
                }
                else
                {
                    tokenValidationParameters.RequireSignedTokens = false;
                }

                if (AuthenticationConfiguration.ShouldValidateIssuer())
                {
                    // Validate the JWT Issuer (iss) claim
                    var issuer = AuthenticationConfiguration.GetIssuer();
                    tokenValidationParameters.ValidateIssuer = true;
                    tokenValidationParameters.ValidIssuers = AuthenticationConfiguration.GetIssuers();
                    tokenValidationParameters.ValidIssuer = issuer;
                    options.ClaimsIssuer = issuer;
                }
                else
                {
                    tokenValidationParameters.ValidateIssuer = false;
                }

                if (AuthenticationConfiguration.ShouldValidateAudience())
                {
                    // Validate the JWT Audience (aud) claim
                    var audience = AuthenticationConfiguration.GetAudience();
                    tokenValidationParameters.RequireAudience = true;
                    tokenValidationParameters.ValidateAudience = true;
                    tokenValidationParameters.ValidAudiences = AuthenticationConfiguration.GetAudiences();
                    tokenValidationParameters.ValidAudience = audience;
                    options.Audience = audience;
                }
                else
                {
                    tokenValidationParameters.RequireAudience = false;
                    tokenValidationParameters.ValidateAudience = false;
                }

                if (AuthenticationConfiguration.ShouldValidateExpiry())
                {
                    // Validate the token expiry
                    tokenValidationParameters.ValidateLifetime = true;
                    tokenValidationParameters.RequireExpirationTime = true;
                }
                else
                {
                    tokenValidationParameters.ValidateLifetime = false;
                    tokenValidationParameters.RequireExpirationTime = false;
                }

                options.IncludeErrorDetails = true;
                options.Authority = AuthenticationConfiguration.GetAuthority();
                options.TokenValidationParameters = tokenValidationParameters;
                options.RequireHttpsMetadata = false;
#pragma warning restore CA5404
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
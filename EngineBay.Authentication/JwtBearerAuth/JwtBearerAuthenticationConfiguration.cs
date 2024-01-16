namespace EngineBay.Authentication
{
    using System.Security.Cryptography;
    using System.Text;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.IdentityModel.Tokens;

    public abstract class JwtBearerAuthenticationConfiguration
    {
        public static IServiceCollection Configure(IServiceCollection services)
        {
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
                    ClockSkew = TimeSpan.FromSeconds(1.0),
                };

                if (AuthenticationConfiguration.ShouldValidateIssuerSigningKey())
                {
                    // The signing key must match!
                    tokenValidationParameters.ValidateIssuerSigningKey = true;
                    tokenValidationParameters.IssuerSigningKey = signingKey;
                }
                else
                {
                    tokenValidationParameters.ValidateIssuerSigningKey = false;
                    tokenValidationParameters.TryAllIssuerSigningKeys = false;
                    tokenValidationParameters.SignatureValidator = (token, parameters) =>
                    {
                        // var jwt = new JwtSecurityToken(token);
                        var jwt = new JsonWebToken(token);

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

                options.IncludeErrorDetails = false;
                options.Authority = AuthenticationConfiguration.GetAuthority();
                options.TokenValidationParameters = tokenValidationParameters;
                options.RequireHttpsMetadata = false;
#pragma warning restore CA5404
            });

            services.AddAuthorization();

            return services;
        }
    }
}
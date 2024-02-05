namespace EngineBay.Authentication
{
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using EngineBay.DemoModule;
    using EngineBay.Telemetry;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.Extensions.Options;

    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly VerifyBasicAuthCredentials verifyBasicAuthCredentials;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            VerifyBasicAuthCredentials verifyBasicAuthCredentials)
            : base(options, logger, encoder)
        {
            this.verifyBasicAuthCredentials = verifyBasicAuthCredentials;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            using var activity = EngineBayActivitySource.Source.StartActivity(TracingActivityNameConstants.Handler + AuthActivityNameConstants.AuthBasic);

            var authHeader = this.Request.Headers["Authorization"].ToString();
            if (authHeader != null && authHeader.StartsWith("basic", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Basic ".Length).Trim();

                var credentialsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
                var credentials = credentialsString.Split(':');

                var username = credentials[0];
                var password = credentials[1];

                var claims = new[] { new Claim(CustomClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, "Basic");
                var claimsPrincipal = new ClaimsPrincipal(identity);

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    this.Response.StatusCode = 401;
                    return AuthenticateResult.Fail("Invalid Authorization Header");
                }

                var basicAuthCredentialsDto = new BasicAuthCredentialsDto()
                {
                    ClaimsPrincipal = claimsPrincipal,
                    Password = password,
                };

                var isAuthenticated = await this.verifyBasicAuthCredentials.Handle(
                    basicAuthCredentialsDto,
                    CancellationToken.None);

                if (isAuthenticated)
                {
                    return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, this.Scheme.Name));
                }

                this.Response.StatusCode = 401;
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
            else
            {
                this.Response.StatusCode = 401;
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }
        }
    }
}
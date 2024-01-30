namespace EngineBay.Authentication.Cookies
{
    using System.Security.Claims;
    using EngineBay.Core;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;

    public class SignIn : ICommandHandler<SignInCredentials, bool>
    {
        private readonly AuthenticationDbContext authDb;
        private readonly VerifyBasicAuthCredentials verifyBasicAuthCredentials;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SignIn(IHttpContextAccessor httpContextAccessor, AuthenticationDbContext authDb, VerifyBasicAuthCredentials verifyBasicAuthCredentials)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.authDb = authDb;
            this.verifyBasicAuthCredentials = verifyBasicAuthCredentials;
        }

        public async Task<bool> Handle(SignInCredentials command, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(this.httpContextAccessor);
            ArgumentNullException.ThrowIfNull(this.httpContextAccessor.HttpContext);

            var claims = new[] { new Claim(CustomClaimTypes.Name, command.Username) };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var basicAuthCredentialsDto = new BasicAuthCredentialsDto()
            {
                ClaimsPrincipal = claimsPrincipal,
                Password = command.Password,
            };

            var authProperties = new AuthenticationProperties
            {
            };

            var isAuthenticated = await this.verifyBasicAuthCredentials.Handle(basicAuthCredentialsDto, cancellation);

            await this.httpContextAccessor.HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return isAuthenticated;
        }
    }
}
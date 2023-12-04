namespace EngineBay.Authentication
{
    using BCrypt.Net;
    using EngineBay.Core;
    using Microsoft.EntityFrameworkCore;

    public class VerifyBasicAuthCredentials : IQueryHandler<BasicAuthCredentialsDto, bool>
    {
        private readonly ILogger<VerifyBasicAuthCredentials> logger;
        private readonly GetApplicationUser getApplicationUserQuery;
        private readonly AuthenticationQueryDbContext authenticationQueryDbContext;

        public VerifyBasicAuthCredentials(
            ILogger<VerifyBasicAuthCredentials> logger,
            GetApplicationUser getApplicationUserQuery,
            AuthenticationQueryDbContext authenticationQueryDbContext)
        {
            this.logger = logger;
            this.getApplicationUserQuery = getApplicationUserQuery;
            this.authenticationQueryDbContext = authenticationQueryDbContext;
        }

        /// <inheritdoc/>
        public async Task<bool> Handle(BasicAuthCredentialsDto credentialsDto, CancellationToken cancellation)
        {
            ArgumentNullException.ThrowIfNull(credentialsDto);

            var claimsPrincipal = credentialsDto.ClaimsPrincipal;
            var password = credentialsDto.Password;

            if (claimsPrincipal is null)
            {
                throw new ArgumentException(nameof(claimsPrincipal));
            }

            if (claimsPrincipal.Identity is null)
            {
                throw new ArgumentException(nameof(claimsPrincipal.Identity));
            }

            var applicationUser = await this.getApplicationUserQuery.Handle(claimsPrincipal, cancellation);

            if (applicationUser is null)
            {
                this.logger.UserDoesNotExist();
                return false;
            }

            var basicAuthCredentials =
                await this.authenticationQueryDbContext.BasicAuthCredentials.SingleOrDefaultAsync(
                    x => x.ApplicationUserId == applicationUser.Id,
                    cancellation);

            if (basicAuthCredentials is null)
            {
                this.logger.UserDoesNotHaveBasicAuthCredentials();
                return false;
            }

            return BCrypt.EnhancedVerify(password, basicAuthCredentials.PasswordHash);
        }
    }
}
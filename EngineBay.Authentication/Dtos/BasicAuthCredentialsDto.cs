namespace EngineBay.Authentication
{
    using System.Security.Claims;

    public class BasicAuthCredentialsDto
    {
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }

        public string? Password { get; set; }
    }
}
namespace EngineBay.Authentication
{
    public static class DefaultAuthenticationConfigurationConstants
    {
        public const string DefaultAudience = "http://localhost";

        public const string DefaultIssuer = "http://localhost";

        public const SigningAlgorithmsTypes DefaultAlgorithm = SigningAlgorithmsTypes.HS256;

        public const AuthenticationTypes DefaultAuthentication = AuthenticationTypes.JwtBearer;
    }
}
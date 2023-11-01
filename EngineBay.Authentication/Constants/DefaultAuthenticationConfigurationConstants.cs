namespace EngineBay.Authentication
{
    public static class DefaultAuthenticationConfigurationConstants
    {
        public const string DefaultAudience = "http://localhost:5050";

        public const string DefaultIssuer = "http://localhost:5050";

        public const string SystemUserName = "System";

        public const string UnauthenticatedUserName = "Unauthenticated User";

        public const SigningAlgorithmsTypes DefaultAlgorithm = SigningAlgorithmsTypes.HS256;

        public const AuthenticationTypes DefaultAuthentication = AuthenticationTypes.JwtBearer;
    }
}
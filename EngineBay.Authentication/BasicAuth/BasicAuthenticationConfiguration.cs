namespace EngineBay.Authentication
{
    using Microsoft.AspNetCore.Authentication;

    public abstract class BasicAuthenticationConfiguration
    {
        public static IServiceCollection Configure(IServiceCollection services)
        {
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(
                "BasicAuthentication", null);

            services.AddAuthorization();

            return services;
        }
    }
}
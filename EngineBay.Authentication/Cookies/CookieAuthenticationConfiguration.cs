namespace EngineBay.Authentication.Cookies
{
    using Microsoft.AspNetCore.Authentication.Cookies;

    public abstract class CookieAuthenticationConfiguration
    {
        public static IServiceCollection Configure(IServiceCollection services)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                    {
                        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                        options.SlidingExpiration = true;
                    });

            services.AddAuthorization();

            return services;
        }
    }
}
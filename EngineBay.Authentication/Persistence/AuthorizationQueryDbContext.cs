namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class AuthorizationQueryDbContext : AuthorizationDbContext
    {
        public AuthorizationQueryDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }
    }
}
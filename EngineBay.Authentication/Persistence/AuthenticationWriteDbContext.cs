namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class AuthenticationWriteDbContext : AuthenticationQueryDbContext
    {
        private readonly IAuditingInterceptor auditingInterceptor;

        public AuthenticationWriteDbContext(DbContextOptions<ModuleWriteDbContext> options, IAuditingInterceptor auditingInterceptor)
            : base(options)
        {
            this.auditingInterceptor = auditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            optionsBuilder.AddInterceptors(this.auditingInterceptor);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
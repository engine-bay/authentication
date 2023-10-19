namespace EngineBay.Authentication
{
    using EngineBay.Auditing;
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class AuthenticationWriteDbContext : AuthenticationQueryDbContext
    {
        private readonly AuditingInterceptor databaseAuditingInterceptor;

        public AuthenticationWriteDbContext(DbContextOptions<ModuleWriteDbContext> options, AuditingInterceptor databaseAuditingInterceptor)
            : base(options)
        {
            this.databaseAuditingInterceptor = databaseAuditingInterceptor;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            ArgumentNullException.ThrowIfNull(optionsBuilder);

            optionsBuilder.AddInterceptors(this.databaseAuditingInterceptor);

            base.OnConfiguring(optionsBuilder);
        }
    }
}
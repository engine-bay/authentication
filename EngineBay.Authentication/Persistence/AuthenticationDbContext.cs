namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Microsoft.EntityFrameworkCore;

    public class AuthenticationDbContext : ModuleWriteDbContext
    {
        public AuthenticationDbContext(DbContextOptions<ModuleWriteDbContext> options)
            : base(options)
        {
        }

        public DbSet<BasicAuthCredential> BasicAuthCredentials { get; set; } = null!;

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BasicAuthCredential.CreateDataAnnotations(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
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

        public DbSet<UserRole> UserRoles { get; set; } = null!;

        public DbSet<Role> Roles { get; set; } = null!;

        public DbSet<Group> Groups { get; set; } = null!;

        public DbSet<Permission> Permissions { get; set; } = null!;

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            BasicAuthCredential.CreateDataAnnotations(modelBuilder);
            UserRole.CreateDataAnnotations(modelBuilder);
            Role.CreateDataAnnotations(modelBuilder);
            Group.CreateDataAnnotations(modelBuilder);
            Permission.CreateDataAnnotations(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }
    }
}
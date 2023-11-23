namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class AuthUser : AuditableModel
    {
        public AuthUser(Guid userId)
        {
            this.ApplicationUserId = userId;
        }

        public Guid ApplicationUserId { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual ICollection<Role>? Roles { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<AuthUser>().ToTable(typeof(AuthUser).Name.Pluralize());

            modelBuilder.Entity<AuthUser>().HasKey(x => x.Id);

            modelBuilder.Entity<AuthUser>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<AuthUser>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<AuthUser>().Property(x => x.CreatedById);

            modelBuilder.Entity<AuthUser>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<AuthUser>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<AuthUser>().Ignore(x => x.LastUpdatedBy);

            modelBuilder.Entity<AuthUser>().Property(x => x.ApplicationUserId).IsRequired();

            modelBuilder.Entity<AuthUser>().HasIndex(x => x.ApplicationUserId).IsUnique();

            modelBuilder.Entity<AuthUser>().HasOne(x => x.ApplicationUser).WithOne().HasForeignKey(typeof(AuthUser), nameof(AuthUser.ApplicationUserId));

            modelBuilder.Entity<AuthUser>().HasMany(x => x.Roles).WithMany(x => x.Users);
        }
    }
}
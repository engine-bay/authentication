namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class UserRole : AuditableModel
    {
        public Guid ApplicationUserId { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        public virtual ICollection<Role>? Roles { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.Entity<UserRole>().ToTable(typeof(UserRole).Name.Pluralize());

            modelBuilder.Entity<UserRole>().HasKey(x => x.Id);

            modelBuilder.Entity<UserRole>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<UserRole>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<UserRole>().Property(x => x.CreatedById);

            modelBuilder.Entity<UserRole>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<UserRole>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<UserRole>().Ignore(x => x.LastUpdatedBy);

            modelBuilder.Entity<UserRole>().Property(x => x.ApplicationUserId).IsRequired();

            modelBuilder.Entity<UserRole>().HasIndex(x => x.ApplicationUserId).IsUnique();

            modelBuilder.Entity<UserRole>().HasOne(x => x.ApplicationUser).WithOne().HasForeignKey(
                typeof(UserRole),
                nameof(UserRole.ApplicationUserId));

            modelBuilder.Entity<UserRole>().HasMany(x => x.Roles).WithMany(x => x.Users);
        }
    }
}
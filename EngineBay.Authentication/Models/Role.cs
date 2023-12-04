namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class Role : AuditableModel
    {
        public Role()
        {
            this.Name = string.Empty;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<AuthUser>? Users { get; set; }

        public virtual ICollection<Group>? Groups { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.Entity<Role>().ToTable(typeof(Role).Name.Pluralize());

            modelBuilder.Entity<Role>().HasKey(x => x.Id);

            modelBuilder.Entity<Role>().Property(x => x.Name).IsRequired();

            modelBuilder.Entity<Role>().HasIndex(x => x.Name).IsUnique();

            modelBuilder.Entity<Role>().Property(x => x.Description);

            modelBuilder.Entity<Role>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<Role>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<Role>().Property(x => x.CreatedById);

            modelBuilder.Entity<Role>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<Role>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<Role>().Ignore(x => x.LastUpdatedBy);

            modelBuilder.Entity<Role>().HasMany(x => x.Groups).WithMany(x => x.Roles);
        }
    }
}
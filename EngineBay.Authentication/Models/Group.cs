namespace EngineBay.Authentication
{
    using EngineBay.Persistence;
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class Group : AuditableModel
    {
        public Group()
        {
            this.Name = string.Empty;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<Role>? Roles { get; set; }

        public virtual ICollection<Permission>? Permissions { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            ArgumentNullException.ThrowIfNull(modelBuilder);

            modelBuilder.Entity<Group>().ToTable(typeof(Group).Name.Pluralize());

            modelBuilder.Entity<Group>().HasKey(x => x.Id);

            modelBuilder.Entity<Group>().Property(x => x.Name).IsRequired();

            modelBuilder.Entity<Group>().Property(x => x.Description);

            modelBuilder.Entity<Group>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<Group>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<Group>().Property(x => x.CreatedById);

            modelBuilder.Entity<Group>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<Group>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<Group>().Ignore(x => x.LastUpdatedBy);

            modelBuilder.Entity<Group>().HasMany(x => x.Permissions).WithMany(x => x.Groups);
        }
    }
}
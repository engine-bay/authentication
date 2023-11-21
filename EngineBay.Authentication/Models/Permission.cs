namespace EngineBay.Persistence
{
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class Permission : AuditableModel
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    {
        public Permission()
        {
            this.Name = string.Empty;
        }

        public string Name { get; set; }

        public virtual ICollection<Group>? Groups { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<Permission>().ToTable(typeof(Permission).Name.Pluralize());

            modelBuilder.Entity<Permission>().HasKey(x => x.Id);

            modelBuilder.Entity<Permission>().Property(x => x.Name).IsRequired();

            modelBuilder.Entity<Permission>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<Permission>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<Permission>().Property(x => x.CreatedById);

            modelBuilder.Entity<Permission>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<Permission>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<Permission>().Ignore(x => x.LastUpdatedBy);
        }
    }
}
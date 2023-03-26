namespace EngineBay.Persistence
{
    using Humanizer;
    using Microsoft.EntityFrameworkCore;

    public class BasicAuthCredential : AuditableModel
    {
        public Guid ApplicationUserId { get; set; }

        public virtual ApplicationUser? ApplicationUser { get; set; }

        public string? Salt { get; set; }

        public string? PasswordHash { get; set; }

        public static new void CreateDataAnnotations(ModelBuilder modelBuilder)
        {
            if (modelBuilder is null)
            {
                throw new ArgumentNullException(nameof(modelBuilder));
            }

            modelBuilder.Entity<BasicAuthCredential>().ToTable(typeof(BasicAuthCredential).Name.Pluralize());

            modelBuilder.Entity<BasicAuthCredential>().HasKey(x => x.Id);

            modelBuilder.Entity<BasicAuthCredential>().Property(x => x.CreatedAt).IsRequired();

            modelBuilder.Entity<BasicAuthCredential>().Property(x => x.LastUpdatedAt).IsRequired();

            modelBuilder.Entity<BasicAuthCredential>().Property(x => x.CreatedById);

            modelBuilder.Entity<BasicAuthCredential>().Ignore(x => x.CreatedBy);

            modelBuilder.Entity<BasicAuthCredential>().Property(x => x.LastUpdatedById);

            modelBuilder.Entity<BasicAuthCredential>().Ignore(x => x.LastUpdatedBy);

            modelBuilder.Entity<BasicAuthCredential>().Property(x => x.PasswordHash).IsRequired();

            modelBuilder.Entity<BasicAuthCredential>().Property(x => x.ApplicationUserId).IsRequired();

            modelBuilder.Entity<BasicAuthCredential>().HasIndex(x => x.ApplicationUserId).IsUnique();

            modelBuilder.Entity<BasicAuthCredential>().HasOne(x => x.ApplicationUser).WithMany().HasForeignKey(x => x.ApplicationUserId);
        }
    }
}
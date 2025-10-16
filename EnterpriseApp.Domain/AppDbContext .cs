using EnterpriseApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseApp.Domain
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> context) : base(context)
        {

        }

        public DbSet<Company> Companies => Set<Company>();

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Company>(e =>
            {
                e.ToTable("Companies");
                e.HasKey(x => x.Id);
                e.Property(x => x.Identification).IsRequired().HasMaxLength(11);
                e.HasIndex(x => x.Identification).IsUnique();
                e.Property(x => x.Name).IsRequired().HasMaxLength(200);
                e.Property(x => x.ComercialName).IsRequired().HasMaxLength(200);
                e.Property(x => x.Category).HasMaxLength(100);
                e.Property(x => x.PaymentScheme).IsRequired().HasMaxLength(100);
                e.Property(x => x.Status).IsRequired().HasMaxLength(50);
                e.Property(x => x.EconomicActivity).IsRequired().HasMaxLength(200);
                e.Property(x => x.GovernmentBranch).IsRequired().HasMaxLength(200);
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETDATE()");
                e.Property(x => x.IsDeleted).HasDefaultValue(false);
            });
        }
    }
}

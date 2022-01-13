using Domain.Models;
using REITs.DataLayer.Configurations;
using REITs.Domain.Models;
using System.Data.Entity;
using System.Diagnostics;

namespace REITs.DataLayer.Contexts
{
    public class ApplicationContext : DbContext
    {
        static ApplicationContext()
        {
            if (Debugger.IsAttached == true)
            {
                Database.SetInitializer(new CreateDatabaseIfNotExists<ApplicationContext>());
            }
            else
            {
                Database.SetInitializer<ApplicationContext>(null);
            }
        }

        public ApplicationContext() : base("name=DatabaseConnectionString")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("REIT");
            modelBuilder.Configurations.Add(new REITConfiguration());
            modelBuilder.Configurations.Add(new REITParentConfiguration());
            modelBuilder.Configurations.Add(new REITParentReviewConfigurationFS());
            modelBuilder.Configurations.Add(new REITParentReviewConfigurationRFS());
            modelBuilder.Configurations.Add(new REITTotalsConfiguration());
            modelBuilder.Configurations.Add(new EntityConfiguration());
            modelBuilder.Configurations.Add(new AdjustmentConfiguration());
            modelBuilder.Configurations.Add(new ReconciliationConfiguration());
            modelBuilder.Configurations.Add(new AdjustmentTypeConfiguration());
            modelBuilder.Configurations.Add(new SystemAdminConfiguration());
            modelBuilder.Configurations.Add(new SystemUserConfiguration());
        }

        public DbSet<REITParent> REITParents { get; set; }
        public DbSet<REITParentReviewFS> REITParentReviewsFS { get; set; }
        public DbSet<REITParentReviewRFS> REITParentReviewsRFS { get; set; }
        public DbSet<REIT> REITs { get; set; }
        public DbSet<REITTotals> REITTotals { get; set; }
        public DbSet<Entity> Entities { get; set; }
        public DbSet<Adjustment> Adjustments { get; set; }
        public DbSet<Reconciliation> Reconciliations { get; set; }
        public DbSet<AdjustmentType> AdjustmentTypes { get; set; }
        public DbSet<SystemAdmin> SystemAdmins { get; set; }
        public DbSet<SystemUser> SystemUsers { get; set; }
    }
}
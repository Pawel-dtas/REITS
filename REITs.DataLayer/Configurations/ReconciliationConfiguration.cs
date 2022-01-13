using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class ReconciliationConfiguration : EntityTypeConfiguration<Reconciliation>
    {
        public ReconciliationConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.ReconciliationType)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.ReconciliationName)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.ReconciliationAmount)
                .IsRequired();
            Property(p => p.REITId)
             .IsRequired();
        }
    }
}
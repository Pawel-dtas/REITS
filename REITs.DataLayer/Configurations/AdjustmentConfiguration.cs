using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class AdjustmentConfiguration : EntityTypeConfiguration<Adjustment>
    {
        public AdjustmentConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.AdjustmentCategory)
              .HasMaxLength(100)
              .IsRequired();
            Property(a => a.AdjustmentType)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.AdjustmentName)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.AdjustmentAmount)
                .IsRequired();
            Property(p => p.EntityId)
            .IsRequired();
        }
    }
}
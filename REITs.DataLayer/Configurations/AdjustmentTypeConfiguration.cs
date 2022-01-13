using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class AdjustmentTypeConfiguration : EntityTypeConfiguration<AdjustmentType>
    {
        public AdjustmentTypeConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.AdjustmentCategory)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.AdjustmentName)
                .HasMaxLength(100)
                .IsRequired();

        }
    }
}

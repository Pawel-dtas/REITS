using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class EntityConfiguration : EntityTypeConfiguration<Entity>
    {
        public EntityConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.EntityName)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.EntityType)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.EntityUTR)
                .HasMaxLength(10)
                .IsOptional();
            Property(a => a.InterestPercentage)
                .IsRequired();
            Property(p => p.Jurisdiction)
               .HasMaxLength(50)
              .IsRequired();
            Property(p => p.TaxExempt)
              .IsRequired();
            Property(p => p.CustomerReference)
                 .HasMaxLength(20)
              .IsOptional();
            Property(p => p.REITId)
             .IsRequired();

            //HasMany(a => a.Adjustments)
            //    .WithRequired().HasForeignKey(f => f.EntityId)
            //    .WillCascadeOnDelete(false);
        }
    }
}
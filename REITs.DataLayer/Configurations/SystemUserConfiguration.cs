using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class SystemUserConfiguration : EntityTypeConfiguration<SystemUser>
    {
        public SystemUserConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.PINumber)
               .HasMaxLength(7)
               .IsRequired();
            Property(a => a.Forename)
               .HasMaxLength(30)
               .IsRequired();
            Property(a => a.Surname)
               .HasMaxLength(30)
               .IsRequired();
            Property(a => a.TelephoneNumber)
               .HasMaxLength(30)
               .IsRequired();
            Property(a => a.Team)
                .HasMaxLength(50)
                .IsOptional();
            Property(a => a.JobRole)
                .HasMaxLength(50)
                .IsOptional();
            Property(a => a.AccessLevel)
                .HasMaxLength(20)
                .IsRequired();
            Property(p => p.CreatedBy)
               .HasMaxLength(7)
               .IsFixedLength()
              .IsRequired();
            Property(p => p.UpdatedBy)
               .HasMaxLength(7)
               .IsFixedLength()
              .IsRequired();
            Property(p => p.DateCreated)
                .IsOptional();
            Property(p => p.DateUpdated)
                .IsOptional();
            Property(p => p.IsActive)
                .IsRequired();
        }
    }
}
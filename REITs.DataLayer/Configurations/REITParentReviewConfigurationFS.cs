using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class REITParentReviewConfigurationFS : EntityTypeConfiguration<REITParentReviewFS>
    {
        public REITParentReviewConfigurationFS()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.ParentId)
                .IsRequired();

            Property(a => a.FSAPEYear)
                .IsRequired();

            Property(a => a.FSDue)
                .IsOptional();

            Property(a => a.FSRecDate)
                .IsOptional();

            Property(a => a.PIDDueDate)
                .IsOptional();

            Property(a => a.PIDRecDate)
                .IsOptional();

            Property(a => a.Comments)
                .IsOptional();

            Property(a => a.UpdatedBy)
                .HasMaxLength(7)
                .IsFixedLength()
                .IsRequired();
            Property(p => p.DateUpdated)
                .IsRequired();

            Property(a => a.CreatedBy)
                .HasMaxLength(7)
                .IsFixedLength()
                .IsRequired();

            Property(p => p.DateCreated)
                .IsRequired();
        }
    }
}
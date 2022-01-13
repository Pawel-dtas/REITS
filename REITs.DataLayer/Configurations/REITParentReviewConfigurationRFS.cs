using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class REITParentReviewConfigurationRFS : EntityTypeConfiguration<REITParentReviewRFS>
    {
        public REITParentReviewConfigurationRFS()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.ParentId)
                .IsRequired();

            Property(a => a.RFSAPEYear)
                .IsRequired();

            Property(a => a.FSReviewedAPEDate)
                .IsOptional();

            Property(a => a.RiskStatus)
                .HasMaxLength(10)
                .IsOptional();

            Property(a => a.OnBRRTT)
                .HasMaxLength(20)
                .IsOptional();

            Property(a => a.InternalBRRDueDate)
                .IsOptional();

            Property(a => a.RFSReviewedDate)
                .IsOptional();

            Property(a => a.RAPlanMeetDate)
                .IsOptional();

            Property(a => a.ReviewedDate)
                .IsOptional();

            Property(a => a.NextReviewDate)
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
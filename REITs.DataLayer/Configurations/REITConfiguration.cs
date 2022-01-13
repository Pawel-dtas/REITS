using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
    public class REITConfiguration : EntityTypeConfiguration<REIT>
    {
        public REITConfiguration()
        {
            HasKey(a => a.Id);
            Property(a => a.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(a => a.ParentId)
                .IsRequired();
            Property(a => a.REITName)
                .HasMaxLength(100)
                .IsRequired();
            Property(a => a.PrincipalUTR)
                .HasMaxLength(10)
                .IsRequired();
            Property(a => a.AccountPeriodEnd)
                .IsRequired();
            Property(a => a.PreviousAccountPeriodEnd)
                .IsRequired();
            Property(a => a.REITNotes)
               .IsOptional();
            Property(a => a.XMLDateSubmitted)
                .IsRequired();
            Property(a => a.XMLVersion)
                .IsRequired();
            Property(a => a.CreatedBy)
                .HasMaxLength(7)
                .IsFixedLength()
                .IsRequired();
            Property(p => p.DateCreated)
                .IsRequired();

            //HasMany(e => e.Entities)
            //    .WithRequired().HasForeignKey(f => f.REITId)
            //    .WillCascadeOnDelete(false);

            //HasMany(e => e.Reconciliations)
            //    .WithRequired().HasForeignKey(f => f.REITId)
            //    .WillCascadeOnDelete(false);

            //HasMany(e => e.REITTotal)
            //    .WithRequired().HasForeignKey(f => f.REITId)
            //    .WillCascadeOnDelete(false);
        }
    }
}
using REITs.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
	public class REITParentConfiguration : EntityTypeConfiguration<REITParent>
	{
		public REITParentConfiguration()
		{
			HasKey(a => a.Id);
			Property(a => a.Id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
			Property(a => a.PrincipalCustomerName)
				.HasMaxLength(100)
				.IsRequired();
			Property(a => a.PrincipalUTR)
				.HasMaxLength(10)
				.IsRequired();

			Property(a => a.TaxExemptUTR)
				.HasMaxLength(10)
				.IsOptional();

			Property(a => a.ConversionDate)
				.IsOptional();
			Property(a => a.APEDate)
				.IsRequired();
			Property(a => a.LastBRRDate)
			   .IsOptional();

			Property(a => a.NewReit)
				.HasMaxLength(5)
			   .IsOptional();

			Property(a => a.NextBRRDate)
				.IsOptional();

			Property(a => a.MarketsListedOn)
				.HasMaxLength(250)
				.IsOptional();

			Property(a => a.MarketCapital)
				.IsOptional();

			Property(a => a.MarketInfo)
				.HasMaxLength(250)
				.IsOptional();

			Property(a => a.CCM)
				.HasMaxLength(7)
				.IsFixedLength()
				.IsOptional();

			Property(a => a.CoOrd)
				.HasMaxLength(7)
				.IsFixedLength()
				.IsOptional();

			Property(a => a.CTG7)
				.HasMaxLength(7)
				.IsFixedLength()
				.IsOptional();

			Property(a => a.CTHO)
				.HasMaxLength(7)
				.IsFixedLength()
				.IsOptional();

			Property(a => a.InformedConsent)
				.HasMaxLength(5)
				.IsOptional();

			Property(a => a.SAO)
				.HasMaxLength(5)
				.IsOptional();

			Property(a => a.SectorsFlag)
				.IsOptional();

			Property(a => a.LeftRegime)
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

			Property(a => a.DateCreated)
				.IsRequired();

			//HasMany(r => r.REITs)
			//    .WithRequired().HasForeignKey(f => f.ParentId)
			//    .WillCascadeOnDelete(false);
		}
	}
}
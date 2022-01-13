using Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace REITs.DataLayer.Configurations
{
	public class REITTotalsConfiguration : EntityTypeConfiguration<REITTotals>
	{
		public REITTotalsConfiguration()
		{
			HasKey(a => a.Id);
			Property(a => a.Id)
				.HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

			//Tests
			Property(a => a.IncomeTestPercentage)
			.IsOptional();
			Property(a => a.IncomeTestResult)
			.HasMaxLength(10)
			.IsRequired();
			Property(a => a.AssetTestPercentage)
			.IsOptional();
			Property(a => a.AssetTestResult)
			.HasMaxLength(10)
			.IsOptional();
			Property(p => p.PIDDistribution90Amount)
			.IsOptional();
			Property(p => p.PIDDistribution100Amount)
			.IsOptional();
			Property(p => p.InterestCoverRatioTestPercentage)
			.IsOptional();
			Property(p => p.InterestCoverRatioTestResult)
			.HasMaxLength(10)
			.IsOptional();

			Property(p => p.PaidDividendScheduleConfirmed)
			.HasMaxLength(10)
			.IsOptional();

			//Property
			Property(p => p.PropCombinedRevenueLessCostOfSales)
			.IsOptional();
			Property(p => p.PropUKRevenueLessCostOfSales)
			.IsOptional();
			Property(p => p.PropNonUKRevenueLessCostOfSales)
			.IsOptional();
			Property(p => p.PropCombinedPIDs)
			.IsOptional();
			Property(p => p.PropUKPIDs)
			.IsOptional();
			Property(p => p.PropNonUKPIDs)
			.IsOptional();
			Property(p => p.PropCombinedOtherIncomeOrExpenseAmount)
			.IsOptional();
			Property(p => p.PropUKOtherIncomeOrExpenseAmount)
			.IsOptional();
			Property(p => p.PropNonUKOtherIncomeOrExpenseAmount)
			.IsOptional();
			Property(p => p.PropCombinedNetFinanceCosts)
			.IsOptional();
			Property(p => p.PropUKNetFinanceCosts)
			.IsOptional();
			Property(p => p.PropNonUKNetFinanceCosts)
			.IsOptional();
			Property(p => p.PropCombinedOtherAdjustmentsAmount)
			.IsOptional();
			Property(p => p.PropUKOtherAdjustmentsAmount)
			.IsOptional();
			Property(p => p.PropNonUKOtherAdjustmentsAmount)
			.IsOptional();
			Property(p => p.PropCombinedNonCurrentAssets)
			.IsOptional();
			Property(p => p.PropUKNonCurrentAssets)
			.IsOptional();
			Property(p => p.PropNonUKNonCurrentAssets)
			.IsOptional();
			Property(p => p.PropCombinedCurrentAssets)
			.IsOptional();
			Property(p => p.PropUKCurrentAssets)
			.IsOptional();
			Property(p => p.PropNonUKCurrentAssets)
			.IsOptional();

			//Residual
			Property(p => p.ResCombinedRevenueLessCostOfSales)
			.IsOptional();
			Property(p => p.ResUKRevenueLessCostOfSales)
			.IsOptional();
			Property(p => p.ResNonUKRevenueLessCostOfSales)
			.IsOptional();
			Property(p => p.ResCombinedBeneficialInterestsIncome)
			.IsOptional();
			Property(p => p.ResUKBeneficialInterestsIncome)
			.IsOptional();
			Property(p => p.ResNonUKBeneficialInterestsIncome)
			.IsOptional();
			Property(p => p.ResCombinedOtherIncomeOrExpenseAmount)
			.IsOptional();
			Property(p => p.ResUKOtherIncomeOrExpenseAmount)
			.IsOptional();
			Property(p => p.ResNonUKOtherIncomeOrExpenseAmount)
			.IsOptional();
			Property(p => p.ResCombinedNetFinanceCosts)
			.IsOptional();
			Property(p => p.ResUKNetFinanceCosts)
			.IsOptional();
			Property(p => p.ResNonUKNetFinanceCosts)
			.IsOptional();
			Property(p => p.ResCombinedOtherAdjustmentsAmount)
			.IsOptional();
			Property(p => p.ResUKOtherAdjustmentsAmount)
			.IsOptional();
			Property(p => p.ResNonUKOtherAdjustmentsAmount)
			.IsOptional();
			Property(p => p.ResCombinedNonCurrentAssets)
			.IsOptional();
			Property(p => p.ResUKNonCurrentAssets)
			.IsOptional();
			Property(p => p.ResNonUKNonCurrentAssets)
			.IsOptional();
			Property(p => p.ResCombinedCurrentAssets)
			.IsOptional();
			Property(p => p.ResUKCurrentAssets)
			.IsOptional();
			Property(p => p.ResNonUKCurrentAssets)
			.IsOptional();

			//Tax Exempt
			Property(p => p.TaxExCombinedPRBIntAndFCsReceivable)
			.IsOptional();
			Property(p => p.TaxExUKPRBIntAndFCsReceivable)
			.IsOptional();
			Property(p => p.TaxExNonUKPRBIntAndFCsReceivable)
			.IsOptional();
			Property(p => p.TaxExCombinedPRBIntAndFCsPayable)
			.IsOptional();
			Property(p => p.TaxExUKPRBIntAndFCsPayable)
			.IsOptional();
			Property(p => p.TaxExNonUKPRBIntAndFCsPayable)
			.IsOptional();
			Property(p => p.TaxExCombinedPRBHedgingDerivatives)
			.IsOptional();
			Property(p => p.TaxExUKPRBHedgingDerivatives)
			.IsOptional();
			Property(p => p.TaxExNonUKPRBHedgingDerivatives)
			.IsOptional();
			Property(p => p.TaxExCombinedPRBResidualIncome)
			.IsOptional();
			Property(p => p.TaxExUKPRBResidualIncome)
			.IsOptional();
			Property(p => p.TaxExNonUKPRBResidualIncome)
			.IsOptional();
			Property(p => p.TaxExCombinedPRBResidualExpenses)
			.IsOptional();
			Property(p => p.TaxExUKPRBResidualExpenses)
			.IsOptional();
			Property(p => p.TaxExNonUKPRBResidualExpenses)
			.IsOptional();
			Property(p => p.TaxExCombinedPBTAdjustments)
			.IsOptional();
			Property(p => p.TaxExUKPBTAdjustments)
			.IsOptional();
			Property(p => p.TaxExNonUKPBTAdjustments)
			.IsOptional();
			Property(p => p.TaxExCombinedUKPRBProfits)
			.IsOptional();
			Property(p => p.TaxExUKUKPRBProfits)
			.IsOptional();
			Property(p => p.TaxExNonUKUKPRBProfits)
			.IsOptional();
			Property(p => p.TaxExCombinedPRBFinanceCosts)
			.IsOptional();
			Property(p => p.TaxExUKPRBFinanceCosts)
			.IsOptional();
			Property(p => p.TaxExNonUKPRBFinanceCosts)
			.IsOptional();
			Property(p => p.TaxExCombinedIntAndFCsReceivable)
			.IsOptional();
			Property(p => p.TaxExUKIntAndFCsReceivable)
			.IsOptional();
			Property(p => p.TaxExNonUKIntAndFCsReceivable)
			.IsOptional();
			Property(p => p.TaxExCombinedIntAndFCsPayable)
			.IsOptional();
			Property(p => p.TaxExUKIntAndFCsPayable)
			.IsOptional();
			Property(p => p.TaxExNonUKIntAndFCsPayable)
			.IsOptional();
			Property(p => p.TaxExCombinedHedgingDerivatives)
			.IsOptional();
			Property(p => p.TaxExUKHedgingDerivatives)
			.IsOptional();
			Property(p => p.TaxExNonUKHedgingDerivatives)
			.IsOptional();
			Property(p => p.TaxExCombinedOtherClaims)
			.IsOptional();
			Property(p => p.TaxExUKOtherClaims)
			.IsOptional();
			Property(p => p.TaxExNonUKOtherClaims)
			.IsOptional();
			Property(p => p.TaxExCombinedCapitalAllowances)
			.IsOptional();
			Property(p => p.TaxExUKCapitalAllowances)
			.IsOptional();
			Property(p => p.TaxExNonUKCapitalAllowances)
			.IsOptional();
			Property(p => p.TaxExCombinedOtherTaxAdjustments)
			.IsOptional();
			Property(p => p.TaxExUKOtherTaxAdjustments)
			.IsOptional();
			Property(p => p.TaxExNonUKOtherTaxAdjustments)
			.IsOptional();
			Property(p => p.TaxExCombinedUKPropertyBroughtFwd)
			.IsOptional();
			Property(p => p.TaxExUKUKPropertyBroughtFwd)
			.IsOptional();
			Property(p => p.TaxExNonUKUKPropertyBroughtFwd)
			.IsOptional();
			Property(p => p.TaxExCombinedProfitsExREITSInvProfits)
			.IsOptional();
			Property(p => p.TaxExUKProfitsExREITSInvProfits)
			.IsOptional();
			Property(p => p.TaxExNonUKProfitsExREITSInvProfits)
			.IsOptional();
			Property(p => p.TaxExCombinedREITSInvProfits)
			.IsOptional();
			Property(p => p.TaxExUKREITSInvProfits)
			.IsOptional();
			Property(p => p.TaxExNonUKREITSInvProfits)
			.IsOptional();

			//Reconciliations
			Property(p => p.PBTReconsToAuditedFinancialStatement)
			.IsOptional();
			Property(p => p.ReconsToAuditedFinancialStatement)
			.IsOptional();

			Property(p => p.REITId)
			.IsRequired();
		}
	}
}
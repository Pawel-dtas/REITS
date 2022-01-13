using System;

namespace Domain.Models
{
	public class REITTotals
	{
		public Guid Id { get; set; }

		//*** Tests ***
		public double IncomeTestPercentage { get; set; }

		public string IncomeTestResult { get; set; }
		public double AssetTestPercentage { get; set; }
		public string AssetTestResult { get; set; }
		public double PIDDistribution90Amount { get; set; }
		public double PIDDistribution100Amount { get; set; }
		public double InterestCoverRatioTestPercentage { get; set; }
		public string InterestCoverRatioTestResult { get; set; }

		public string PaidDividendScheduleConfirmed { get; set; }

		//*** Property Totals ***
		public double PropCombinedRevenueLessCostOfSales { get; set; }

		public double PropUKRevenueLessCostOfSales { get; set; }
		public double PropNonUKRevenueLessCostOfSales { get; set; }

		public double PropCombinedPIDs { get; set; }
		public double PropUKPIDs { get; set; }
		public double PropNonUKPIDs { get; set; }

		public double PropCombinedOtherIncomeOrExpenseAmount { get; set; }
		public double PropUKOtherIncomeOrExpenseAmount { get; set; }
		public double PropNonUKOtherIncomeOrExpenseAmount { get; set; }

		public double PropCombinedNetFinanceCosts { get; set; }
		public double PropUKNetFinanceCosts { get; set; }
		public double PropNonUKNetFinanceCosts { get; set; }

		public double PropCombinedOtherAdjustmentsAmount { get; set; }
		public double PropUKOtherAdjustmentsAmount { get; set; }
		public double PropNonUKOtherAdjustmentsAmount { get; set; }

		public double PropCombinedNonCurrentAssets { get; set; }
		public double PropUKNonCurrentAssets { get; set; }
		public double PropNonUKNonCurrentAssets { get; set; }

		public double PropCombinedCurrentAssets { get; set; }
		public double PropUKCurrentAssets { get; set; }
		public double PropNonUKCurrentAssets { get; set; }

		//*** Residual Totals ***
		public double ResCombinedRevenueLessCostOfSales { get; set; }

		public double ResUKRevenueLessCostOfSales { get; set; }
		public double ResNonUKRevenueLessCostOfSales { get; set; }

		public double ResCombinedBeneficialInterestsIncome { get; set; }
		public double ResUKBeneficialInterestsIncome { get; set; }
		public double ResNonUKBeneficialInterestsIncome { get; set; }

		public double ResCombinedOtherIncomeOrExpenseAmount { get; set; }
		public double ResUKOtherIncomeOrExpenseAmount { get; set; }
		public double ResNonUKOtherIncomeOrExpenseAmount { get; set; }

		public double ResCombinedNetFinanceCosts { get; set; }
		public double ResUKNetFinanceCosts { get; set; }
		public double ResNonUKNetFinanceCosts { get; set; }

		public double ResCombinedOtherAdjustmentsAmount { get; set; }
		public double ResUKOtherAdjustmentsAmount { get; set; }
		public double ResNonUKOtherAdjustmentsAmount { get; set; }

		public double ResCombinedNonCurrentAssets { get; set; }
		public double ResUKNonCurrentAssets { get; set; }
		public double ResNonUKNonCurrentAssets { get; set; }

		public double ResCombinedCurrentAssets { get; set; }
		public double ResUKCurrentAssets { get; set; }
		public double ResNonUKCurrentAssets { get; set; }

		//*** Tax Exempt Totals ***

		public double TaxExCombinedPRBProfitsBeforeTax { get; set; }
		public double TaxExUKPRBProfitsBeforeTax { get; set; }
		public double TaxExNonUKPRBProfitsBeforeTax { get; set; }

		public double TaxExCombinedPRBIntAndFCsReceivable { get; set; }
		public double TaxExUKPRBIntAndFCsReceivable { get; set; }
		public double TaxExNonUKPRBIntAndFCsReceivable { get; set; }

		public double TaxExCombinedPRBIntAndFCsPayable { get; set; }
		public double TaxExUKPRBIntAndFCsPayable { get; set; }
		public double TaxExNonUKPRBIntAndFCsPayable { get; set; }

		public double TaxExCombinedPRBHedgingDerivatives { get; set; }
		public double TaxExUKPRBHedgingDerivatives { get; set; }
		public double TaxExNonUKPRBHedgingDerivatives { get; set; }

		public double TaxExCombinedPRBResidualIncome { get; set; }
		public double TaxExUKPRBResidualIncome { get; set; }
		public double TaxExNonUKPRBResidualIncome { get; set; }

		public double TaxExCombinedPRBResidualExpenses { get; set; }
		public double TaxExUKPRBResidualExpenses { get; set; }
		public double TaxExNonUKPRBResidualExpenses { get; set; }

		public double TaxExCombinedPBTAdjustments { get; set; }
		public double TaxExUKPBTAdjustments { get; set; }
		public double TaxExNonUKPBTAdjustments { get; set; }

		public double TaxExCombinedUKPRBProfits { get; set; }
		public double TaxExUKUKPRBProfits { get; set; }
		public double TaxExNonUKUKPRBProfits { get; set; }

		public double TaxExCombinedPRBFinanceCosts { get; set; }
		public double TaxExUKPRBFinanceCosts { get; set; }
		public double TaxExNonUKPRBFinanceCosts { get; set; }

		public double TaxExCombinedIntAndFCsReceivable { get; set; }
		public double TaxExUKIntAndFCsReceivable { get; set; }
		public double TaxExNonUKIntAndFCsReceivable { get; set; }

		public double TaxExCombinedIntAndFCsPayable { get; set; }
		public double TaxExUKIntAndFCsPayable { get; set; }
		public double TaxExNonUKIntAndFCsPayable { get; set; }

		public double TaxExCombinedHedgingDerivatives { get; set; }
		public double TaxExUKHedgingDerivatives { get; set; }
		public double TaxExNonUKHedgingDerivatives { get; set; }

		public double TaxExCombinedOtherClaims { get; set; }
		public double TaxExUKOtherClaims { get; set; }
		public double TaxExNonUKOtherClaims { get; set; }

		public double TaxExCombinedCapitalAllowances { get; set; }
		public double TaxExUKCapitalAllowances { get; set; }
		public double TaxExNonUKCapitalAllowances { get; set; }

		public double TaxExCombinedOtherTaxAdjustments { get; set; }
		public double TaxExUKOtherTaxAdjustments { get; set; }
		public double TaxExNonUKOtherTaxAdjustments { get; set; }

		public double TaxExCombinedUKPropertyBroughtFwd { get; set; }
		public double TaxExUKUKPropertyBroughtFwd { get; set; }
		public double TaxExNonUKUKPropertyBroughtFwd { get; set; }

		public double TaxExCombinedProfitsExREITSInvProfits { get; set; }
		public double TaxExUKProfitsExREITSInvProfits { get; set; }
		public double TaxExNonUKProfitsExREITSInvProfits { get; set; }

		public double TaxExCombinedREITSInvProfits { get; set; }
		public double TaxExUKREITSInvProfits { get; set; }
		public double TaxExNonUKREITSInvProfits { get; set; }

		//*** Reconciliation Totals ***
		public double PBTReconsToAuditedFinancialStatement { get; set; }

		public double ReconsToAuditedFinancialStatement { get; set; }

		public Guid REITId { get; set; }

		//public virtual REIT REIT { get; set; } //It's parent REIT
	}
}
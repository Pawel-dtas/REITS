using Domain.Models;
using REITs.Domain.Models;
using System.Data.Entity.Migrations;

namespace REITs.DataLayer.Seeds
{
	public static class REITSeed
	{
		private static REIT seededREIT;

		private static REITTotals seededREITTotals;

		private static Reconciliation seededReconciliationFirst;
		private static Reconciliation seededReconciliationSecond;

		private static Entity seededEntityOne;
		private static Entity seededEntityTwo;

		private static Adjustment seededAdjustmentOneFirst;
		private static Adjustment seededAdjustmentOneSecond;
		private static Adjustment seededAdjustmentTwoFirst;
		private static Adjustment seededAdjustmentTwoSecond;

		public static void Seed(Contexts.ApplicationContext context)
		{
			//*** REIT ***
			seededREIT = new REIT
			{
				REITName = "TestType",
				PrincipalUTR = "1111111111",
				AccountPeriodEnd = System.Convert.ToDateTime("31/03/2018"),
				PreviousAccountPeriodEnd = System.Convert.ToDateTime("31/03/2017"),
				REITNotes = "Test Notes",
				CreatedBy = "5305136",
				DateCreated = System.Convert.ToDateTime("10/08/2018")
			};
			context.REITs.AddOrUpdate(seededREIT);
			context.SaveChanges();

			//*** REITTotals ***
			seededREITTotals = new REITTotals
			{
				//*** Tests ***
				IncomeTestPercentage = 100,
				IncomeTestResult = "Pass",
				AssetTestPercentage = 100,
				AssetTestResult = "Pass",
				PIDDistribution90Amount = 100,
				PIDDistribution100Amount = 100,
				InterestCoverRatioTestPercentage = 100,
				InterestCoverRatioTestResult = "Pass",

				//*** Property Totals ***
				PropCombinedRevenueLessCostOfSales = 100,
				PropUKRevenueLessCostOfSales = 100,
				PropNonUKRevenueLessCostOfSales = 100,
				PropCombinedPIDs = 100,
				PropUKPIDs = 100,
				PropNonUKPIDs = 100,
				PropCombinedOtherIncomeOrExpenseAmount = 100,
				PropUKOtherIncomeOrExpenseAmount = 100,
				PropNonUKOtherIncomeOrExpenseAmount = 100,
				PropCombinedNetFinanceCosts = 100,
				PropUKNetFinanceCosts = 100,
				PropNonUKNetFinanceCosts = 100,
				PropCombinedOtherAdjustmentsAmount = 100,
				PropUKOtherAdjustmentsAmount = 100,
				PropNonUKOtherAdjustmentsAmount = 100,
				PropCombinedNonCurrentAssets = 100,
				PropUKNonCurrentAssets = 100,
				PropNonUKNonCurrentAssets = 100,
				PropCombinedCurrentAssets = 100,
				PropUKCurrentAssets = 100,
				PropNonUKCurrentAssets = 100,

				//*** Residual Totals ***
				ResCombinedRevenueLessCostOfSales = 100,
				ResUKRevenueLessCostOfSales = 100,
				ResNonUKRevenueLessCostOfSales = 100,
				ResCombinedBeneficialInterestsIncome = 100,
				ResUKBeneficialInterestsIncome = 100,
				ResNonUKBeneficialInterestsIncome = 100,
				ResCombinedOtherIncomeOrExpenseAmount = 100,
				ResUKOtherIncomeOrExpenseAmount = 100,
				ResNonUKOtherIncomeOrExpenseAmount = 100,
				ResCombinedNetFinanceCosts = 100,
				ResUKNetFinanceCosts = 100,
				ResNonUKNetFinanceCosts = 100,
				ResCombinedOtherAdjustmentsAmount = 100,
				ResUKOtherAdjustmentsAmount = 100,
				ResNonUKOtherAdjustmentsAmount = 100,
				ResCombinedNonCurrentAssets = 100,
				ResUKNonCurrentAssets = 100,
				ResNonUKNonCurrentAssets = 100,
				ResCombinedCurrentAssets = 100,
				ResUKCurrentAssets = 100,
				ResNonUKCurrentAssets = 100,

				//*** Tax Exempt Totals ***
				TaxExCombinedPRBIntAndFCsReceivable = 100,
				TaxExUKPRBIntAndFCsReceivable = 100,
				TaxExNonUKPRBIntAndFCsReceivable = 100,
				TaxExCombinedPRBIntAndFCsPayable = 100,
				TaxExUKPRBIntAndFCsPayable = 100,
				TaxExNonUKPRBIntAndFCsPayable = 100,
				TaxExCombinedPRBHedgingDerivatives = 100,
				TaxExUKPRBHedgingDerivatives = 100,
				TaxExNonUKPRBHedgingDerivatives = 100,
				TaxExCombinedPRBResidualIncome = 100,
				TaxExUKPRBResidualIncome = 100,
				TaxExNonUKPRBResidualIncome = 100,
				TaxExCombinedPRBResidualExpenses = 100,
				TaxExUKPRBResidualExpenses = 100,
				TaxExNonUKPRBResidualExpenses = 100,
				TaxExCombinedPBTAdjustments = 100,
				TaxExUKPBTAdjustments = 100,
				TaxExNonUKPBTAdjustments = 100,
				TaxExCombinedUKPRBProfits = 100,
				TaxExUKUKPRBProfits = 100,
				TaxExNonUKUKPRBProfits = 100,
				TaxExCombinedPRBFinanceCosts = 100,
				TaxExUKPRBFinanceCosts = 100,
				TaxExNonUKPRBFinanceCosts = 100,
				TaxExCombinedIntAndFCsReceivable = 100,
				TaxExUKIntAndFCsReceivable = 100,
				TaxExNonUKIntAndFCsReceivable = 100,
				TaxExCombinedIntAndFCsPayable = 100,
				TaxExUKIntAndFCsPayable = 100,
				TaxExNonUKIntAndFCsPayable = 100,
				TaxExCombinedHedgingDerivatives = 100,
				TaxExUKHedgingDerivatives = 100,
				TaxExNonUKHedgingDerivatives = 100,
				TaxExCombinedOtherClaims = 100,
				TaxExUKOtherClaims = 100,
				TaxExNonUKOtherClaims = 100,
				TaxExCombinedCapitalAllowances = 100,
				TaxExUKCapitalAllowances = 100,
				TaxExNonUKCapitalAllowances = 100,
				TaxExCombinedOtherTaxAdjustments = 100,
				TaxExUKOtherTaxAdjustments = 100,
				TaxExNonUKOtherTaxAdjustments = 100,
				TaxExCombinedUKPropertyBroughtFwd = 100,
				TaxExUKUKPropertyBroughtFwd = 100,
				TaxExNonUKUKPropertyBroughtFwd = 100,
				TaxExCombinedProfitsExREITSInvProfits = 100,
				TaxExUKProfitsExREITSInvProfits = 100,
				TaxExNonUKProfitsExREITSInvProfits = 100,
				TaxExCombinedREITSInvProfits = 100,
				TaxExUKREITSInvProfits = 100,
				TaxExNonUKREITSInvProfits = 100,

				//*** Reconciliation ***
				PBTReconsToAuditedFinancialStatement = 100,
				ReconsToAuditedFinancialStatement = 100,

				REITId = seededREIT.Id,
			};
			context.REITTotals.Add(seededREITTotals);
			context.SaveChanges();

			//*** Entities ***
			seededEntityOne = new Entity
			{
				EntityName = "One Ltd",
				EntityType = "TestType",
				EntityUTR = "1111111111",
				InterestPercentage = 60,
				Jurisdiction = "United Kingdom",
				TaxExempt = true,
				CustomerReference = "Custome001",

				REITId = seededREIT.Id,
			};
			seededREIT.Entities.Add(seededEntityOne);
			context.SaveChanges();

			seededEntityTwo = new Entity
			{
				EntityName = "Two Ltd",
				EntityType = "TestType",
				EntityUTR = "2222222222",
				InterestPercentage = 20,
				Jurisdiction = "United Kingdom",
				TaxExempt = true,
				CustomerReference = "Custome001",

				REITId = seededREIT.Id,
			};
			seededREIT.Entities.Add(seededEntityTwo);
			context.SaveChanges();

			//***Adjustments * **
			//***Entity One***
			seededAdjustmentOneFirst = new Adjustment
			{
				AdjustmentCategory = "Property Rental Business",
				AdjustmentType = "Profits/Expenses",
				AdjustmentName = "Revenue Less Cost of Sales",
				AdjustmentAmount = 500,

				EntityId = seededEntityOne.Id,
			};
			seededEntityOne.Adjustments.Add(seededAdjustmentOneFirst);
			context.SaveChanges();

			seededAdjustmentOneSecond = new Adjustment
			{
				AdjustmentCategory = "Property Rental Business",
				AdjustmentType = "Profits/Expenses",
				AdjustmentName = "Revenue Less Cost of Sales",
				AdjustmentAmount = 500,

				EntityId = seededEntityOne.Id,
			};
			seededEntityOne.Adjustments.Add(seededAdjustmentOneSecond);
			context.SaveChanges();

			//***Entity Two***
			seededAdjustmentTwoFirst = new Adjustment
			{
				AdjustmentCategory = "Property",
				AdjustmentType = "TestInputType",
				AdjustmentName = "1st Adjustment to Entity Two",
				AdjustmentAmount = 500,

				EntityId = seededEntityTwo.Id,
			};
			seededEntityTwo.Adjustments.Add(seededAdjustmentTwoFirst);
			context.SaveChanges();

			seededAdjustmentTwoSecond = new Adjustment
			{
				AdjustmentCategory = "Property",
				AdjustmentType = "TestInputType",
				AdjustmentName = "2nd Adjustment to Entity Two",
				AdjustmentAmount = 500,

				EntityId = seededEntityTwo.Id,
			};
			seededEntityTwo.Adjustments.Add(seededAdjustmentTwoSecond);
			context.SaveChanges();

			//*** Reconciliations ***
			seededReconciliationFirst = new Reconciliation
			{
				ReconciliationType = "Dropdown1",
				ReconciliationName = "TestName1",
				ReconciliationAmount = 111,

				REITId = seededREIT.Id,
			};
			seededREIT.Reconciliations.Add(seededReconciliationFirst);
			context.SaveChanges();

			seededReconciliationSecond = new Reconciliation
			{
				ReconciliationType = "Dropdown2",
				ReconciliationName = "TestName2",
				ReconciliationAmount = 222,

				REITId = seededREIT.Id,
			};
			seededREIT.Reconciliations.Add(seededReconciliationSecond);
			context.SaveChanges();
		}
	}
}
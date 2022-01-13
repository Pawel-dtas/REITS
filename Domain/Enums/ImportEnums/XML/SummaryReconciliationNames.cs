using System.ComponentModel;

namespace Domain.Enums
{
	public enum SummaryReconciliationNames
	{
		[Description("Consolidation Adjustments")]
		ConsolidationAdjustments,

		[Description("Distributions Or Dividends")]
		DistributionsOrDividends,

		[Description("Impairment Or Amortisation")]
		ImpairmentOrAmortisation,

		[Description("Profits Of Associated Entities")]
		ProfitsOfAssociatedEntities,

		[Description("Realised Gains Or Losses On Property")]
		RealisedGainsOrLossesOnProperty,

		[Description("Revaluation")]
		Revaluation,

		[Description("Unrealised Gains Or Losses On Property")]
		UnrealisedGainsOrLossesOnProperty,

		[Description("Other Reconciliation")]
		OtherRreconciliation
	}
}
using System.ComponentModel;

namespace Domain.Enums
{
	public enum TaxExemptOtherAdjustmentNames
	{
		[Description("Balancing Allowances")]
		BalancingAllowances,

		[Description("Balancing Charges")]
		BalancingCharges,

		[Description("Capitalised Interest")]
		CapitalisedInterest,

		[Description("CIR Allocated Disallowance")]
		CIRAllocatedDisallowance,

		[Description("Fair Value Movement On Derivatives")]
		FairValueMovementOnDerivatives,

		[Description("Land Remediation")]
		LandRemediation,

		[Description("Non-Trade Loan Relationships")]
		NonTradeLoanRelationships,

		[Description("Transfer Pricing Adjustment")]
		TransferPricingAdjustment,

		[Description("Other Tax Adjustment")]
		OtherTaxAdjustment
	}
}
using System.ComponentModel;

namespace Domain.Enums
{
	public enum PropertyAdjustmentNames
	{
		[Description("Revenue Less Cost Of Sales")]
		RevenueLessCostOfSales,

		[Description("PIDs From Other UK REITs")]
		PIDsFromOtherUKREITs,

		[Description("Net Finance Costs External")]
		NetFinanceCostsExternal,

		[Description("Non-Current Assets")]
		NonCurrentAssets,

		[Description("Current Assets")]
		CurrentAssets
	}
}
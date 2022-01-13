using System.ComponentModel;

namespace Domain.Enums
{
    public enum ResidualAdjustmentNames
    {
        [Description("Revenue Less Cost Of Sales")]
        RevenueLessCostOfSales,
        [Description("Beneficial Interests Income")]
        BeneficialInterestsIncome,
        [Description("Net Finance Costs Residual")]
        NetFinanceCostsResidual,
        [Description("Non-Current Assets")]
        NonCurrentAssets,
        [Description("Current Assets")]
        CurrentAssets
    }
}

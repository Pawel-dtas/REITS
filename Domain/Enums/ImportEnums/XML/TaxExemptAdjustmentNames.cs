using System.ComponentModel;

namespace Domain.Enums
{
    public enum TaxExemptAdjustmentNames
    {
        [Description("PRB-Profit Before Tax")]
        PRBProfitsBeforeTax,        
        [Description("PRB-Interest And Other Financial Costs Receivable")]
        PRBCostsReceivable,
        [Description("PRB-Interest And Other Financial Costs Payable")]
        PRBCostsPayable,
        [Description("PRB-Fair Value Movement Of Hedging Derivatives")]
        PRBFairMovementOfHedgingDerivatives,
        [Description("Residual Income")]
        ResidualIncome,
        [Description("Residual Expenses")]
        ResidualExpense,
        [Description("Interest And Other Financial Costs Receivable (Including Related Transfer Pricing Adjustments)")]
        CostsReceivable,
        [Description("Interest And Other Financial Costs Payable (Including Related Transfer Pricing Adjustments)")]
        CostsPayable,
        [Description("Fair Value Movement Of Hedging Derivatives")]
        FairMovementOfHedgingDerivatives,
        [Description("Capital Allowances - In Full")]
        CapitalAllowancesInFull,
        [Description("Less UK Property Losses Brought Forward")]
        PropertyLossesBroughtForward

    }
}

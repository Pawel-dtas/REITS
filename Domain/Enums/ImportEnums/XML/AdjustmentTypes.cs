using System.ComponentModel;
namespace Domain.Enums
{
    public enum AdjustmentTypes
    {
        [Description("Property Rental Business IAS - Profits/Expenses")]
        PropertyRentalBusinessIASProfitsExpenses,
        [Description("Property Rental Business Other Income Or Expense")]
        PropertyRentalBusinessOtherIncomeOrExpense,
        [Description("Property Rental Business Net Finance Costs External")]
        PropertyRentalBusinessNetFinanceCosts,
        [Description("Property Rental Business Other Adjustments")]
        PropertyRentalBusinessOtherAdjustments,
        [Description("Property Rental Business IAS - Assets")]
        PropertyRentalBusinessIASAssets,
        [Description("Residual Income IAS - Profits/Expenses")]
        ResidualIncomeIASProfitsExpenses,
        [Description("Residual Income Other Income Or Expense")]
        ResidualIncomeOtherIncomeOrExpense,
        [Description("Residual Income Net Finance Costs Residual")]
        ResidualIncomeNetFinanceCosts,
        [Description("Residual Income Other Adjustments")]
        ResidualIncomeOtherAdjustments,
        [Description("Residual Income IAS - Assets")]
        ResidualIncome_IAS_Assets,
        [Description("Tax Exempt UK PRB Profits")]
        TaxExemptUKPRBProfits,
        [Description("Tax Exempt Adjustments To Exclude Other Tax-Disallowable Expenditure In Accounting PBT")]
        TaxExemptAdjustmentsToExcludeOtherTaxDisallowableExpenditureInAccountingPBT,
        [Description("Tax Exempt Tax-Exempt Profits")]
        TaxExemptTaxExemptProfits,
        [Description("Tax Exempt Other Tax Adjustments")]
        TaxExemptOtherTaxAdjustments

    }
}

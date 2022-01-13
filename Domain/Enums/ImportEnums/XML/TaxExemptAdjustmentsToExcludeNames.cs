using System.ComponentModel;

namespace Domain.Enums
{
    public enum TaxExemptAdjustmentsToExcludeNames
    {
        [Description("Admin Expenses")]
        AdminExpenses,
        [Description("Abortive Capital Expenditure")]
        AbortiveCapitalExpenditure,
        [Description("Admin Recharge")]
        AdminRecharge,
        [Description("Associated Costs")]
        AssociatedCosts,
        [Description("Capitalised Revenue Expenditure")]
        CapitalisedRevenueExpenditure,
        [Description("Corporate Entertaining")]
        CorporateEntertaining,
        [Description("Depreciation")]
        Depreciation,
        [Description("Legal And Letting Fees")]
        LegalAndLettingFees,
        [Description("P/L On Disposal Of Assets")]
        PLOnDisposalOfAssets,
        [Description("Revaluation Movements")]
        RevaluationMovements,
        [Description("Other Capital Income/Expense")]
        OtherCapitalIncomeExpense,
        [Description("Other Tax Disallowable Item")]
        OtherTaxDisallowableItem
    }
}

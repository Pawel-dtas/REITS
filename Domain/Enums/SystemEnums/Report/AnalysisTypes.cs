namespace Domain.Enums
{
    using System.ComponentModel;

    public enum AnalysisAdjustmentType
    {
        [Description("")]
        NotSet,

        [Description("IAS Profits/Expenses")]
        IASProfitsExpenses,

        [Description("Other Income Or Expense")]
        OtherIncomeOrExpense,

        [Description("Net Finance Costs External")]
        NetFinanceCostsExternal,

        [Description("Other Adjustments")]
        OtherAdjustments,

        [Description("IAS - Assets")]
        IASAssets
    }
}
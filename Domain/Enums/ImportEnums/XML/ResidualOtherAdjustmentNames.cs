using System.ComponentModel;

namespace Domain.Enums
{
    public enum ResidualOtherAdjustments
    {
        [Description("Allocation Of Interest To PRB")]
        AllocationOofInterestToPRB,
        [Description("Amount Treated As Residual Under S543(4) – Profit Financing Cost Ratio")]
        AmountTreatedUnderS543,
        [Description("Amount Treated As Residual Under S551(5) – Distribution To Holder Of Excessive Rights")]
        AmountTreatedUnderS551,
        [Description("Amount Treated As Residual Under S564(3) – Breach Of Distribution Condition")]
        AmountTreatedUnderS564,
        [Description("Change In Fair Value Of Derivative Instruments")]
        ChangeInFairValueOfDerivativeInstruments,
        [Description("Income Reallocation From PRB")]
        IncomeReallocationFromPRB,
        [Description("Outside The Ordinary Course Of Business")]
        OutsideTheOrdinaryCourseOfBusiness,
        [Description("Realised Gains Or Losses On Property")]
        RealisedGainsOnLossesOnProperty,
        [Description("Reallocation From PRB")]
        ReallocationFromPRB,
        [Description("Unrealised Gains Or Losses On Property")]
        UnrealisedGainsOrLossesOnProperty,
        [Description("Other Comprehensive Income/Loss")]
        OtherComprehensiveIncomeLoss,
        //added by Pawel
        [Description("S106 Obligations")]
        S106Obligations
    }
}









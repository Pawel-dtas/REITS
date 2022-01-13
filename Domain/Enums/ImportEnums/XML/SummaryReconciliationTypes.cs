using System.ComponentModel;

namespace Domain.Enums
{
    public enum SummaryReconciliationTypes
    {
        [Description("PBT Reconciliation")]
        PBTReconciliation,
        [Description("Asset Reconciliation")]
        AssetReconciliation
    }
}

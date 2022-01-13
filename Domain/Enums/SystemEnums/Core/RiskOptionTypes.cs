using System.ComponentModel;

namespace REITs.Domain.Enums
{
    public enum RiskOptionTypes
    {
        [Description("")]
        NotSet,

        [Description("Low")]
        Low,

        [Description("Non Low")]
        NonLow
    }
}
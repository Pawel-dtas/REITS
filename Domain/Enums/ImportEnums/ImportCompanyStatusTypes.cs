using System.ComponentModel;

namespace REITs.Domain.Enums
{
    public enum ImportCompanyStatusTypes
    {
        [Description("")]
        NotSet,

        [Description("Exists")]
        Exists,

        [Description("No Match")]
        NoMatch,

        [Description("Created")]
        Created,

        [Description("Error")]
        Error
    }
}
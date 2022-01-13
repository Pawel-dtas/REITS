using System.ComponentModel;

namespace Domain.Enums
{
    public enum SearchTypes
    {
        [Description("REITParent")]
        REITParent,

        [Description("REIT")]
        REIT,

        [Description("Entity")]
        Entity
    }
}
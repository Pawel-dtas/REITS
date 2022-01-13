using System.ComponentModel;

namespace REITs.Domain.Enums
{
    public enum AccessLevels
    {
        [Description("Admin")]
        Admin,

        [Description("User")]
        User,

        [Description("Viewer")]
        Viewer
    }
}
using System.ComponentModel;

namespace REITs.Domain.Enums
{
    public enum ImportXMLStatusTypes
    {
        [Description("")]
        NotSet,

        [Description("Ready")]
        Ready,

        [Description("Validated")]
        Validated,

        [Description("Validated, Already Exists, Will be Versioned")]
        ValidatedAndExists,

        [Description("Validated, Already Exists, ExactMatch, Won't Import")]
        ValidatedAndExistsExactMatch,

        [Description("Errors")]
        Errors,

        [Description("Errors, Already Exists, ExactMatch")]
        ErrorsAndExists,

        [Description("Imported OK")]
        ImportedOK,

        [Description("Import Failed")]
        ImportError
    }
}
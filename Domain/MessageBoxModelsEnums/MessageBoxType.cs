using System.ComponentModel;

namespace Domain.MessageBoxModelsEnums
{
    public enum MessageBoxType
    {
        [Description("Ok")]
        Ok,
        [Description("Ok Cancel")]
        OkCancel,
        [Description("Yes No")]
        YesNo
    }
}

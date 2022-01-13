using System.ComponentModel;

namespace Domain.MessageBoxModelsEnums
{
	public enum MessageBoxButtonType
	{
		[Description("OK")]
		OK,

		[Description("OKCancel")]
		OKCancel,

		[Description("YesNo")]
		YesNo
	}
}
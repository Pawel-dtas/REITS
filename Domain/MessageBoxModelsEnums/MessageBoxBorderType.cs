using System.ComponentModel;

namespace Domain.MessageBoxModelsEnums
{
	public enum MessageBoxBorderType
	{
		[Description("DarkGreen")]
		SuccessBorder,

		[Description("Goldenrod")]
		ExclamationBorder,

		[Description("Darkred")]
		ErrorBorder,

		[Description("DarkBlue")]
		QuestionBorder
	}
}
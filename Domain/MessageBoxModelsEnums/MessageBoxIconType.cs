using System.ComponentModel;

namespace Domain.MessageBoxModelsEnums
{
	public enum MessageBoxIconType
	{
		[Description("/REITs.Infrastructure;component/Resources/SystemMessageIconSuccess.png")]
		SuccessIcon,

		[Description("/REITs.Infrastructure;component/Resources/SystemMessageIconExclamation.png")]
		ExclamationIcon,

		[Description("/REITs.Infrastructure;component/Resources/SystemMessageIconError.png")]
		ErrorIcon,

		[Description("/REITs.Infrastructure;component/Resources/SystemMessageIconQuestion.png")]
		QuestionIcon
	}
}
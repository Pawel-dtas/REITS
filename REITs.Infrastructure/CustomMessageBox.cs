using Domain;
using Domain.MessageBoxModelsEnums;
//using Hmrc.BDApp.WorkFlow.Enums;
using REITs.Infrastructure.ViewModels;
using REITs.Infrastructure.Views;
using System.Windows;

namespace REITs.Infrastructure
{
	public static class CustomMessageBox
	{
		public static MessageBoxResult Show(MessageBoxContentTypes mbContentType)
		{
			var messageDetails = MessageContent.GetMessageContent(mbContentType);

			var messageBox = new SystemMessageBoxViewModel
			{
				MessageContent = messageDetails.Content,
				MessageTitle = messageDetails.Title,
				MessageBorder = messageDetails.MessageBorder.GetDescriptionFromEnum(),
				MessageIcon = messageDetails.MessageIcon.GetDescriptionFromEnum(),
				MessageType = messageDetails.MessageType,
				MessageButtons = messageDetails.MessageButtons
			};

			var messageBoxview = new SystemMessageBoxView { DataContext = messageBox };

			messageBoxview.ShowDialog();

			return messageBox.SelectedOption;
		}
	}
}
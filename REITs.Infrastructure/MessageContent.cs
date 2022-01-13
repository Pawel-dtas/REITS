using Domain;
using Domain.MessageBoxModelsEnums;

namespace REITs.Infrastructure
{ }

public static class MessageContent
{
	#region Public Methods

	public static MessageBoxDetails GetMessageContent(MessageBoxContentTypes Option)
	{
		MessageBoxDetails messageDetails = new MessageBoxDetails();

		switch (Option)
		{
			case MessageBoxContentTypes.DeleteConfirmation:
				{
					messageDetails.MessageType = MessageBoxType.YesNo;
					messageDetails.Title = "Delete";
					messageDetails.Content = "Are you sure you wish to delete this record?";
					messageDetails.MessageIcon = MessageBoxIconType.QuestionIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.QuestionBorder;
				}
				break;

			case MessageBoxContentTypes.DeleteSuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Delete";
					messageDetails.Content = "Record deleted.";
					messageDetails.MessageIcon = MessageBoxIconType.SuccessIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.SuccessBorder;
				}
				break;

			case MessageBoxContentTypes.DeleteUnsuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Delete";
					messageDetails.Content = "There was a problem deleting the record.";
					messageDetails.MessageIcon = MessageBoxIconType.ErrorIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ErrorBorder;
				}
				break;

			case MessageBoxContentTypes.DeleteUserConfirmation:
				{
					messageDetails.MessageType = MessageBoxType.YesNo;
					messageDetails.Title = "Delete";
					messageDetails.Content = "Are you sure you wish to delete this User?";
					messageDetails.MessageIcon = MessageBoxIconType.QuestionIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.QuestionBorder;
				}
				break;

			case MessageBoxContentTypes.DeleteUserSuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Delete";
					messageDetails.Content = "User deleted.";
					messageDetails.MessageIcon = MessageBoxIconType.SuccessIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.SuccessBorder;
				}
				break;

			case MessageBoxContentTypes.DeleteUserUnsuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Delete";
					messageDetails.Content = "There was a problem deleting the selected user.";
					messageDetails.MessageIcon = MessageBoxIconType.ErrorIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ErrorBorder;
				}
				break;

			case MessageBoxContentTypes.EditUserUnsuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Edit User";
					messageDetails.Content = "There was a problem loading the selected user.";
					messageDetails.MessageIcon = MessageBoxIconType.ErrorIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ErrorBorder;
				}
				break;

			case MessageBoxContentTypes.EditUserSuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Edit User";
					messageDetails.Content = "User successfully updated.";
					messageDetails.MessageIcon = MessageBoxIconType.SuccessIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.SuccessBorder;
				}
				break;

			case MessageBoxContentTypes.InvalidNetworkPath:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Network Location";
					messageDetails.Content = "The selected network location does not exist.";
					messageDetails.MessageIcon = MessageBoxIconType.ExclamationIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ExclamationBorder;
				}
				break;

			case MessageBoxContentTypes.SaveUserSuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "User";
					messageDetails.Content = "User successfully saved.";
					messageDetails.MessageIcon = MessageBoxIconType.SuccessIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.SuccessBorder;
				}
				break;

			case MessageBoxContentTypes.SaveUserUnsuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "User";
					messageDetails.Content = "There was a problem saving the User.";
					messageDetails.MessageIcon = MessageBoxIconType.ExclamationIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ExclamationBorder;
				}
				break;

			case MessageBoxContentTypes.UserAlreadyExists:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "User";
					messageDetails.Content = "The user you are trying to add already exists.";
					messageDetails.MessageIcon = MessageBoxIconType.ExclamationIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ExclamationBorder;
				}
				break;

			case MessageBoxContentTypes.ImportUnsuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Import REIT Template";
					messageDetails.Content = "There was a problem importing the selected REITs Templates.";
					messageDetails.MessageIcon = MessageBoxIconType.ExclamationIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ExclamationBorder;
				}
				break;

			case MessageBoxContentTypes.ImportSuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Import REIT Template";
					messageDetails.Content = "The selected REITs Templates were successfully imported.";
					messageDetails.MessageIcon = MessageBoxIconType.SuccessIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.SuccessBorder;
				}
				break;

			case MessageBoxContentTypes.NoSearchResults:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Search";
					messageDetails.Content = "No results to display.";
					messageDetails.MessageIcon = MessageBoxIconType.ExclamationIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ExclamationBorder;
				}
				break;

			case MessageBoxContentTypes.SearchOptionsResetWarning:
				{
					messageDetails.MessageType = MessageBoxType.YesNo;
					messageDetails.Title = "Search Type";
					messageDetails.Content = "Changing the Search Type will clear any search fields that have been completed. Do you wish to proceed?";
					messageDetails.MessageIcon = MessageBoxIconType.QuestionIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.QuestionBorder;
				}
				break;

			case MessageBoxContentTypes.NoCustomersSelected:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "No Customer Selected";
					messageDetails.Content = "Filter drop-downs could not be populated, as no customer has been selected.";
					messageDetails.MessageIcon = MessageBoxIconType.ExclamationIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.ExclamationBorder;
				}
				break;

			case MessageBoxContentTypes.DeleteFSRecordCheck:
				{
					messageDetails.MessageType = MessageBoxType.YesNo;
					messageDetails.Title = "Confirm Record Deletion";
					messageDetails.Content = "Are you sure you wish to delete the selected record?";
					messageDetails.MessageIcon = MessageBoxIconType.QuestionIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.QuestionBorder;
				}
				break;

			case MessageBoxContentTypes.CopyToClipboard:
				{
					messageDetails.MessageType = MessageBoxType.YesNo;
					messageDetails.Title = "Copy list to clipboard";
					messageDetails.Content = "Are you sure you wish to copy the current list to clipboard, to allow paste as text?";
					messageDetails.MessageIcon = MessageBoxIconType.QuestionIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.QuestionBorder;
				}
				break;

			default:
				break;
		}

		return messageDetails;
	}

	public static MessageBoxDetails GetMessageContent(MessageBoxContentTypes Option, int numberOption)
	{
		MessageBoxDetails messageDetails = new MessageBoxDetails();

		switch (Option)
		{
			case MessageBoxContentTypes.ImportSuccessful:
				{
					messageDetails.MessageType = MessageBoxType.Ok;
					messageDetails.Title = "Import";
					messageDetails.Content = numberOption + " REITs Templates were imported successfully.";
					messageDetails.MessageIcon = MessageBoxIconType.SuccessIcon;
					messageDetails.MessageBorder = MessageBoxBorderType.SuccessBorder;
				}
				break;
		}

		return messageDetails;
	}

	#endregion Public Methods
}
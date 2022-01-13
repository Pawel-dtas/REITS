using Domain.MessageBoxModelsEnums;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Windows;

namespace REITs.Infrastructure.ViewModels
{
	public class SystemMessageBoxViewModel : BindableBase
	{
		#region properties

		public string MessageTitle
		{
			get { return _messageTitle; }
			set { SetProperty(ref _messageTitle, value); }
		}

		public string MessageContent
		{
			get { return _messageContent; }
			set { SetProperty(ref _messageContent, value); }
		}

		public string MessageIcon
		{
			get { return _messageIcon; }
			set { SetProperty(ref _messageIcon, value); }
		}

		public string MessageBorder
		{
			get { return _messageBorder; }
			set { SetProperty(ref _messageBorder, value); }
		}

		public MessageBoxType MessageType
		{
			get { return _messageType; }
			set
			{
				SetProperty(ref _messageType, value);
				SetMessageBoxButtonVisibility(_messageType);
			}
		}

		public MessageBoxButtonType MessageButtons
		{
			get { return _messageButtons; }
			set
			{
				SetProperty(ref _messageButtons, value);
				SetMessageBoxButtonVisibility(_messageType);
			}
		}

		public Action FinishInteraction { get; set; }

		public bool IsOKButtonVisible
		{
			get { return _isOKButtonVisible; }
			set
			{
				SetProperty(ref _isOKButtonVisible, value);
			}
		}

		public bool IsCancelButtonVisible
		{
			get { return _isCancelButtonVisible; }
			set
			{
				SetProperty(ref _isCancelButtonVisible, value);
			}
		}

		public bool AreYESNOButtonsVisible
		{
			get { return _areYESNOButtonsVisible; }
			set
			{
				SetProperty(ref _areYESNOButtonsVisible, value);
			}
		}

		public MessageBoxResult SelectedOption
		{
			get { return _selectedOption; }
			set
			{
				SetProperty(ref _selectedOption, value);
			}
		}

		private bool CanExecuteNo(Window arg)
		{
			return true;
		}

		private bool CanExecuteYes(Window arg)
		{
			return true;
		}

		private bool CanExecuteCancel(Window arg)
		{
			return true;
		}

		private bool CanExecuteOK(Window arg)
		{
			return true;
		}

		private void OkExecute(Window obj)
		{
			SelectedOption = MessageBoxResult.OK;
			obj.Close();
		}

		private void CancelExecute(Window obj)
		{
			SelectedOption = MessageBoxResult.Cancel;
			obj.Close();
		}

		private void YesExecute(Window obj)
		{
			SelectedOption = MessageBoxResult.Yes;
			obj.Close();
		}

		private void NoExecute(Window obj)
		{
			SelectedOption = MessageBoxResult.No;
			obj.Close();
		}

		#endregion properties

		#region Delegate Commands

		public DelegateCommand<Window> MessageBoxOKClickCommand { get; private set; }
		public DelegateCommand<Window> MessageBoxCancelClickCommand { get; private set; }
		public DelegateCommand<Window> MessageBoxYesClickCommand { get; private set; }
		public DelegateCommand<Window> MessageBoxNoClickCommand { get; private set; }

		#endregion Delegate Commands

		#region Variables

		private string _messageTitle;
		private string _messageContent;
		private string _messageIcon;
		private string _messageBorder;

		private MessageBoxType _messageType;
		private MessageBoxButtonType _messageButtons;
		private MessageBoxResult _selectedOption;

		private bool _isOKButtonVisible;
		private bool _isCancelButtonVisible;
		private bool _areYESNOButtonsVisible;

		#endregion Variables

		#region Constructor

		public SystemMessageBoxViewModel()
		{
			InitiliseCommands();
		}

		private void InitiliseCommands()
		{
			MessageBoxOKClickCommand = new DelegateCommand<Window>(OkExecute, CanExecuteOK);
			MessageBoxCancelClickCommand = new DelegateCommand<Window>(CancelExecute, CanExecuteCancel);
			MessageBoxYesClickCommand = new DelegateCommand<Window>(YesExecute, CanExecuteYes);
			MessageBoxNoClickCommand = new DelegateCommand<Window>(NoExecute, CanExecuteNo);
		}

		#endregion Constructor

		#region Private Methods

		private void SetMessageBoxButtonVisibility(MessageBoxType option)
		{
			IsOKButtonVisible = false;
			RaisePropertyChanged(nameof(IsOKButtonVisible));

			IsCancelButtonVisible = false;
			RaisePropertyChanged(nameof(IsCancelButtonVisible));

			AreYESNOButtonsVisible = false;
			RaisePropertyChanged(nameof(AreYESNOButtonsVisible));

			switch (option)
			{
				case MessageBoxType.Ok:
					{
						IsOKButtonVisible = true;
						RaisePropertyChanged(nameof(IsOKButtonVisible));
					}
					break;

				case MessageBoxType.YesNo:
					{
						AreYESNOButtonsVisible = true;
						RaisePropertyChanged(nameof(AreYESNOButtonsVisible));
					}
					break;

				case MessageBoxType.OkCancel:
					{
						IsCancelButtonVisible = true;
						RaisePropertyChanged(nameof(IsCancelButtonVisible));

						IsOKButtonVisible = true;
						RaisePropertyChanged(nameof(IsOKButtonVisible));
					}
					break;

				default:
					break;
			}
		}

		#endregion Private Methods
	}
}
using Domain;
using Domain.MessageBoxModelsEnums;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using REITs.DataLayer.Services;
using REITs.Domain.Enums;
using REITs.Domain.MenuModels;
using REITs.Domain.Models;
using REITs.Infrastructure;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace REITs.UserModule.ViewModels
{
	public class UsersViewModel : BindableBase, INavigationAware
	{
		#region Constructor

		public UsersViewModel(IEventAggregator eventAggregator, IUserDataService dataService)
		{
			_eventAggregator = eventAggregator;
			_userDataService = dataService;

			SaveUserCommand = new DelegateCommand(OnSaveUserExecute, CanExecuteSaveUser);
			DeleteUserCommand = new DelegateCommand(OnDeleteUserExecute, CanExecuteDeleteUser);
			EditUserCommand = new DelegateCommand(OnEditUserExecute, CanExecuteEditUser);

			ClearFieldsCommand = new DelegateCommand(ClearFields);

			RefreshUsersList();

			ClearFields();

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));
		}

		#endregion Constructor

		#region Properties

		public string UserPINumber
		{
			get { return _userPINumber; }
			set
			{
				SetProperty(ref _userPINumber, value);

				if (_userPINumber != null)
					if (_userPINumber.Length == 7 && editingExistingItem == false)
						CheckIfUserPIExists(_userPINumber);

				RecordChanged = true;

				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		private bool _userPINumberIsEditable;

		public bool UserPINumberIsEditable
		{
			get { return _userPINumberIsEditable; }
			set
			{
				SetProperty(ref _userPINumberIsEditable, value);
			}
		}

		public string UserForename
		{
			get { return _UserForename; }
			set
			{
				SetProperty(ref _UserForename, value);

				RecordChanged = true;

				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		public string UserSurname
		{
			get { return _userSurname; }
			set
			{
				SetProperty(ref _userSurname, value);

				RecordChanged = true;

				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		public string UserTelNo
		{
			get { return _userTelNo; }
			set
			{
				SetProperty(ref _userTelNo, value);

				RecordChanged = true;

				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		public string UserTeam
		{
			get { return _userTeam; }
			set
			{
				SetProperty(ref _userTeam, value);
				RecordChanged = true;
				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		public string UserJobRole
		{
			get { return _userJobRole; }
			set
			{
				SetProperty(ref _userJobRole, value);
				RecordChanged = true;
				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		public string UserAccessLevel
		{
			get { return _userAccessLevel; }
			set
			{
				SetProperty(ref _userAccessLevel, value);
				RecordChanged = true;
				SaveUserCommand.RaiseCanExecuteChanged();
			}
		}

		public bool RecordChanged = false;

		private ObservableCollection<SystemUser> _systemUsersList;

		public ObservableCollection<SystemUser> SystemUserList
		{
			get { return _systemUsersList; }
			set
			{
				SetProperty(ref _systemUsersList, value);
			}
		}

		private SystemUser _selectedSystemUserItem;

		public SystemUser SelectedSystemUserItem
		{
			get { return _selectedSystemUserItem; }
			set
			{
				SetProperty(ref _selectedSystemUserItem, value);

				SaveUserCommand.RaiseCanExecuteChanged();
				DeleteUserCommand.RaiseCanExecuteChanged();
			}
		}

		//ComboBox Item Lists
		public List<string> AccessLevels
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<AccessLevels>().ToList();
			}
		}

		public List<string> JobRoles
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<JobRoleTypes>().ToList();
			}
		}

		public List<string> Teams
		{
			get
			{
				List<string> displayList = new List<string>();
				displayList = BaseEnumExtension.GetEnumDescriptions<Teams>();
				return displayList;
			}
		}

		#endregion Properties

		#region Variables

		private string _userPINumber;

		private string _UserForename;
		private string _userSurname;
		private string _userTelNo;
		private string _userTeam;
		private string _userJobRole;
		private string _userAccessLevel;

		private bool editingExistingItem = false;

		private IEventAggregator _eventAggregator;
		private IUserDataService _userDataService;

		#endregion Variables

		#region Delegate Commands

		public DelegateCommand SaveUserCommand { get; private set; }
		public DelegateCommand DeleteUserCommand { get; private set; }
		public DelegateCommand EditUserCommand { get; private set; }
		public DelegateCommand ClearFieldsCommand { get; private set; }

		#endregion Delegate Commands

		#region Commands

		private void OnSaveUserExecute()
		{
			try
			{
				IUserDataService _dataService = PrismHelpers.ResolveService<IUserDataService>();

				SystemUser tempSystemUser = new SystemUser();
				tempSystemUser = _dataService.GetSystemUser(UserPINumber);

				if (tempSystemUser == null)
					tempSystemUser = new SystemUser();

				if (_selectedSystemUserItem == null)
					editingExistingItem = false;

				tempSystemUser.PINumber = UserPINumber;
				tempSystemUser.Forename = UserForename;
				tempSystemUser.Surname = UserSurname;
				tempSystemUser.TelephoneNumber = UserTelNo;
				tempSystemUser.Team = UserTeam;
				tempSystemUser.JobRole = UserJobRole;
				tempSystemUser.AccessLevel = UserAccessLevel;
				tempSystemUser.IsActive = true;

				if (SaveUserRecord(tempSystemUser) == true)
				{
					if (editingExistingItem)
					{
						CustomMessageBox.Show(MessageBoxContentTypes.EditUserSuccessful);
					}
					else
					{
						CustomMessageBox.Show(MessageBoxContentTypes.SaveUserSuccessful);
					}

					ClearFields();
				}
				else
				{
					if (editingExistingItem)
					{
						CustomMessageBox.Show(MessageBoxContentTypes.EditUserUnsuccessful);
					}
					else
					{
						CustomMessageBox.Show(MessageBoxContentTypes.SaveUserUnsuccessful);
					}
				}
			}
			catch
			{
				CustomMessageBox.Show(MessageBoxContentTypes.SaveUserUnsuccessful);
			}
		}

		private void OnDeleteUserExecute()
		{
			try
			{
				if (CustomMessageBox.Show(MessageBoxContentTypes.DeleteUserConfirmation) == MessageBoxResult.Yes)
				{
                    DeleteUser(SelectedSystemUserItem);

                    ClearFields();

					RefreshUsersList();

					CustomMessageBox.Show(MessageBoxContentTypes.DeleteUserSuccessful);
				}
			}
			catch
			{
				CustomMessageBox.Show(MessageBoxContentTypes.DeleteUserUnsuccessful);
			}
		}

		private void OnEditUserExecute()
		{
			try
			{
				UserPINumber = SelectedSystemUserItem.PINumber;
				UserForename = SelectedSystemUserItem.Forename;
				UserSurname = SelectedSystemUserItem.Surname;
				UserTelNo = SelectedSystemUserItem.TelephoneNumber;
				UserTeam = SelectedSystemUserItem.Team;
				UserJobRole = SelectedSystemUserItem.JobRole;
				UserAccessLevel = SelectedSystemUserItem.AccessLevel;

				RecordChanged = false;

				UserPINumberIsEditable = false;

				editingExistingItem = true;
			}
			catch
			{
				CustomMessageBox.Show(MessageBoxContentTypes.EditUserUnsuccessful);
			}
		}

		#endregion Commands

		#region Validation

		private bool CanExecuteSaveUser()
		{
			return (CheckUserComplete() == true && RecordChanged == true);
		}

		private bool CanExecuteDeleteUser()
		{
			return SelectedSystemUserItem != null;
		}

		private bool CanExecuteEditUser()
		{
			return (SelectedSystemUserItem != null);
		}

		#endregion Validation

		#region Private Methods

		private void CheckIfUserPIExists(string lookupPID)
		{
			SystemUser tempSystemUser = PrismHelpers.ResolveService<IUserDataService>().GetSystemUser(lookupPID);

			if (tempSystemUser != null)
			{
				editingExistingItem = true;

				SelectedSystemUserItem = tempSystemUser;

				OnEditUserExecute();
			}
		}

		private bool SaveUserRecord(SystemUser systemUser)
		{
			return (editingExistingItem) ? _userDataService.UpdateSystemUser(systemUser) : _userDataService.SaveSystemUser(systemUser);
		}

		private void RefreshUsersList()
		{
			SystemUserList = GetSystemUserList();

			SelectedSystemUserItem = null;
		}

		private bool CheckUserComplete()
		{
			bool result = true;

			if (string.IsNullOrEmpty(UserPINumber))
			{
				result = false;
			}
			else
			{
				if (UserPINumber.Length != 7)
					result = false;
			}

			if (string.IsNullOrEmpty(UserForename))
			{
				result = false;
			}

			if (string.IsNullOrEmpty(UserSurname))
			{
				result = false;
			}

			if (string.IsNullOrEmpty(UserTelNo))
			{
				result = false;
			}

			if (string.IsNullOrEmpty(UserTeam))
			{
				result = false;
			}

			if (string.IsNullOrEmpty(UserJobRole))
			{
				result = false;
			}

			if (string.IsNullOrEmpty(UserAccessLevel))
			{
				result = false;
			}

			return result;
		}

		private void ClearFields()
		{
			UserPINumber = null;
			UserForename = null;
			UserSurname = null;
			UserTelNo = null;
			UserTeam = null;
			UserJobRole = null;
			UserAccessLevel = null;

			editingExistingItem = false;

			UserPINumberIsEditable = true;

			RefreshUsersList();
		}

		private bool CheckItemPresentInList(List<SystemUser> listToCheck, SystemUser item)
		{
			bool valid = false;

			foreach (SystemUser listItem in listToCheck)
			{
				if (item.PINumber == listItem.PINumber && item.Forename == listItem.Forename && item.Surname == listItem.Surname &&
					item.TelephoneNumber == listItem.TelephoneNumber && item.Team == listItem.Team && item.AccessLevel == listItem.AccessLevel)
				{
					valid = true;
				}
			}
			return valid;
		}

		private bool CheckUserPresentInList(List<SystemUser> listToCheck, SystemUser item)
		{
			bool valid = false;

			foreach (SystemUser listItem in listToCheck)
			{
				if (item.PINumber == listItem.PINumber)
				{
					valid = true;
				}
			}
			return valid;
		}

		private ObservableCollection<SystemUser> GetSystemUserList()
		{
			IUserDataService _dataService = PrismHelpers.ResolveService<IUserDataService>();

			return new ObservableCollection<SystemUser>(_dataService.GetAllActiveSystemUsers());
		}

		private void DeleteUser(SystemUser tempSystemUser)
		{
			tempSystemUser.IsActive = false;

			_userDataService = PrismHelpers.ResolveService<IUserDataService>();

			_userDataService.UpdateSystemUser(tempSystemUser);
		}

		#endregion Private Methods

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Users", true));
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Users", false));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Print", false));
		}
	}
}
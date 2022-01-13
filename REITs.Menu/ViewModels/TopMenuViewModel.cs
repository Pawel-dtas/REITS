using Domain;
using Domain.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using REITs.Domain.Enums;
using REITs.Domain.MenuModels;
using REITs.Domain.Models;
using REITs.Infrastructure;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace REITs.MenuModule.ViewModels
{
	public class TopMenuViewModel : BindableBase
	{
		private bool _isSearchButtonEnabled;

		public bool IsSearchButtonEnabled
		{
			get { return _isSearchButtonEnabled; }
			set
			{
				SetProperty(ref _isSearchButtonEnabled, value);
			}
		}

		private bool _isREITButtonEnabled;

		public bool IsREITButtonEnabled
		{
			get { return _isREITButtonEnabled; }
			set
			{
				SetProperty(ref _isREITButtonEnabled, value);
			}
		}

		private bool _isREITCompanyButtonEnabled;

		public bool IsREITCompanyButtonEnabled
		{
			get { return _isREITCompanyButtonEnabled; }
			set
			{
				SetProperty(ref _isREITCompanyButtonEnabled, value);
			}
		}

		private bool _isREITCompanyAddButtonEnabled;

		public bool IsREITCompanyAddButtonEnabled
		{
			get { return _isREITCompanyAddButtonEnabled; }
			set
			{
				SetProperty(ref _isREITCompanyAddButtonEnabled, value);
			}
		}

		private bool _isSaveButtonEnabled;

		public bool IsSaveButtonEnabled
		{
			get { return _isSaveButtonEnabled; }
			set
			{
				SetProperty(ref _isSaveButtonEnabled, value);
			}
		}

		private bool _isPrintButtonEnabled;

		public bool IsPrintButtonEnabled
		{
			get { return _isPrintButtonEnabled; }
			set
			{
				SetProperty(ref _isPrintButtonEnabled, value);
			}
		}

		private bool _isExportPDFButtonEnabled;

		public bool IsExportPDFButtonEnabled
		{
			get { return _isExportPDFButtonEnabled; }
			set
			{
				SetProperty(ref _isExportPDFButtonEnabled, value);
			}
		}

		private bool _isReportsButtonEnabled;

		public bool IsReportsButtonEnabled
		{
			get { return _isReportsButtonEnabled; }
			set
			{
				SetProperty(ref _isReportsButtonEnabled, value);
			}
		}

		private bool _isImportButtonEnabled;

		public bool IsImportButtonEnabled
		{
			get { return _isImportButtonEnabled; }
			set
			{
				SetProperty(ref _isImportButtonEnabled, value);
			}
		}

		private bool _isSystemUsersButtonEnabled;

		public bool IsSystemUsersButtonEnabled
		{
			get { return _isSystemUsersButtonEnabled; }
			set
			{
				SetProperty(ref _isSystemUsersButtonEnabled, value);
			}
		}

		public DelegateCommand<string> MenuClickRequest { get; private set; }

		#region Constructor

		public TopMenuViewModel()
		{
			SetMenuDefaults();

			MenuClickRequest = new DelegateCommand<string>(OnMenuClickRequest);

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Subscribe(SetMenuEnabledState);

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemClickedEvent>().Subscribe(MenuItemClicked);
		}

		private void MenuItemClicked(MenuItemClickedEventPayload obj)
		{
			HandleMenuClickedRequest(obj.MenuName, obj.XMLGuid);
		}

		private void SetMenuEnabledState(MenuItemEnabledEventPayload obj)
		{
			var menuName = obj.MenuName;
			var menuEnabled = obj.MenuEnabled;

			var requestedMenu = menuName.GetEnumFromString<MenuItems>();

			switch (requestedMenu)
			{
				case MenuItems.Seach:
					IsSearchButtonEnabled = menuEnabled;
					break;

				case MenuItems.REIT:
					IsREITButtonEnabled = menuEnabled;
					break;

				case MenuItems.REITCompany:
					IsREITCompanyButtonEnabled = menuEnabled;
					break;

				case MenuItems.REITParentAdd:
					IsREITCompanyAddButtonEnabled = menuEnabled;
					break;

				case MenuItems.Save:
					IsSaveButtonEnabled = menuEnabled;
					break;

				case MenuItems.Print:
					IsPrintButtonEnabled = menuEnabled;
					break;

				case MenuItems.Export:
					IsExportPDFButtonEnabled = menuEnabled;
					break;

				case MenuItems.Report:
					IsReportsButtonEnabled = menuEnabled;
					break;

				case MenuItems.Import:
					IsImportButtonEnabled = menuEnabled;
					break;

				case MenuItems.Users:
					IsSystemUsersButtonEnabled = menuEnabled;
					break;
			}
		}

		private void SetMenuDefaults()
		{
			IsSearchButtonEnabled = false;

			IsREITButtonEnabled = false;
			IsREITCompanyButtonEnabled = false;
			IsREITCompanyAddButtonEnabled = false;

			IsSaveButtonEnabled = false;
			IsPrintButtonEnabled = false;

			IsExportPDFButtonEnabled = false;

			IsReportsButtonEnabled = (UserSecurityDetails.AccessLevel == AccessLevels.Admin
										|| UserSecurityDetails.AccessLevel == AccessLevels.User);

			IsImportButtonEnabled = (UserSecurityDetails.AccessLevel == AccessLevels.Admin);

			IsSystemUsersButtonEnabled = (UserSecurityDetails.AccessLevel == AccessLevels.Admin);
		}

		#endregion Constructor

		private void OnMenuClickRequest(string request)
		{
			ViewNames tempViewName = request.GetEnumFromString<ViewNames>();

			if (tempViewName != ViewNames.NotSet)
			{
				if (CheckCanNavigate())
					ManageNavigationRequest(tempViewName);
			}
			else  // must be a standalone command
			{
				HandleMenuClickedRequest(request, Guid.Empty);
			}
		}

		private bool CheckCanNavigate()
		{
			bool shouldAllowNavigate = false;

			if (IsSaveButtonEnabled)
			{
				MessageBoxResult result = MessageBox.Show("Record has been changed, but not saved.\n\nDiscard Changes?",
															"Unsaved Changes",
															MessageBoxButton.YesNo, MessageBoxImage.Exclamation);

				if (result == MessageBoxResult.Yes)
				{
					IsSaveButtonEnabled = false;
					shouldAllowNavigate = true;
				}
			}
			else
			{
				shouldAllowNavigate = true;
			}

			return shouldAllowNavigate;
		}

		private void HandleMenuClickedRequest(string request, Guid guid)
		{
			MenuItems chosenMenu = request.GetEnumFromString<MenuItems>();

			switch (chosenMenu)
			{
				case MenuItems.REIT:
					if (CheckCanNavigate())
						OpenREITRecord(guid);
					break;

				case MenuItems.REITParentAdd:
					if (CheckCanNavigate())
						OpenNewREITParentRecord();
					break;

				case MenuItems.Save:
					SaveRecord();
					break;

				case MenuItems.Print:
					PrintScreen();
					break;
			}
		}

		private void OpenREITRecord(Guid guid)
		{
			NavigationParameters navParameter = new NavigationParameters();
			navParameter.Add("ResultType", "Record");
			navParameter.Add("RequestSource", "Click");
			navParameter.Add("REITRecord", guid);

			PrismHelpers.GetRegionManager().RequestNavigate("ContentRegion", "REITView", navParameter);
		}

		private void OpenNewREITParentRecord()
		{
			NavigationParameters navParameter = new NavigationParameters();
			navParameter.Add("ResultType", "Create");
			navParameter.Add("RequestSource", "New");

			PrismHelpers.GetRegionManager().RequestNavigate("ContentRegion", "REITParentView", navParameter);
		}

		private void SaveRecord()
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemClickedEvent>().Publish(new MenuItemClickedEventPayload("SaveParent", Guid.Empty));
		}

		private void PrintScreen()
		{
			try
			{
				UserControl contentToPrint = new UserControl();

				object view = PrismHelpers.GetRegionManager().Regions["ContentRegion"].ActiveViews.FirstOrDefault();

				if (view != null)
					contentToPrint = (UserControl)view;

				if (contentToPrint != null)
				{
					PrintDialog printDlg = new System.Windows.Controls.PrintDialog();

					if (printDlg.ShowDialog() == true)
						printDlg.PrintTicket = new System.Printing.PrintTicket() { };     // define top margin
					printDlg.PrintVisual(contentToPrint, "Print Ouptut");
				}
			}
			catch (Exception ex) { }
		}

		private void ManageNavigationRequest(ViewNames viewName)
		{
			PrismHelpers.GetRegionManager().RequestNavigate("ContentRegion", viewName.GetDescriptionFromEnum());
		}
	}
}
using Domain;
using Domain.MessageBoxModelsEnums;
using Domain.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using REITs.DataLayer.Services;
using REITs.Domain.Enums;
using REITs.Domain.MenuModels;
using REITs.Domain.Models;
using REITs.Infrastructure;
using REITs.Infrastructure.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace REITs.REITParentDisplayModule.ViewModels
{
	public class REITParentViewModel : BindableBase, INavigationAware
	{
		// public props
		private REITParent _currentParent;

		public REITParent CurrentParent
		{
			get { return _currentParent; }
			set
			{
				SetProperty(ref _currentParent, value);

				_currentParent.PropertyChanged += null;
				_currentParent.PropertyChanged += new ObjectChangeEventing(this).ObjectPropertyChanged;

				if (LastStateParent == null)
				{
					LastStateParent = new REITParent();

					UIHelper.CopyObject(CurrentParent, LastStateParent);
				}
			}
		}

		private REITParentReviewFS _currentParentReviewFS;

		public REITParentReviewFS CurrentParentReviewFS
		{
			get { return _currentParentReviewFS; }
			set
			{
				SetProperty(ref _currentParentReviewFS, value);

				if (_currentParentReviewFS != null)
				{
					_currentParentReviewFS.PropertyChanged += null;
					_currentParentReviewFS.PropertyChanged += new ObjectChangeEventing(this).ObjectPropertyChanged;

					if (LastStateParentReviewFS == null)
					{
						LastStateParentReviewFS = new REITParentReviewFS();

						UIHelper.CopyObject(_currentParentReviewFS, LastStateParentReviewFS);
					}
				}
			}
		}

		private REITParentReviewRFS _currentParentReviewRFS;

		public REITParentReviewRFS CurrentParentReviewRFS
		{
			get { return _currentParentReviewRFS; }
			set
			{
				SetProperty(ref _currentParentReviewRFS, value);

				if (_currentParentReviewRFS != null)
				{
					_currentParentReviewRFS.PropertyChanged += null;
					_currentParentReviewRFS.PropertyChanged += new ObjectChangeEventing(this).ObjectPropertyChanged;

					if (LastStateParentReviewRFS == null)
					{
						LastStateParentReviewRFS = new REITParentReviewRFS();

						UIHelper.CopyObject(_currentParentReviewRFS, LastStateParentReviewRFS);
					}
				}
			}
		}

		private REIT _selectedXMLSub;

		public REIT SelectedXMLSub
		{
			get { return _selectedXMLSub; }
			set
			{
				SetProperty(ref _selectedXMLSub, value);
			}
		}

		private bool _isEnabled;

		public bool IsEnabled
		{
			get { return _isEnabled; }

			set
			{
				SetProperty(ref _isEnabled, value);
			}
		}

		public DelegateCommand OpenREITXMLSubmission { get; private set; }
		public DelegateCommand<object> ContextMenuAPEYearHandlerCommand { get; set; }

		public DelegateCommand DiscardEditorChanges { get; set; }

		public DelegateCommand CommandSectorsCheckBoxChanged { get; set; }

		public REITParentReviewFS SelectedParentReviewFS { get; set; }
		public REITParentReviewRFS SelectedParentReviewRFS { get; set; }

		public Visibility FSEditorVisible { get; set; }
		public Visibility RFSEditorVisible { get; set; }

		public int FSActiveBorderThickness { get; set; }
		public int RFSActiveBorderThickness { get; set; }

		public REITParentViewModel()
		{
			OpenREITXMLSubmission = new DelegateCommand(OnOpenXMLSub, CanExecuteOpenXMLSub);

			ContextMenuAPEYearHandlerCommand = new DelegateCommand<object>(HandleAPEYearContextSelection);

			DiscardEditorChanges = new DelegateCommand(PerformDiscardEditorChanges);

			CommandSectorsCheckBoxChanged = new DelegateCommand(ToggleCheckBoxSectorsChanged);

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemClickedEvent>().Subscribe(MenuItemClicked);
		}

		private void PerformDiscardEditorChanges()
		{
			CurrentParentReviewFS = null;
			CurrentParentReviewRFS = null;

			FSEditorVisible = Visibility.Collapsed;
			RFSEditorVisible = Visibility.Collapsed;

			SetViewsEnabled(true);

			RaiseEditorVisibility();

			if (REITFSRecordChanged)
				LoadFSReviewList();

			if (REITRFSRecordChanged)
				LoadRFSReviewList();

			REITFSRecordChanged = false;
			REITRFSRecordChanged = false;

			if (!REITParentRecordChanged)
				UpdateSaveMenuState(false);
		}

		public bool CanExecuteOpenXMLSub()
		{
			return SelectedXMLSub != null;
		}

		public List<string> YesNoOptionsList
		{
			get
			{
				return Enum.GetValues(typeof(YesNoOptions)).Cast<YesNoOptions>().Select(x => x.GetDescriptionFromEnum()).ToList();
			}
		}

		public List<string> RiskOptionsList
		{
			get
			{
				return Enum.GetValues(typeof(RiskOptionTypes)).Cast<RiskOptionTypes>().Select(x => x.GetDescriptionFromEnum()).ToList();
			}
		}

		private ObservableCollection<CustomCheckBoxItem> _sectorTypesListCollection;

		public ObservableCollection<CustomCheckBoxItem> SectorTypesListCollection
		{
			get
			{
				if (_sectorTypesListCollection != null)
					CollectionViewSource.GetDefaultView(_sectorTypesListCollection).Refresh();

				return _sectorTypesListCollection;
			}

			set
			{
				SetProperty(ref _sectorTypesListCollection, value);
			}
		}

		public string SectorsLabelText
		{
			get { return FormattedSectorsLabel(); }
		}

		private bool _parentRecordViewEnabled;

		public bool ParentRecordViewEnabled
		{
			get { return _parentRecordViewEnabled; }
			set
			{
				SetProperty(ref _parentRecordViewEnabled, value);
			}
		}

		private bool _fsViewEnabled;

		public bool FSViewEnabled
		{
			get { return _fsViewEnabled; }
			set
			{
				SetProperty(ref _fsViewEnabled, value);
			}
		}

		private bool _rfsViewEnabled;

		public bool RFSViewEnabled
		{
			get { return _rfsViewEnabled; }
			set
			{
				SetProperty(ref _rfsViewEnabled, value);
			}
		}

		private ObservableCollection<REITParentReviewFS> _parentReviewFSList;

		public ObservableCollection<REITParentReviewFS> ParentReviewFSList
		{
			get { return _parentReviewFSList; }

			set
			{
				SetProperty(ref _parentReviewFSList, value);
			}
		}

		private ObservableCollection<REITParentReviewRFS> _parentReviewRFSList;

		public ObservableCollection<REITParentReviewRFS> ParentReviewRFSList
		{
			get { return _parentReviewRFSList; }

			set
			{
				SetProperty(ref _parentReviewRFSList, value);
			}
		}

		private ObservableCollection<REIT> _XMLSubList;

		public ObservableCollection<REIT> XMLSubList
		{
			get { return _XMLSubList; }

			set
			{
				SetProperty(ref _XMLSubList, value);
			}
		}

		public IList<string> ListOfCCM
		{
			get { return GetListOfSystemUsersByJobRole(JobRoleTypes.CCM); }
		}

		public IList<string> ListOfCoOrd
		{
			get { return GetListOfSystemUsersByJobRole(JobRoleTypes.CoOrd); }
		}

		public IList<string> ListOfCTG7
		{
			get { return GetListOfSystemUsersByJobRole(JobRoleTypes.CTG7); }
		}

		public IList<string> ListOfTHO
		{
			get { return GetListOfSystemUsersByJobRole(JobRoleTypes.CTHO); }
		}

		public ColumnConfig FSListViewColumns { get; set; }

		public ColumnConfig RFSListViewColumns { get; set; }

		public ColumnConfig XMLSubListViewColumns { get; set; }

		private REITParent LastStateParent;
		private REITParentReviewFS LastStateParentReviewFS;
		private REITParentReviewRFS LastStateParentReviewRFS;
		private IREITDataService _dataService = PrismHelpers.ResolveService<IREITDataService>();

		private bool REITParentSectorsChanged = false;
		private bool REITParentRecordChanged = false;
		private bool REITFSRecordChanged = false;
		private bool REITRFSRecordChanged = false;
		private bool IsBusy = false;

		#region Private Methods

		private void LoadParentRecord(Guid currentParentGuid)
		{
			SetDefaultFSRFSStates();

			if (UserSecurityDetails.AccessLevel == AccessLevels.Viewer)
				SetViewsEnabled(false);

			new Action(async () =>
			{
				CurrentParent = await PrismHelpers.ResolveService<IREITDataService>().GetREITParentRecord(currentParentGuid);

				REITParentRecordChanged = false;

				PopulateChildDataLists();

				FillSectorTypesCheckBoxList();
			}).Invoke();
		}

		private void CreateNewParent()
		{
			SetDefaultFSRFSStates();

			if (UserSecurityDetails.AccessLevel == AccessLevels.Viewer)
				SetViewsEnabled(false);

			CurrentParent = new REITParent();
			CurrentParent.Id = Guid.NewGuid();
			CurrentParent.PrincipalCustomerName = string.Format("{0} - New Customer Name", UserSecurityDetails.PINumber);
			CurrentParent.APEDate = DateTime.Today;
			CurrentParent.PrincipalUTR = "1234567890";
			CurrentParent.IsActive = true;

			REITParentRecordChanged = false;

			PopulateChildDataLists();

			FillSectorTypesCheckBoxList();
		}

		private void PopulateChildDataLists()
		{
			LoadXMLSubList();

			LoadFSReviewList();

			LoadRFSReviewList();
		}

		private void LoadFSReviewList()
		{
			new Action(async () =>
			{
				ParentReviewFSList = new ObservableCollection<REITParentReviewFS>(await PrismHelpers.ResolveService<IREITDataService>().GetAllFSReviewsForParent(CurrentParent.Id));

				IsEnabled = false;
			}).Invoke();

			IsEnabled = true;

			FSListViewColumns = FSColumnConfigs;

			RaisePropertyChanged(nameof(FSListViewColumns));
		}

		private void LoadRFSReviewList()
		{
			new Action(async () =>
			{
				ParentReviewRFSList = new ObservableCollection<REITParentReviewRFS>(await PrismHelpers.ResolveService<IREITDataService>().GetAllRFSReviewsForParent(CurrentParent.Id));

				IsEnabled = false;
			}).Invoke();

			IsEnabled = true;

			RFSListViewColumns = RFSColumnConfigs;

			RaisePropertyChanged(nameof(RFSListViewColumns));
		}

		private IList<string> GetListOfSystemUsersByJobRole(JobRoleTypes jobRole)
		{
			IList<string> tempUserList = StaticUserList.StaticListOfUsers.Where(x => x.JobRole == jobRole.GetDescriptionFromEnum()).Select(x => x.FullNameAndPINumber).ToList();
			tempUserList.Insert(0, string.Empty);

			return tempUserList;
		}

		private void LoadXMLSubList()
		{
			new Action(async () =>
			{
				XMLSubList = new ObservableCollection<REIT>(await PrismHelpers.ResolveService<IREITDataService>().GetAllREITsForParent(CurrentParent.Id));

				IsEnabled = false;
			}).Invoke();

			IsEnabled = true;

			XMLSubListViewColumns = XMLSubColumnConfigs;

			RaisePropertyChanged(nameof(XMLSubListViewColumns));
		}

		private string GetUserNameFromPID(string pid)
		{
			string result = string.Empty;

			SystemUser tempSystemUser = PrismHelpers.ResolveService<IUserDataService>().GetSystemUser(pid);

			if (tempSystemUser != null)
				result = tempSystemUser.FullNameAndPINumber;

			return result;
		}

		private void HandleAPEYearContextSelection(object paramList)
		{
			IList<string> strList = (IList<string>)paramList;

			ContextMenuActionTypes chosenMenuItem = strList[0].GetEnumFromString<ContextMenuActionTypes>();
			EditorWindows chosenListView = strList[1].GetEnumFromString<EditorWindows>();

			switch (chosenListView)
			{
				case EditorWindows.FSReviewList:
					FSEditorVisible = Visibility.Visible;
					HandleEditorObject(SelectedParentReviewFS, chosenMenuItem, chosenListView);
					FSActiveBorderThickness = 5;
					break;

				case EditorWindows.RFSReviewList:
					RFSEditorVisible = Visibility.Visible;
					HandleEditorObject(SelectedParentReviewRFS, chosenMenuItem, chosenListView);
					RFSActiveBorderThickness = 5;
					break;

				default:
					SetViewsEnabled(true);
					break;
			}
		}

		private void HandleEditorObject(object selectedObj, ContextMenuActionTypes chosenMenu, EditorWindows editorList)
		{
			switch (chosenMenu)
			{
				case ContextMenuActionTypes.Edit:
					if (selectedObj is REITParentReviewFS)
						CurrentParentReviewFS = SelectedParentReviewFS;

					if (selectedObj is REITParentReviewRFS)
						CurrentParentReviewRFS = SelectedParentReviewRFS;

					SetViewsEnabled(false);

					RaiseEditorVisibility();
					break;

				case ContextMenuActionTypes.New:
					if (editorList == EditorWindows.FSReviewList)
					{
						CurrentParentReviewFS = new REITParentReviewFS();
						CurrentParentReviewFS.Id = Guid.NewGuid();
						CurrentParentReviewFS.ParentId = CurrentParent.Id;
						CurrentParentReviewFS.IsActive = true;
						CurrentParentReviewFS.FSAPEYear = DateTime.Now;
					}

					if (editorList == EditorWindows.RFSReviewList)
					{
						CurrentParentReviewRFS = new REITParentReviewRFS();
						CurrentParentReviewRFS.RFSAPEYear = GetRFSNextYear();
						CurrentParentReviewRFS.ParentId = CurrentParent.Id;
					}

					SetViewsEnabled(false);

					RaiseEditorVisibility();
					break;

				case ContextMenuActionTypes.Delete:
					if (selectedObj is REITParentReviewFS)
					{
						try
						{
							CurrentParentReviewFS = SelectedParentReviewFS;

							if (CustomMessageBox.Show(MessageBoxContentTypes.DeleteFSRecordCheck) == MessageBoxResult.Yes)
							{
								CurrentParentReviewFS.IsActive = false;
								REITFSRecordChanged = true;
								SaveRecord();
								CustomMessageBox.Show(MessageBoxContentTypes.DeleteSuccessful);
							}
						}
						catch
						{
							CustomMessageBox.Show(MessageBoxContentTypes.DeleteSuccessful);
						}
					}
					break;
			}
		}

		private DateTime GetFSNextYear()
		{
			DateTime tempYear = ParentReviewFSList.OrderByDescending(x => x.FSAPEYear).Select(x => x.FSAPEYear).FirstOrDefault();

			return tempYear.AddYears(1);
		}

		private DateTime GetRFSNextYear()
		{
			DateTime tempYear = ParentReviewRFSList.OrderByDescending(x => x.RFSAPEYear).Select(x => x.RFSAPEYear).FirstOrDefault();

			return tempYear.AddYears(1);
		}

		private DateTime GetFSEarlierYear()
		{
			DateTime tempYear = ParentReviewFSList.OrderBy(x => x.FSAPEYear).Select(x => x.FSAPEYear).FirstOrDefault();

			return tempYear.AddYears(-1);
		}

		private DateTime GetRFSEarlierYear()
		{
			DateTime tempYear = ParentReviewRFSList.OrderBy(x => x.RFSAPEYear).Select(x => x.RFSAPEYear).FirstOrDefault();

			return tempYear.AddYears(-1);
		}

		private void SetDefaultFSRFSStates()
		{
			ParentRecordViewEnabled = true;

			FSEditorVisible = Visibility.Collapsed;
			RFSEditorVisible = Visibility.Collapsed;

			FSViewEnabled = true;
			RFSViewEnabled = true;

			FSActiveBorderThickness = 0;
			RFSActiveBorderThickness = 0;

			RaiseViewsEnabled();
			RaiseEditorVisibility();
		}

		private void SetViewsEnabled(bool state)
		{
			ParentRecordViewEnabled = state;
			FSViewEnabled = state;
			RFSViewEnabled = state;

			if (state == true)
			{
				FSActiveBorderThickness = 0;
				RFSActiveBorderThickness = 0;
			}

			RaiseViewsEnabled();
		}

		private void RaiseViewsEnabled()
		{
			RaisePropertyChanged(nameof(ParentRecordViewEnabled));
			RaisePropertyChanged(nameof(FSViewEnabled));
			RaisePropertyChanged(nameof(RFSViewEnabled));
			RaisePropertyChanged(nameof(FSActiveBorderThickness));
			RaisePropertyChanged(nameof(RFSActiveBorderThickness));

			RaisePropertyChanged(nameof(ParentReviewFSList));
			RaisePropertyChanged(nameof(ParentReviewRFSList));
		}

		private void RaiseEditorVisibility()
		{
			RaisePropertyChanged(nameof(FSEditorVisible));
			RaisePropertyChanged(nameof(RFSEditorVisible));
		}

		public void ObjectREITParentChanged()
		{
			if (!CurrentParent.Equals(LastStateParent))
			{
				REITParentRecordChanged = true;

				UpdateSaveMenuState(true);
			}
		}

		public void ObjectREITParentReviewFSChanged()
		{
			if (!CurrentParentReviewFS.Equals(LastStateParentReviewFS))
			{
				REITFSRecordChanged = true;
				UpdateSaveMenuState(true);
			}
		}

		public void ObjectREITParentReviewRFSChanged()
		{
			if (!CurrentParentReviewRFS.Equals(LastStateParentReviewRFS))
			{
				REITRFSRecordChanged = true;
				UpdateSaveMenuState(true);
			}
		}

		public void PropertyLeftRegimeChanged()
		{
			if (CurrentParent.LeftRegime.HasValue)
			{
				CurrentParent.NextBRRDate = null;

				ObjectREITParentChanged();
			}
		}

		private void UpdateSaveMenuState(bool enabled)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Save", enabled));
		}

		private void MenuItemClicked(MenuItemClickedEventPayload obj)
		{
			MenuItems requestedClick = obj.MenuName.GetEnumFromString<MenuItems>();

			switch (requestedClick)
			{
				case MenuItems.SaveParent:
					SaveRecord();
					break;
			}
		}

		private void SaveRecord()
		{
			bool overallSuccess = true;

			if (REITParentRecordChanged)
			{
				if (REITParentSectorsChanged)
					RecordSectorTypesCheckBoxList();

				UIHelper.CopyObject(CurrentParent, LastStateParent);

				if (!_dataService.SaveREITParent(CurrentParent))
				{
					overallSuccess = false;
					REITParentRecordChanged = false;
				}
			}

			if (REITFSRecordChanged)
			{
				UIHelper.CopyObject(CurrentParentReviewFS, LastStateParentReviewFS);

				if (!_dataService.SaveREITParentReviewFS(CurrentParentReviewFS))
				{
					overallSuccess = false;
				}
				else
				{
					CurrentParentReviewFS = null;
					FSEditorVisible = Visibility.Collapsed;
					REITFSRecordChanged = false;
					SetViewsEnabled(true);
					LoadFSReviewList();
				}
			}

			if (REITRFSRecordChanged)
			{
				UIHelper.CopyObject(CurrentParentReviewRFS, LastStateParentReviewRFS);

				if (!_dataService.SaveREITParentReviewRFS(CurrentParentReviewRFS))
				{
					overallSuccess = false;
				}
				else
				{
					CurrentParentReviewRFS = null;
					RFSEditorVisible = Visibility.Collapsed;
					REITRFSRecordChanged = false;
					SetViewsEnabled(true);
					LoadRFSReviewList();
				}
			}

			if (overallSuccess)
			{
				RaiseEditorVisibility();
				UpdateSaveMenuState(false);
			}
		}

		private void OnOpenXMLSub()
		{
			if (SelectedXMLSub != null)
			{
				Guid recordGuid = SelectedXMLSub.Id;

				PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITCompany", true));

				PrismHelpers.GetEventAggregator().GetEvent<MenuItemClickedEvent>().Publish(new MenuItemClickedEventPayload("REIT", recordGuid));
			}
		}

		private void FillSectorTypesCheckBoxList()
		{
			SectorTypesListCollection = new ObservableCollection<CustomCheckBoxItem>();

			SectorTypes flags = (SectorTypes)CurrentParent.SectorsFlag;

			foreach (string item in Enum.GetValues(typeof(SectorTypes)).Cast<SectorTypes>().Select(x => x.GetDescriptionFromEnum()).OrderBy(x => x).ToList())
			{
				bool selected = flags.HasFlag(item.GetEnumFromString<SectorTypes>());

				SectorTypesListCollection.Add(new CustomCheckBoxItem() { Name = item, IsSelected = selected });
			}

			RaisePropertyChanged("SectorsLabelText");
		}

		private void RecordSectorTypesCheckBoxList()
		{
			SectorTypes flags = 0;

			foreach (CustomCheckBoxItem item in SectorTypesListCollection)
			{
				if (item.IsSelected)
					flags = flags | item.Name.GetEnumFromString<SectorTypes>();
			}

			CurrentParent.SectorsFlag = (int)flags;
		}

		private void ToggleCheckBoxSectorsChanged()
		{
			REITParentRecordChanged = true;

			REITParentSectorsChanged = true;

			RaisePropertyChanged("SectorsLabelText");

			UpdateSaveMenuState(true);
		}

		private string FormattedSectorsLabel()
		{
			int selectedItemsCount = 0;

			if (SectorTypesListCollection != null)
				selectedItemsCount = SectorTypesListCollection.Where(x => x.IsSelected == true).Count();

			return string.Format("{0} items selected", selectedItemsCount);
		}

		#endregion Private Methods

		#region ColumnConfigs

		private ColumnConfig FSColumnConfigs = new ColumnConfig
		{
			Columns = new List<Column> {
							new Column { Header = "APEY", DataField = "FSAPEYear", Width = 45, IsYear = true},
							new Column { Header = "FS Due", DataField = "FSDue", Width = 90, IsDate = true},
							new Column { Header = "FS Rec", DataField = "FSRecDate", Width = 90, IsDate = true},
							new Column { Header = "PID Due", DataField = "PIDDueDate", Width = 90, IsDate = true},
							new Column { Header = "PID Rec", DataField = "PIDRecDate", Width = 90, IsDate = true},
							new Column { Header = "Comments", DataField = "Comments", Width = 900, IsDate = true}
			}
		};

		private ColumnConfig RFSColumnConfigs = new ColumnConfig
		{
			Columns = new List<Column> {
							new Column { Header = "APEY", DataField = "RFSAPEYear", Width = 45, IsYear = true},
							new Column { Header = "FS Reviewed", DataField = "FSReviewedAPEDate", Width = 90, IsDate = true},
							new Column { Header = "RiskStatus", DataField = "RiskStatus", Width = 90, IsDate = true},
							new Column { Header = "OnBRR TT", DataField = "OnBRRTT", Width = 100, IsDate = true},
							new Column { Header = "Int BRR Due", DataField = "InternalBRRDueDate", Width = 90, IsDate = true},
							new Column { Header = "Reviewed", DataField = "RFSReviewedDate", Width = 90, IsDate = true},
							new Column { Header = "RA Plan Meet ", DataField = "RAPlanMeetDate", Width = 100, IsDate = true},
							new Column { Header = "Reviewed", DataField = "ReviewedDate", Width = 90, IsDate = true},
							new Column { Header = "Next Review", DataField = "NextReviewDate", Width = 100, IsDate = true},
							new Column { Header = "Comments", DataField = "Comments", Width = 700, IsDate = true}
			}
		};

		private ColumnConfig XMLSubColumnConfigs = new ColumnConfig
		{
			Columns = new List<Column> {
							new Column { Header = "APE", DataField = "AccountPeriodEnd", Width = 90, IsDate = true},
							new Column { Header = "Name", DataField = "REITName", Width = 150},
							new Column { Header = "Vrn", DataField = "XMLVersion", Width = 30},
							new Column { Header = "Submitted", DataField = "XMLDateSubmitted", Width = 140, IsDateTime = true},
							new Column { Header = "Imported", DataField = "DateCreated", Width = 85, IsDate = true},
							new Column { Header = "Notes", DataField = "REITNotes", Width = 400}
			}
		};

		#endregion ColumnConfigs

		#region Navigation Methods

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITParentAdd", false));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Print", true));

			PrismHelpers.GetEventAggregator().GetEvent<TopMenuVisibilityCollapseEvent>().Publish();

			try
			{
				if (navigationContext == null)
					return;

				Guid? currentParent = navigationContext.Parameters["REITParentRecord"] as Guid?;

				if (currentParent != null && (currentParent != Guid.Empty))
				{
					LoadParentRecord((Guid)currentParent);
				}
				else
				{
					string requestSource = navigationContext.Parameters["RequestSource"] as string;

					if (requestSource == "New")
					{
						CreateNewParent();
					}
				}
			}
			catch (Exception ex) { }
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{ }

		#endregion Navigation Methods
	}
}
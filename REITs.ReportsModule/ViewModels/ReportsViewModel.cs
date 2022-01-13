using Browse;
using Domain;
using Domain.Enums;
using Domain.MessageBoxModelsEnums;
using Domain.Models;
using Hmrc.BDApp.O365.Services;
using Hmrc.BDApp.WorkFlow.AppConfig;
using Hmrc.BDApp.WorkFlow.DialogBrowsers;
using Hmrc.BDApp.WorkFlow.Excel;
using Hmrc.BDApp.WorkFlow.FileStorage;
using Microsoft.Graph;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using REITs.DataLayer.Services;
using REITs.Domain.Enums;
using REITs.Domain.MenuModels;
using REITs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Xml.Linq;

namespace REITs.ReportsModule.ViewModels
{
	public partial class ReportsViewModel : BindableBase, INavigationAware
	{
		private ReportCriteria ReportingCriteria;

		private ReportService _reportService = new ReportService();

		private DataTable _reportEntities;

		private IList<CustomerListCheckBoxItem> FullCustomerList;

		private ReportType SelectedReport = ReportType.NotSet;

		public DataTable ReportEntities
		{
			get { return _reportEntities; }
			set
			{
				SetProperty(ref _reportEntities, value);

				RaisePropertyChanged(nameof(HasResults));

				if (_reportEntities != null)
					UpdateRecordInformationDetails();
			}
		}

		private bool _isBusy = false;

		public bool IsBusy
		{
			get { return _isBusy; }
			set
			{
				SetProperty(ref _isBusy, value);

				RaisePropertyChanged(nameof(IsEnabled));
			}
		}

		public bool IsEnabled
		{
			get { return !IsBusy; }
		}

		private string _selectedReportType;

		public string SelectedReportType
		{
			get { return _selectedReportType; }
			set
			{
				SetProperty(ref _selectedReportType, value);

				HandleReportTypeChange();
			}
		}

		private bool _adjustmentCategoryEnabled;

		public bool AdjustmentCategoryEnabled
		{
			get { return _adjustmentCategoryEnabled; }
			set
			{
				SetProperty(ref _adjustmentCategoryEnabled, value);
			}
		}

		private bool _entityLevelEnabled;

		public bool EntityLevelEnabled
		{
			get { return _entityLevelEnabled; }
			set
			{
				SetProperty(ref _entityLevelEnabled, value);
			}
		}

		private string _selectedCompany;

		public string SelectedCompany
		{
			get { return _selectedCompany; }
			set
			{
				if (value.StartsWith("System.Windows.Controls.ListBox")) // omit listbox selection
					value = null;

				SetProperty(ref _selectedCompany, value);

				FilterCustomerList(value);

				RaisePropertyChanged(nameof(CustomerList));
			}
		}

		private string _selectedVersion;

		public string SelectedVersion
		{
			get { return _selectedVersion; }
			set
			{
				SetProperty(ref _selectedVersion, value);

				ProduceReportCommand.RaiseCanExecuteChanged();
			}
		}

		private string _selectedReportAnalysisType;

		public string SelectedReportAnalysisType
		{
			get { return _selectedReportAnalysisType; }
			set
			{
				SetProperty(ref _selectedReportAnalysisType, value);

				if (_selectedReportAnalysisType != null)
					ConfigureDropDownsForAnalysisType();

				ProduceReportCommand.RaiseCanExecuteChanged();
			}
		}

		private void ConfigureDropDownsForAnalysisType()
		{
			DefaultDropDownLists();

			if (CustomerList.Where(x => x.IsSelected == true).Count() == 0)
				CustomMessageBox.Show(MessageBoxContentTypes.NoCustomersSelected);

			ReportAnalysisTypes chosenReportAnalysisType = SelectedReportAnalysisType.GetEnumFromString<ReportAnalysisTypes>();

			switch (chosenReportAnalysisType)
			{
				case ReportAnalysisTypes.Adjustments:
					AdjustmentCategoryEnabled = true;
					GetUsedCategoryNames();
					EntityLevelEnabled = true;
					break;

				case ReportAnalysisTypes.Reconciliations:
					AdjustmentCategoryEnabled = false;
					GetUsedReconciliationTypes();
					EntityLevelEnabled = false;
					break;
			}
		}

		private Visibility _analysisPanelVisibility;

		public Visibility AnalysisPanelVisibility
		{
			get { return _analysisPanelVisibility; }
			set
			{
				SetProperty(ref _analysisPanelVisibility, value);
			}
		}

		private string _selectedAdjustmentCategory;

		public string SelectedAdjustmentCategory
		{
			get { return _selectedAdjustmentCategory; }
			set
			{
				SetProperty(ref _selectedAdjustmentCategory, value);

				if (_selectedAdjustmentCategory != null)
					PopulateAdjustmentTypeList();

				ProduceReportCommand.RaiseCanExecuteChanged();
			}
		}

		private string _selectedAnalysisType;

		public string SelectedAnalysisType
		{
			get { return _selectedAnalysisType; }
			set
			{
				SetProperty(ref _selectedAnalysisType, value);

				if (_selectedAnalysisType != null)
					PopulateAnalysisNameList();

				SelectedAnalysisName = null;

				ProduceReportCommand.RaiseCanExecuteChanged();
			}
		}

		private void PopulateAnalysisNameList()
		{
			ReportAnalysisTypes chosenReportAnalysisType = SelectedReportAnalysisType.GetEnumFromString<ReportAnalysisTypes>();

			switch (chosenReportAnalysisType)
			{
				case ReportAnalysisTypes.Adjustments:
					GetUsedAdjustmentNames();
					break;

				case ReportAnalysisTypes.Reconciliations:
					GetUsedReconciliationNames();
					break;
			}
		}

		private string _selectedAnalysisName;

		public string SelectedAnalysisName
		{
			get { return _selectedAnalysisName; }
			set
			{
				SetProperty(ref _selectedAnalysisName, value);

				ProduceReportCommand.RaiseCanExecuteChanged();
			}
		}

		private string _selectedEntityLevel = "No";

		public string SelectedEntityLevel
		{
			get { return _selectedEntityLevel; }
			set
			{
				SetProperty(ref _selectedEntityLevel, value);

				ProduceReportCommand.RaiseCanExecuteChanged();
			}
		}

		private string _recordsReturned;

		private Visibility _noteSearchWordVisibility;

		public Visibility NoteSearchWordVisibility
		{
			get { return _noteSearchWordVisibility; }
			set
			{
				SetProperty(ref _noteSearchWordVisibility, value);
			}
		}

		private List<string> _listOfNoteSearchWords;

		public List<string> ListOfNoteSearchWords
		{
			get { return _listOfNoteSearchWords; }
			set
			{
				SetProperty(ref _listOfNoteSearchWords, value);
			}
		}

		private string _selectedNoteSearchWord;

		public string SelectedNoteSearchWord
		{
			get { return _selectedNoteSearchWord; }
			set
			{
				SetProperty(ref _selectedNoteSearchWord, value);

				CustomNoteSearchWordText = _selectedNoteSearchWord;
			}
		}

		private string _customNoteSearchWordText;

		public string CustomNoteSearchWordText
		{
			get { return _customNoteSearchWordText; }
			set
			{
				SetProperty(ref _customNoteSearchWordText, value);
			}
		}

		public string RecordsReturned
		{
			get { return _recordsReturned; }
			set
			{
				SetProperty(ref _recordsReturned, value);
			}
		}

		private string _reportInformationLabel;

		public string ReportInformationLabel
		{
			get { return _reportInformationLabel; }
			set
			{
				SetProperty(ref _reportInformationLabel, value);
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

		public string CustomersLabelText
		{
			get { return FormattedCustomersLabel(); }
		}

		public bool HasResults
		{
			get { return (ReportEntities != null); }
		}

		public List<string> ReportTypeList
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<ReportType>().ToList();
			}
		}

		private ObservableCollection<CustomerListCheckBoxItem> _customerList;

		public ObservableCollection<CustomerListCheckBoxItem> CustomerList
		{
			get
			{
				if (_customerList != null)
					CollectionViewSource.GetDefaultView(_customerList).Refresh();

				return _customerList;
			}

			set
			{
				SetProperty(ref _customerList, value);
			}
		}

		public List<string> VersionTypeList
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<VersionTypes>().ToList();
			}
		}

		public List<string> ReportAnalysisTypeList
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<ReportAnalysisTypes>().ToList();
			}
		}

		public List<string> EntityLevelOptions
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<YesNoOptions>().ToList();
			}
		}

		public List<string> MonthsList
		{
			get
			{
				List<string> monthsList = new List<string>();

				foreach (string m in System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray())
					monthsList.Add(m.Substring(0, 3));

				return monthsList;
			}
		}

		public List<string> YearsList
		{
			get
			{
				List<string> yearsList = new List<string>();

				int fY = 2016;  // first date of record

				int cy1 = DateTime.Now.Year + 1; // CY +1

				for (int i = fY; i <= cy1; i++)
					yearsList.Insert(0, i.ToString());  // list reverse

				return yearsList;
			}
		}

		private string _selectedMonthFrom = "Jan";

		public string SelectedMonthFrom
		{
			get { return _selectedMonthFrom; }
			set
			{
				SetProperty(ref _selectedMonthFrom, value);

				ProduceReportCommand.RaiseCanExecuteChanged();

				DefaultDropDownLists();
			}
		}

		private string _selectedMonthTo = "Dec";

		public string SelectedMonthTo
		{
			get { return _selectedMonthTo; }
			set
			{
				SetProperty(ref _selectedMonthTo, value);

				ProduceReportCommand.RaiseCanExecuteChanged();

				DefaultDropDownLists();
			}
		}

		private string _selectedYearFrom = "2016";

		public string SelectedYearFrom
		{
			get { return _selectedYearFrom; }
			set
			{
				SetProperty(ref _selectedYearFrom, value);

				ProduceReportCommand.RaiseCanExecuteChanged();

				DefaultDropDownLists();
			}
		}

		private string _selectedYearTo = DateTime.Now.Year.ToString();

		public string SelectedYearTo
		{
			get { return _selectedYearTo; }
			set
			{
				SetProperty(ref _selectedYearTo, value);

				ProduceReportCommand.RaiseCanExecuteChanged();

				DefaultDropDownLists();
			}
		}

		private DateTime FormatSelectedDateFrom()
		{
			int day = 1;

			int month = DateTime.ParseExact(SelectedMonthFrom, "MMM", CultureInfo.CurrentCulture).Month;

			int year = Convert.ToInt16(SelectedYearFrom);

			string strDate = string.Format("{0}/{1}/{2}", day, month, year);

			return DateTime.ParseExact(strDate, "d/M/yyyy", CultureInfo.InvariantCulture);
		}

		private DateTime FormatSelectedDateTo()
		{
			int month = DateTime.ParseExact(SelectedMonthTo, "MMM", CultureInfo.CurrentCulture).Month;

			int year = Convert.ToInt16(SelectedYearTo);

			int day = DateTime.DaysInMonth(year, month);

			string strDate = string.Format("{0}/{1}/{2}", day, month, year);

			return DateTime.ParseExact(strDate, "d/M/yyyy", CultureInfo.InvariantCulture);
		}

		private ObservableCollection<string> _adjustmentCategoryList;

		public ObservableCollection<string> AdjustmentCategoryList
		{
			get
			{
				if (_adjustmentCategoryList != null)
					CollectionViewSource.GetDefaultView(_adjustmentCategoryList).Refresh();

				return _adjustmentCategoryList;
			}

			set
			{
				SetProperty(ref _adjustmentCategoryList, value);
			}
		}

		private ObservableCollection<string> _analysisTypeList;

		public ObservableCollection<string> AnalysisTypeList
		{
			get
			{
				if (_analysisTypeList != null)
					CollectionViewSource.GetDefaultView(_analysisTypeList).Refresh();

				return _analysisTypeList;
			}

			set
			{
				SetProperty(ref _analysisTypeList, value);
			}
		}

		private ObservableCollection<string> _analysisNameList;

		public ObservableCollection<string> AnalysisNameList
		{
			get
			{
				if (_analysisTypeList != null)
					CollectionViewSource.GetDefaultView(_analysisNameList).Refresh();

				return _analysisNameList;
			}

			set
			{
				SetProperty(ref _analysisNameList, value);
			}
		}

		public DelegateCommand CustomerListCheckBoxChanged { get; private set; }

		public DelegateCommand ProduceReportCommand { get; private set; }

		public DelegateCommand ExportDataCommand { get; private set; }

		public DelegateCommand ClearFiltersCommand { get; private set; }

		public DelegateCommand ToggleSelectAllCustomersCommand { get; private set; }

		public DelegateCommand<object> CommandSectorsCheckBoxChanged { get; set; }

		private IList<Guid> CurrentlySelectedCustomerGuids;

		private string ChosenCustomers = string.Empty;

		private string OverallTotalValue = string.Empty;

		public ReportsViewModel()
		{
			Initialise();
		}

		private void Initialise()
		{
			InitialiseCommands();

			InitialiseEvents();

			GetInitialCompanyList();

			GetBaseNoteSearchWords();

			HandleReportTypeChange();

			FillSectorTypesListCollection();
		}

		private void InitialiseCommands()
		{
			ProduceReportCommand = new DelegateCommand(PerformProduceReport, CanProduceReport).ObservesProperty(() => SelectedReportType);

			ExportDataCommand = new DelegateCommand(PerformExportData, CanExportReport).ObservesProperty(() => ReportEntities);

			ClearFiltersCommand = new DelegateCommand(PerformClearFilters, CanClearFilters);

			CustomerListCheckBoxChanged = new DelegateCommand(PerformCustomerListCheckBoxChanged);

			CommandSectorsCheckBoxChanged = new DelegateCommand<object>(ToggleCheckBoxSectorsChanged);

			ToggleSelectAllCustomersCommand = new DelegateCommand(ToggleSelectAllCustomers);
		}

		private void InitialiseEvents()
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));
		}

		private void FillSectorTypesListCollection()
		{
			SectorTypesListCollection = new ObservableCollection<CustomCheckBoxItem>();

			foreach (string item in Enum.GetValues(typeof(SectorTypes)).Cast<SectorTypes>().Select(x => x.GetDescriptionFromEnum()).OrderBy(x => x).ToList())
			{
				SectorTypesListCollection.Add(new CustomCheckBoxItem() { Name = item, IsSelected = false });
			}
		}

		private bool _toggleSelectAllCustomersState = false;

		public bool ToggleSelectAllCustomersState
		{
			get { return _toggleSelectAllCustomersState; }
			set
			{
				SetProperty(ref _toggleSelectAllCustomersState, value);
			}
		}

		public void ToggleSelectAllCustomers()
		{
			ToggleSelectAllCustomersState = !ToggleSelectAllCustomersState;

			// always clear down
			SectorTypesListCollection.All(x => { x.IsSelected = false; return true; });

			CustomerList.All(x => { x.IsSelected = ToggleSelectAllCustomersState; return true; });

			RaisePropertyChanged(nameof(SectorsLabelText));

			RaisePropertyChanged(nameof(CustomersLabelText));

			UpdateRecordInformationDetails();
		}

		private bool CanClearFilters()
		{
			return true;
		}

		private void PerformClearFilters()
		{
			CustomerList.Select(c => { c.IsSelected = false; return c; }).ToList();

			SectorTypesListCollection.Select(c => { c.IsSelected = false; return c; }).ToList();

			SelectedNoteSearchWord = null;

			CustomNoteSearchWordText = string.Empty;

			SelectedMonthFrom = "Jan";
			SelectedMonthTo = "Dec";

			SelectedYearFrom = "2016";
			SelectedYearTo = DateTime.Now.Year.ToString();

			PerformCustomerListCheckBoxChanged();

			RaisePropertyChanged(nameof(SectorsLabelText));
		}

		private void GetInitialCompanyList()
		{
			FullCustomerList = _reportService.GetGetCompanyNamesAndGuids().ToList();

			SetCustomerListToDefault();
		}

		private void SetCustomerListToDefault()
		{
			CustomerList = new ObservableCollection<CustomerListCheckBoxItem>(FullCustomerList.ToList());
		}

		private void DefaultDropDownLists()
		{
			AdjustmentCategoryList = null;
			AnalysisTypeList = null;
			AnalysisNameList = null;

			SelectedAdjustmentCategory = null;
			SelectedAnalysisName = null;
			SelectedAnalysisType = null;

			SelectedEntityLevel = YesNoOptions.No.GetDescriptionFromEnum();

			ReportEntities = null;

			UpdateRecordInformationDetails();
		}

		private void FilterCustomerList(string filter)
		{
			if (string.IsNullOrEmpty(filter))
			{
				SetCustomerListToDefault();
			}
			else
			{
				CustomerList = new ObservableCollection<CustomerListCheckBoxItem>(FullCustomerList.Where(x => x.Name.ToLower().StartsWith(filter.ToLower())).ToList());
			}
		}

		private void GetUsedCategoryNames()
		{
			AdjustmentCategoryList = new ObservableCollection<string>(_reportService.GetAdjustmentCategoryNames().ToList());
		}

		private void PopulateAdjustmentTypeList()
		{
			AnalysisTypeList = new ObservableCollection<string>(_reportService.GetUsedAdjustmentTypesForCategory(SelectedAdjustmentCategory).ToList());
		}

		private void GetUsedAdjustmentNames()
		{
			AnalysisNameList = new ObservableCollection<string>(_reportService.GetUsedAdjustmentNamesForAdjustmentType(SelectedAdjustmentCategory, SelectedAnalysisType).ToList());
		}

		private void GetUsedReconciliationTypes()
		{
			AnalysisTypeList = new ObservableCollection<string>(_reportService.GetUsedReconciliationTypes().ToList());
		}

		private void GetUsedReconciliationNames()
		{
			AnalysisNameList = new ObservableCollection<string>(_reportService.GetUsedReconciliationNamesForType(SelectedAnalysisType).ToList());
		}

		private void PerformCustomerListCheckBoxChanged()
		{
			SetChosenCompanyData();

			SelectedCompany = string.Empty;

			SelectedReportAnalysisType = null;

			DefaultDropDownLists();
		}

		private void SetChosenCompanyData()
		{
			ChosenCustomers = string.Join(",   + ", CustomerList.Where(x => x.IsSelected == true).Select(x => x.Name).ToArray());

			CurrentlySelectedCustomerGuids = GetSelectedCustomerGuidsList();

			_reportService.SelectedCustomerList = CurrentlySelectedCustomerGuids;

			RaisePropertyChanged(nameof(CustomersLabelText));
		}

		private string GetSelectedCustomerGuids()
		{
			IList<string> tempList = new List<string>();

			foreach (Guid item in GetSelectedCustomerGuidsList())
				tempList.Add(string.Format("\'{0}\'", item.ToString()));

			return string.Join(", ", tempList);
		}

		private IList<Guid> GetSelectedCustomerGuidsList()
		{
			return new List<Guid>(CustomerList.Where(x => x.IsSelected == true).Select(x => x.Guid).ToList());
		}

		private bool CanProduceReport()
		{
			bool canProduce = true;

			ReportType tempReportType = SelectedReportType.GetEnumFromString<ReportType>();

			switch (tempReportType)
			{
				case ReportType.Analysis:
					if (SelectedReportAnalysisType == null)
						canProduce = false;
					break;

				case ReportType.NotSet:
					canProduce = false;
					break;
			}

			return canProduce;
		}

		private bool CanExportReport()
		{
			bool tempReturn = false;

			if (ReportEntities != null)
				tempReturn = (ReportEntities.Rows.Count > 0);

			return tempReturn;
		}

		private void HandleReportTypeChange()
		{
			ReportEntities = null;

			AnalysisPanelVisibility = Visibility.Collapsed;

			NoteSearchWordVisibility = Visibility.Collapsed;

			ReportInformationLabel = string.Empty;

			SelectedReport = (ReportType)SelectedReportType.GetEnumFromString<ReportType>();

			try
			{
				switch (SelectedReport)
				{
					case ReportType.Submissions:
					case ReportType.ExpectedSubmissions:
					case ReportType.BRRSchedule:
					case ReportType.Summary:
					case ReportType.Tests:
						break;

					case ReportType.Analysis:
						AnalysisPanelVisibility = Visibility.Visible;
						break;

					case ReportType.Notes:
						NoteSearchWordVisibility = Visibility.Visible;
						break;

					default:
						SelectedReport = ReportType.NotSet;
						break;
				}
			}
			catch { }

			Debug.Print("ReportTypeChanged: " + SelectedReportType);
		}

		private void PerformProduceReport()
		{
			try
			{
				ReportType tempReportType = SelectedReportType.GetEnumFromString<ReportType>();

				if (tempReportType != ReportType.NotSet)
				{
					IsBusy = true;

					new Action(() =>
					{
						ReportEntities = null;

						PackageReportCriteria();

						ReportService tempReports = new ReportService();
						tempReports.ProduceReport(tempReportType, ReportingCriteria);

						ReportEntities = tempReports.ReportResults;

						IsBusy = false;
					}).Invoke();
				}
			}
			catch { }
		}

		private async Task<string> GetReportTypeFolderId(string reportType)
		{
			var resultDriveId = string.Empty;

			try
			{
				var folderExists = await O365Service.GraphService.GetFolderAsync(StorageFunctionality.Instance.StorageDefaults.DriveId, reportType);
				resultDriveId = folderExists.Id;
			}
			catch { }

			if (resultDriveId.IsNullOrEmpty())
			{
				var createFolder = await O365Service.GraphService.CreateRootFolderAsync(StorageFunctionality.Instance.StorageDefaults.DriveId, reportType);
				resultDriveId = createFolder.Id;
			}

			return resultDriveId;
		}

		private async void PerformExportData()
		{
			var success = false;

			if (ReportEntities != null)
			{
				if (ReportEntities.Rows.Count > 0)
				{
					//EventingCommands.RaiseEventForStatusBarMenuChange(StatusBarCaptions.Exporting);

					var reportType = SelectedReport.GetDescriptionFromEnum().Replace(" ", "");

					var reportName = $"{DateTime.Today.ToString("yyyyMMdd")} REITs {reportType} Report.xlsx";

					var repType = await GetReportTypeFolderId(reportType);

					var export = DialogBrowsers.SaveFile(StorageFunctionality.Instance.StorageDefaults.DriveId, reportType, FileFilterTypes.xlsx, reportName);

					if (export)
					{
						if (ExcelUtility.ExportRawData(ReportEntities, StorageFunctionality.Instance.CurrentWorkingFile))
						{
							await StorageFunctionality.Instance.StorageActions.Process();

							success = true;

							MessageBox.Show("Data Export Completed\n\nSite: REIT BDApp Exports", "EXPORT SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
						}
					}
				}

				if (!success)
					MessageBox.Show("Data Export Failed, please try again.", "EXPORT FAILED", MessageBoxButton.OK, MessageBoxImage.Warning);

				//EventingCommands.RaiseEventForStatusBarMenuChange(StatusBarCaptions.Idle);
			}
		}

		private void PackageReportCriteria()
		{
			SelectedCompany = string.Empty;

			ReportingCriteria = new ReportCriteria()
			{
				ReportType = SelectedReport.GetDescriptionFromEnum(),

				ChosenCompanyGuids = GetSelectedCustomerGuids(),

				StartDate = FormatSelectedDateFrom().ToString("yyyyMMdd"),
				EndDate = FormatSelectedDateTo().ToString("yyyyMMdd"),

				NoteSearchWord = CustomNoteSearchWordText,

				Version = SelectedVersion,

				AnalysisReportType = SelectedReportAnalysisType,
				AdjustmentCategory = SelectedAdjustmentCategory,
				AnalysisType = SelectedAnalysisType,
				AnalysisName = SelectedAnalysisName,
				EntityLevel = Convert.ToBoolean(BaseEnumExtension.EnumToBoolean(SelectedEntityLevel))
			};
		}

		private void UpdateRecordInformationDetails()
		{
			SetRecordsReturned();

			SetOverallTotal();

			SetReportInformation();
		}

		private void SetRecordsReturned()
		{
			RecordsReturned = string.Format("Record Count: {0}", (ReportEntities != null) ? ReportEntities.Rows.Count.ToString("D4") : 0.ToString("D4"));
		}

		private void SetOverallTotal()
		{
			var temp = 0.00;

			OverallTotalValue = string.Empty;

			try
			{
				if (ReportEntities != null)
					temp = ReportEntities.AsEnumerable().Sum(x => x.Field<double>("Amount"));

				OverallTotalValue = (ReportEntities == null) ? string.Empty : ReportEntities.AsEnumerable().Sum(x => x.Field<double>("Amount")).ToString("C0");
			}
			catch { }
		}

		private void SetReportInformation()
		{
			SetChosenCompanyData();

			if (ReportEntities == null)
			{
				ReportInformationLabel = (string.IsNullOrEmpty(ChosenCustomers)) ? string.Empty : string.Format("Customers:  {0}", ChosenCustomers);
			}
			else
			{
				ReportInformationLabel = (string.IsNullOrEmpty(ChosenCustomers)) ? string.Format("{0}   #   TotalValue: {1}", RecordsReturned, OverallTotalValue) : string.Format("Customers:  {0}   #   {1}   #   TotalValue: {2}", ChosenCustomers, RecordsReturned, OverallTotalValue);
			}
		}

		private void GetBaseNoteSearchWords()
		{
			Task.Factory.StartNew(() =>
			{
				string rootPath = ConfigurationManager.AppSettings["rootPath"].ToString();

				string source = Path.Combine(rootPath, @"NoteWordsXML\NoteSearchWords.xml");

				string destination = GetBaseWordSearchFilename();

				CompareAndCopyLatestBaseWordSearchXML(source, destination);

				ListOfNoteSearchWords = DeserializeToList(destination);
			});
		}

		private string GetBaseWordSearchFilename()
		{
			return string.Format("{0}{1}", System.IO.Path.GetTempPath(), @"NoteSearchWords.xml");
		}

		private void CompareAndCopyLatestBaseWordSearchXML(string sourceFilePath, string destinationFilePath)
		{
			bool sourceExists = System.IO.File.Exists(sourceFilePath);
			bool destinationExists = System.IO.File.Exists(destinationFilePath);
			bool shouldCopy = false;

			if (sourceExists)
			{
				if (destinationExists)
				{
					DateTime ftime = System.IO.File.GetLastWriteTime(sourceFilePath);
					DateTime ftime2 = System.IO.File.GetLastWriteTime(destinationFilePath);

					if (ftime > ftime2) // source is older so needs to be copied
						shouldCopy = true;
				}
				else
				{
					shouldCopy = true;
				}

				if (shouldCopy)
					System.IO.File.Copy(sourceFilePath, destinationFilePath, true);
			}
			else
			{
				Debug.Print(string.Format("No Source: {0}", sourceFilePath));
			}
		}

		private List<string> DeserializeToList(string fileName)
		{
			List<string> list = new List<string>();

			if (System.IO.File.Exists(fileName))
			{
				try
				{
					XElement _root = XElement.Load(fileName);

					foreach (XElement ele in _root.Descendants("Word"))
						list.Add(ele.Value);
				}
				catch (Exception ex) { Debug.Print(ex.ToString()); }
			}

			return list.OrderBy(x => x).ToList();
		}

		//

		private string FormattedSectorsLabel()
		{
			int selectedItemsCount = 0;

			if (SectorTypesListCollection != null)
				selectedItemsCount = SectorTypesListCollection.Where(x => x.IsSelected == true).Count();

			return string.Format("{0} selected", selectedItemsCount);
		}

		private string FormattedCustomersLabel()
		{
			int selectedItemsCount = 0;

			if (CustomerList != null)
				selectedItemsCount = CustomerList.Where(x => x.IsSelected == true).Count();

			return string.Format("{0} selected", selectedItemsCount);
		}

		private void ToggleCheckBoxSectorsChanged(object param)
		{
			CustomerList.All(x => { x.IsSelected = false; return true; });

			foreach (CustomerListCheckBoxItem customer in CustomerList)
			{
				SectorTypes customerFlags = (SectorTypes)customer.SectorsFlag;

				foreach (CustomCheckBoxItem currentSector in SectorTypesListCollection.Where(x => x.IsSelected == true))
				{
					SectorTypes currentSectorFlag = currentSector.Name.GetEnumFromString<SectorTypes>();

					if (customerFlags.HasFlag(currentSectorFlag))
					{
						customer.IsSelected = true;
						break;
					}
				}
			}

			PerformCustomerListCheckBoxChanged();

			RaisePropertyChanged(nameof(SectorsLabelText));

			RaisePropertyChanged(nameof(CustomerList));
		}

		// NAVIGATION TO PROPS
		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Report", true));
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Report", false));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Print", false));

			// CLEAR SELECTED COMPANY CHECKBOXES
		}
	}
}
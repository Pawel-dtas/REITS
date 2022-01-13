using Domain;
using Domain.Enums;
using Domain.MessageBoxModelsEnums;
using Domain.Models;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using REITs.DataLayer.Services;
using REITs.Domain.MenuModels;
using REITs.Domain.Models;
using REITs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace REITs.SearchModule.ViewModels
{
	public class SearchViewModel : BindableBase, INavigationAware
	{
		#region Constructor

		public SearchViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IUnityContainer unityContainer, IREITDataService dataService, ISearchService searchService)
		{
			_eventAggregator = eventAggregator;
			_regionManager = regionManager;
			_unityContainer = unityContainer;
			_searchService = searchService;
			_dataService = dataService;

			Initialise();
		}

		private void Initialise()
		{
			SearchCommand = new DelegateCommand(OnSearchExecute, CanExecuteSearch);
			ClearCommand = new DelegateCommand(OnClearExecute, CanExecuteClear);
			OpenRecordCommand = new DelegateCommand(OnOpenRecordExecute, CanExecuteOpenRecord);
		}

		#endregion Constructor

		#region Properties

		//*** Display Properties ***
		public List<string> SearchTypesList
		{
			get
			{
				return BaseEnumExtension.GetEnumDescriptions<SearchTypes>();
			}
		}

		public string SelectedSearchType
		{
			get { return _selectdSearchType; }
			set
			{
				SetProperty(ref _selectdSearchType, value);

				ConfigureSearchDisplay();

				SearchResultList = null;

				SearchCommand.RaiseCanExecuteChanged();
				ClearCommand.RaiseCanExecuteChanged();
			}
		}

		private void ConfigureSearchDisplay()
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITParentAdd", false));

			SetSearchLabels();
			EnableDisableSearchFields();
			SetUpListView();
		}

		public string SearchNameLabel
		{
			get { return _searchNameLabel; }
			set
			{
				SetProperty(ref _searchNameLabel, value);
			}
		}

		public string SearchUTRLabel
		{
			get { return _searchUTRLabel; }
			set
			{
				SetProperty(ref _searchUTRLabel, value);
			}
		}

		//*** Search Model Properties ***

		public string SearchName
		{
			get { return _searchName; }
			set
			{
				SetProperty(ref _searchName, value);
				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
			}
		}

		public string SearchUTR
		{
			get { return _searchUTR; }
			set
			{
				SetProperty(ref _searchUTR, value);
				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
			}
		}

		public string SearchCustomerReference
		{
			get { return _searchCustomerReference; }
			set
			{
				SetProperty(ref _searchCustomerReference, value);
				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
			}
		}

		//public DateTime? SearchAPEFrom
		//{
		//	get { return _searchAPEFrom; }
		//	set
		//	{
		//		SetProperty(ref _searchAPEFrom, value);
		//		ClearCommand.RaiseCanExecuteChanged();
		//		SearchCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public DateTime? SearchAPETo
		//{
		//	get { return _searchAPETo; }
		//	set
		//	{
		//		SetProperty(ref _searchAPETo, value);
		//		ClearCommand.RaiseCanExecuteChanged();
		//		SearchCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public DateTime? SearchPAPEFrom
		//{
		//	get { return _searchPAPEFrom; }
		//	set
		//	{
		//		SetProperty(ref _searchPAPEFrom, value);
		//		ClearCommand.RaiseCanExecuteChanged();
		//		SearchCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//public DateTime? SearchPAPETo
		//{
		//	get { return _searchPAPETo; }
		//	set
		//	{
		//		SetProperty(ref _searchPAPETo, value);
		//		ClearCommand.RaiseCanExecuteChanged();
		//		SearchCommand.RaiseCanExecuteChanged();
		//	}
		//}

		//*** Search Results Properties ***

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

				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
			}
		}

		private string _selectedMonthTo = "Dec";

		public string SelectedMonthTo
		{
			get { return _selectedMonthTo; }
			set
			{
				SetProperty(ref _selectedMonthTo, value);

				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
			}
		}

		private string _selectedYearFrom = "2016";

		public string SelectedYearFrom
		{
			get { return _selectedYearFrom; }
			set
			{
				SetProperty(ref _selectedYearFrom, value);

				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
			}
		}

		private string _selectedYearTo = DateTime.Now.Year.ToString();

		public string SelectedYearTo
		{
			get { return _selectedYearTo; }
			set
			{
				SetProperty(ref _selectedYearTo, value);

				ClearCommand.RaiseCanExecuteChanged();
				SearchCommand.RaiseCanExecuteChanged();
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

		public ObservableCollection<SearchResultModel> SearchResultList
		{
			get { return _searchResultList; }
			set
			{
				SetProperty(ref _searchResultList, value);
			}
		}

		public SearchResultModel SelectedSearchResultItem
		{
			get { return _selectedSearchResultItem; }
			set
			{
				SetProperty(ref _selectedSearchResultItem, value);
				OpenRecordCommand.RaiseCanExecuteChanged();
			}
		}

		public REIT SelectedREITRecord
		{
			get;
			set;
		}

		public REITParent SelectedREITParentRecord
		{
			get;
			set;
		}

		public REITTotals SelectedREITTotalsRecord
		{
			get;
			set;
		}

		public REIT SelectedEntityRecord
		{
			get;
			set;
		}

		//*** Column Width Properties ***

		public bool IsSearchNameEnabled
		{
			get; set;
		}

		public bool IsSearchUTREnabled
		{
			get; set;
		}

		public bool IsSearchCustomerReferenceEnabled
		{
			get; set;
		}

		public bool IsSearchAPEFromEnabled
		{
			get; set;
		}

		public bool IsSearchAPEToEnabled
		{
			get; set;
		}

		public bool IsSearchPAPEFromEnabled
		{
			get; set;
		}

		public bool IsSearchPAPEToEnabled
		{
			get; set;
		}

		#endregion Properties

		#region Variables

		private IEventAggregator _eventAggregator;
		private IRegionManager _regionManager;
		private IUnityContainer _unityContainer;
		private ISearchService _searchService;
		private IREITDataService _dataService;

		private SearchResultModel _selectedSearchResultItem;
		private SearchResultModel _selectedSearchResultDisplayModel;
		private ObservableCollection<SearchResultModel> _searchResultList;

		private string _searchType;
		private string _searchName;
		private string _searchUTR;
		private string _searchCustomerReference;
		//private DateTime? _searchAPEFrom;
		//private DateTime? _searchAPETo;
		//private DateTime? _searchPAPEFrom;
		//private DateTime? _searchPAPETo;

		private string _selectdSearchType;
		private string _searchNameLabel;
		private string _searchUTRLabel;

		private ColumnConfig _currentSearchViewColumns;

		public ColumnConfig CurrentSearchViewColumns
		{
			get { return _currentSearchViewColumns; }
			set
			{
				SetProperty(ref _currentSearchViewColumns, value);
			}
		}

		#endregion Variables

		#region Delegate Commands

		public DelegateCommand SearchCommand { get; private set; }
		public DelegateCommand ClearCommand { get; private set; }
		public DelegateCommand OpenRecordCommand { get; private set; }

		#endregion Delegate Commands

		#region Commands

		private void OnSearchExecute()
		{
			System.Windows.Application.Current.MainWindow.Cursor = Cursors.Wait;

			Search();

			System.Windows.Application.Current.MainWindow.Cursor = Cursors.Arrow;
		}

		private void OnClearExecute()
		{
			ClearSearchFields();
		}

		private void OnOpenRecordExecute()
		{
			SelectRecordToOpen();
		}

		#endregion Commands

		#region Validation

		private bool CanExecuteSearch()
		{
			return CheckSearchFieldsComplete(true);
		}

		private bool CanExecuteClear()
		{
			return CheckSearchFields();
		}

		private bool CanExecuteOpenRecord()
		{
			return SelectedSearchResultItem != null;
		}

		#endregion Validation

		#region Private Methods

		private void SelectRecordToOpen()
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITParentAdd", false));

			SearchTypes selectedSearch = SelectedSearchType.GetEnumFromString<SearchTypes>();

			switch (selectedSearch)
			{
				case SearchTypes.REIT:
					OpenREITRecord(SelectedSearchResultItem);
					break;

				case SearchTypes.REITParent:
					OpenREITParentRecord(SelectedSearchResultItem);
					break;

				case SearchTypes.Entity:
					OpenEntityRecord(SelectedSearchResultItem);
					break;
			}
		}

		private void OpenREITRecord(SearchResultModel rEITRecord)
		{
			Guid recordGuid = rEITRecord.Id;

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemClickedEvent>().Publish(new MenuItemClickedEventPayload("REIT", recordGuid));
		}

		private void OpenREITParentRecord(SearchResultModel rEITRecord)
		{
			Guid recordGuid = rEITRecord.Id;

			NavigationParameters navParameter = new NavigationParameters();
			navParameter.Add("SearchResult", true);
			navParameter.Add("ResultType", "Parent");

			navParameter.Add("REITParentRecord", recordGuid);

			_regionManager.RequestNavigate("ContentRegion", "REITParentView", navParameter);
		}

		private void OpenEntityRecord(SearchResultModel entityRecord)
		{
			Guid recordGuid = entityRecord.REITId;

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemClickedEvent>().Publish(new MenuItemClickedEventPayload("REIT", recordGuid));
		}

		private void Search()
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITParentAdd", false));

			SearchOptionModel searchOptions = GetSearchOptions();

			SearchTypes selectedSearchType = searchOptions.SearchType.GetEnumFromString<SearchTypes>();

			switch (selectedSearchType)
			{
				case SearchTypes.REIT:
					LoadSearchResults<REIT>(_searchService.GetREITSearchResults(searchOptions));
					break;

				case SearchTypes.REITParent:
					LoadSearchResults<REITParent>(_searchService.GetREITParentSearchResults(searchOptions));
					break;

				case SearchTypes.Entity:
					LoadSearchResults<Entity>(_searchService.GetEntitySearchResults(searchOptions));
					break;
			}

			if (SearchResultList.Count == 0)
			{
				CustomMessageBox.Show(MessageBoxContentTypes.NoSearchResults);

				if (selectedSearchType == SearchTypes.REITParent)
					PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITParentAdd", true));
			}
		}

		private bool CheckSearchFieldsComplete(bool option)
		{
			bool fieldsComplete = false;

			if (option != false)
			{
				if (SelectedSearchType != null)
				{
					fieldsComplete = true;
				}
			}
			if (!string.IsNullOrEmpty(SearchName))
			{
				fieldsComplete = true;
			}
			if (!string.IsNullOrEmpty(SearchUTR))
			{
				fieldsComplete = true;
			}
			if (!string.IsNullOrEmpty(SearchCustomerReference))
			{
				fieldsComplete = true;
			}

			//if (SearchAPEFrom != null)
			//{
			//	fieldsComplete = true;
			//}
			//if (SearchAPETo != null)
			//{
			//	fieldsComplete = true;
			//}
			//if (SearchPAPEFrom != null)
			//{
			//	fieldsComplete = true;
			//}
			//if (SearchPAPETo != null)
			//{
			//	fieldsComplete = true;
			//}

			return fieldsComplete;
		}

		private bool CheckSearchFields()
		{
			bool result = false;

			if (SelectedSearchType != null)
			{
				result = true;
			}
			if (SearchName != null)
			{
				result = true;
			}
			if (SearchUTR != null)
			{
				result = true;
			}
			if (SearchCustomerReference != null)
			{
				result = true;
			}

			//if (SearchAPEFrom != null)
			//{
			//	result = true;
			//}
			//if (SearchAPETo != null)
			//{
			//	result = true;
			//}
			//if (SearchPAPEFrom != null)
			//{
			//	result = true;
			//}
			//if (SearchPAPETo != null)
			//{
			//	result = true;
			//}

			return result;
		}

		private void ClearSearchFields()
		{
			SelectedSearchType = null;
			SearchName = null;
			SearchUTR = null;
			SearchCustomerReference = null;

			SearchNameLabel = string.Empty;
			SearchUTRLabel = string.Empty;

			SelectedMonthFrom = "Jan";
			SelectedMonthTo = "Dec";

			SelectedYearFrom = "2016";
			SelectedYearTo = DateTime.Now.Year.ToString();

			//SearchAPEFrom = null;
			//SearchAPETo = null;
			//SearchPAPEFrom = null;
			//SearchPAPETo = null;

			if (SearchResultList != null)
			{
				SearchResultList.Clear();
			}
		}

		private void SetSearchLabels()
		{
			SearchTypes tempSearchType = SelectedSearchType.GetEnumFromString<SearchTypes>();

			switch (tempSearchType)
			{
				case SearchTypes.REIT:
					SearchNameLabel = "REIT Name";
					SearchUTRLabel = "Principal UTR";
					break;

				case SearchTypes.REITParent:
					SearchNameLabel = "Parent Name";
					SearchUTRLabel = "Parent UTR";
					break;

				case SearchTypes.Entity:
					SearchNameLabel = "Entity Name";
					SearchUTRLabel = "Entity UTR";
					break;
			}

			RaisePropertyChanged("SearchNameLabel");
			RaisePropertyChanged("SearchUTRLabel");
		}

		private SearchOptionModel GetSearchOptions()
		{
			SearchOptionModel tempSearchOptions = new SearchOptionModel();

			if (!string.IsNullOrEmpty(SelectedSearchType))
				tempSearchOptions.SearchType = SelectedSearchType;

			if (!string.IsNullOrEmpty(SearchName))
				tempSearchOptions.SearchName = SearchName;

			if (!string.IsNullOrEmpty(SearchUTR))
				tempSearchOptions.SearchUTR = SearchUTR;

			if (!string.IsNullOrEmpty(SearchCustomerReference))
				tempSearchOptions.SearchCustomerReference = SearchCustomerReference;

			//if (SearchAPEFrom != null)
			tempSearchOptions.SearchAPEFrom = FormatSelectedDateFrom();

			//if (SearchAPETo != null)
			tempSearchOptions.SearchAPETo = FormatSelectedDateTo();

			//if (SearchPAPEFrom != null)
			//	tempSearchOptions.SearchPAPEFrom = SearchPAPEFrom;

			//if (SearchPAPETo != null)
			//	tempSearchOptions.SearchPAPETo = SearchPAPETo;

			return tempSearchOptions;
		}

		private void LoadSearchResults<T>(ICollection<T> searchResultList)
		{
			List<SearchResultModel> searchResultsDisplayModelList = new List<SearchResultModel>();

			foreach (T record in searchResultList)
			{
				SearchResultModel tempSearchDisplayResult = new SearchResultModel();

				foreach (PropertyInfo propInfo in tempSearchDisplayResult.GetType().GetProperties())
				{
					try
					{
						if (record.GetType().GetProperty(propInfo.Name) != null)
						{
							var propValue = record.GetType().GetProperty(propInfo.Name).GetValue(record, null);

							propInfo.SetValue(tempSearchDisplayResult, propValue);

							if (propInfo.Name == "APEDate")
							{
								// sort by APE Only + override year
								DateTime dt;
								if (DateTime.TryParse(propValue.ToString(), out dt))
								{
									DateTime newDate = new DateTime(1900, dt.Month, dt.Day);
									propInfo.SetValue(tempSearchDisplayResult, newDate);
								}
							}

							// REIT LEVEL Specific to Entity

							if (record is Entity)
							{
								if (propInfo.Name == "Id")  // only do it once per entity
								{
									Entity entityObj = record as Entity;
									tempSearchDisplayResult.AccountPeriodEnd = entityObj.REIT.AccountPeriodEnd;
								}
							}
						}
					}
					catch (Exception ex) { Debug.Print("Property not set: " + propInfo.Name + " - " + ex.Message.ToString()); }
				}

				searchResultsDisplayModelList.Add(tempSearchDisplayResult);
			}

			SearchResultList = new ObservableCollection<SearchResultModel>(searchResultsDisplayModelList);

			SetUpListView();
		}

		private void SetUpListView()
		{
			SearchTypes chosenSearchType = SelectedSearchType.GetEnumFromString<SearchTypes>();

			switch (chosenSearchType)
			{
				case SearchTypes.REIT:
					CurrentSearchViewColumns = REITSearchColumns;
					break;

				case SearchTypes.REITParent:
					CurrentSearchViewColumns = REITParentSearchColumns;
					break;

				case SearchTypes.Entity:
					CurrentSearchViewColumns = EntitySearchColumns;
					break;
			}

			RaisePropertyChanged(nameof(SearchResultList));
			RaisePropertyChanged(nameof(CurrentSearchViewColumns));
		}

		private void EnableDisableSearchFields()
		{
			if (!string.IsNullOrEmpty(SelectedSearchType))
			{
				if (SelectedSearchType == BaseEnumExtension.GetDescriptionFromEnum(SearchTypes.REIT))
				{
					EnableREITFields();
				}
				else
				{
					EnableEntityFields();
				}
			}
			else
			{
				DisableAllSearchFields(true);
			}
			RaisePropertyChanges();
		}

		private void EnableREITFields()
		{
			IsSearchNameEnabled = true;
			IsSearchUTREnabled = true;
			IsSearchCustomerReferenceEnabled = false;
			IsSearchAPEFromEnabled = true;
			IsSearchAPEToEnabled = true;
			IsSearchPAPEFromEnabled = true;
			IsSearchPAPEToEnabled = true;
		}

		private void EnableEntityFields()
		{
			IsSearchNameEnabled = true;
			IsSearchUTREnabled = true;
			IsSearchCustomerReferenceEnabled = true;
			IsSearchAPEFromEnabled = false;
			IsSearchAPEToEnabled = false;
			IsSearchPAPEFromEnabled = false;
			IsSearchPAPEToEnabled = false;
		}

		private void DisableAllSearchFields(bool option)
		{
			IsSearchNameEnabled = option;
			IsSearchUTREnabled = option;
			IsSearchCustomerReferenceEnabled = option;
			IsSearchAPEFromEnabled = option;
			IsSearchAPEToEnabled = option;
			IsSearchPAPEFromEnabled = option;
			IsSearchPAPEToEnabled = option;
		}

		private void RaisePropertyChanges()
		{
			RaisePropertyChanged("IsSearchNameEnabled");
			RaisePropertyChanged("IsSearchUTREnabled");
			RaisePropertyChanged("IsSearchCustomerReferenceEnabled");
			RaisePropertyChanged("IsSearchAPEFromEnabled");
			RaisePropertyChanged("IsSearchAPEToEnabled");
			RaisePropertyChanged("IsSearchPAPEFromEnabled");
			RaisePropertyChanged("IsSearchPAPEToEnabled");
		}

		//private void ResetSearchFields()
		//{
		//    if(CheckSearchFieldsComplete(false)==true)
		//    {
		//        if(CustomMessageBox.Show(MessageContent.GetMessageContent(MessageBoxContentTypes.SearchOptionsResetWarning)) == MessageBoxResult.Yes)
		//        {
		//        ClearSearchFields();
		//        }
		//}
		//}

		private ColumnConfig REITSearchColumns = new ColumnConfig
		{
			Columns = new List<Column> {
							new Column { Header = "APE", DataField = "AccountPeriodEnd", Width = 100, IsDate = true},
							new Column { Header = "REIT Name", DataField = "REITName", Width = 400},
							new Column { Header = "Principal UTR", DataField = "PrincipalUTR", Width = 100}
			}
		};

		private ColumnConfig REITParentSearchColumns = new ColumnConfig
		{
			Columns = new List<Column> {
							new Column { Header = "Customer", DataField = "PrincipalCustomerName", Width = 400},
							new Column { Header = "Principal UTR", DataField = "PrincipalUTR", Width = 100},
							new Column { Header = "APE", DataField = "APEDate", Width = 100, IsAPE = true},
							new Column { Header = "Sectors", DataField = "SectorsFlag", Width = 900, IsEnumFlag = true}
			}
		};

		private ColumnConfig EntitySearchColumns = new ColumnConfig
		{
			Columns = new List<Column> {
						new Column { Header = "APE", DataField = "AccountPeriodEnd", Width = 100, IsDate = true},
							new Column { Header = "Entity Name", DataField = "EntityName", Width = 400},
							new Column { Header = "Entity Type", DataField = "EntityType", Width = 150},
							new Column { Header = "UTR", DataField = "EntityUTR", Width = 100},
							new Column { Header = "CustomerRef", DataField = "CustomerReference", Width = 100},
							new Column { Header = "%", DataField = "InterestPercentage", Width = 40},
							new Column { Header = "Jurisdiction", DataField = "Jurisdiction", Width = 150},
							new Column { Header = "Tax-Exempt", DataField = "TaxExempt", Width = 150},
			}
		};

		#endregion Private Methods

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", false));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Print", false));
		}
	}
}
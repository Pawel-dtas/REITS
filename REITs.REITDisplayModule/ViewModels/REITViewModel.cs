using Domain;
using Domain.Enums;
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace REITs.REITDisplayModule.ViewModels
{
	public class REITViewModel : BindableBase, INavigationAware
	{
		#region Constructor

		public DelegateCommand ToggleSubmissionDateEdit { get; private set; }

		public REITViewModel()
		{
			Initialise();
		}

		private void Initialise()
		{
			_dataService = PrismHelpers.ResolveService<IREITDataService>();

			PDSVisibility = Visibility.Hidden;

			SDUVisibility = Visibility.Hidden;

			ToggleSubmissionDateEdit = new DelegateCommand(performToggleSubmissionDateEdit);
		}

		private void performToggleSubmissionDateEdit()
		{
			DateSubmittedIsReadOnly = !DateSubmittedIsReadOnly;

			if (DateSubmitted != OriginalDateSubmitted)
				CallUpdateREITSubmissionDate();
		}

		#endregion Constructor

		#region Properties

		//Header Properties
		public string REITName
		{
			get { return _rEITName; }
			set { SetProperty(ref _rEITName, value); }
		}

		public string PrincipalUTR
		{
			get { return _principalUTR; }
			set { SetProperty(ref _principalUTR, value); }
		}

		public string Notes
		{
			get { return _notes; }
			set { SetProperty(ref _notes, value); }
		}

		public string APE
		{
			get { return _aPE; }
			set { SetProperty(ref _aPE, value); }
		}

		public string PAPE
		{
			get { return _pAPE; }
			set { SetProperty(ref _pAPE, value); }
		}

		public string Version
		{
			get { return _version; }
			set { SetProperty(ref _version, value); }
		}

		public string DateSubmitted
		{
			get { return _dateSubmitted; }
			set
			{
				SetProperty(ref _dateSubmitted, value);

				if (!DateSubmittedIsReadOnly)
					HandleSubmissionDateChange();
			}
		}

		public string _originalDateSubmitted;

		public string OriginalDateSubmitted
		{
			get { return _originalDateSubmitted; }
			set { SetProperty(ref _originalDateSubmitted, value); }
		}

		private void HandleSubmissionDateChange()
		{
			string dateFormat = "dd/MM/yyyy hh:mm:ss";

			try
			{
				if (DateSubmitted.Length == 10)
					DateSubmitted = String.Format("{0} 12:00:00", DateSubmitted);

				_currentREIT.XMLDateSubmitted = DateTime.ParseExact(DateSubmitted, dateFormat, DateTimeFormatInfo.InvariantInfo);

				ToggleSubmissionDateEditIsEnabled = true;
			}
			catch
			{
				ToggleSubmissionDateEditIsEnabled = false;
			}
		}

		public bool _dateSubmittedIsReadOnly = true;

		public bool DateSubmittedIsReadOnly
		{
			get { return _dateSubmittedIsReadOnly; }
			set { SetProperty(ref _dateSubmittedIsReadOnly, value); }
		}

		public bool _toggleSubmissionDateEditIsEnabled = true;

		public bool ToggleSubmissionDateEditIsEnabled
		{
			get { return _toggleSubmissionDateEditIsEnabled; }
			set { SetProperty(ref _toggleSubmissionDateEditIsEnabled, value); }
		}

		public string CreatedBy
		{
			get { return _createdBy; }
			set { SetProperty(ref _createdBy, value); }
		}

		public string DateCreated
		{
			get { return _dateCreated; }
			set { SetProperty(ref _dateCreated, value); }
		}

		//Test Properties
		public double IncomeTestPercentage
		{
			get { return _incomeTestPercentage; }
			set { SetProperty(ref _incomeTestPercentage, value); }
		}

		public string IncomeTestResult
		{
			get { return _incomeTestResult; }
			set { SetProperty(ref _incomeTestResult, value); }
		}

		public double AssetTestPercentage
		{
			get { return _assetTestPercentage; }
			set { SetProperty(ref _assetTestPercentage, value); }
		}

		public string AssetTestResult
		{
			get { return _assetTestResult; }
			set { SetProperty(ref _assetTestResult, value); }
		}

		public double PidDist90Amount
		{
			get { return _pidDist90Amount; }
			set { SetProperty(ref _pidDist90Amount, value); }
		}

		public double PidDist100Amount
		{
			get { return _pidDist100Amount; }
			set { SetProperty(ref _pidDist100Amount, value); }
		}

		public double InterestCoverAmount
		{
			get { return _interestCoverAmount; }
			set { SetProperty(ref _interestCoverAmount, value); }
		}

		public string InterestCoverRatio
		{
			get { return _interestCoverRatio; }
			set { SetProperty(ref _interestCoverRatio, value); }
		}

		public string InterestCoverResult
		{
			get { return _interestCoverResult; }
			set { SetProperty(ref _interestCoverResult, value); }
		}

		public string PaidDividendScheduleConfirmed
		{
			get { return _paidDividendScheduleConfirmed; }
			set
			{
				SetProperty(ref _paidDividendScheduleConfirmed, value);

				HandleDividendValueChange(value);
			}
		}

		private void HandleDividendValueChange(string value)
		{
			if (_currentREITTotals.PaidDividendScheduleConfirmed != value)
				CallPIDScheduleChanged();
		}

		public Visibility PDSVisibility
		{
			get { return _pdsVisibility; }
			set
			{
				SetProperty(ref _pdsVisibility, value);
			}
		}

		public Visibility SDUVisibility
		{
			get { return _sduVisibility; }
			set
			{
				SetProperty(ref _sduVisibility, value);
			}
		}

		//Property Combined Total
		public double PropertyCombinedRevenueLCT
		{
			get { return _propertyCombinedRevenueLCT; }
			set { SetProperty(ref _propertyCombinedRevenueLCT, value); }
		}

		public double PropertyCombinedPIDs
		{
			get { return _propertyCombinedPIDs; }
			set { SetProperty(ref _propertyCombinedPIDs, value); }
		}

		public double PropertyCombinedOtherIncomeTotal
		{
			get { return _propertyCombinedOtherIncomeTotal; }
			set { SetProperty(ref _propertyCombinedOtherIncomeTotal, value); }
		}

		public double PropertyCombinedNetFinanceCosts
		{
			get { return _propertyCombinedNetFinanceCosts; }
			set { SetProperty(ref _propertyCombinedNetFinanceCosts, value); }
		}

		public double PropertyCombinedOtherAdjustmentsTotal
		{
			get { return _propertyCombinedOtherAdjustmentsTotal; }
			set { SetProperty(ref _propertyCombinedOtherAdjustmentsTotal, value); }
		}

		public double PropertyCombinedNonCurrentAssets
		{
			get { return _propertyCombinedNonCurrentAssets; }
			set { SetProperty(ref _propertyCombinedNonCurrentAssets, value); }
		}

		public double PropertyCombinedCurrentAssets
		{
			get { return _propertyCombinedCurrentAssets; }
			set { SetProperty(ref _propertyCombinedCurrentAssets, value); }
		}

		public double PropertyCombinedGrandTotal
		{
			get { return _propertyCombinedGrandTotal; }
			set { SetProperty(ref _propertyCombinedGrandTotal, value); }
		}

		public ObservableCollection<Adjustment> PropertyCombinedIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> PropertyCombinedOtherAdjustments
		{
			get; set;
		}

		//Property UK Entity Totals
		public double PropertyUKRevenueLCT
		{
			get { return _propertyUKRevenueLCT; }
			set { SetProperty(ref _propertyUKRevenueLCT, value); }
		}

		public double PropertyUKPIDs
		{
			get { return _propertyUKPIDs; }
			set { SetProperty(ref _propertyUKPIDs, value); }
		}

		public double PropertyUKOtherIncomeTotal
		{
			get { return _propertyUKOtherIncomeTotal; }
			set { SetProperty(ref _propertyUKOtherIncomeTotal, value); }
		}

		public double PropertyUKNetFinanceCosts
		{
			get { return _propertyUKNetFinanceCosts; }
			set { SetProperty(ref _propertyUKNetFinanceCosts, value); }
		}

		public double PropertyUKOtherAdjustmentsTotal
		{
			get { return _propertyUKOtherAdjustmentsTotal; }
			set { SetProperty(ref _propertyUKOtherAdjustmentsTotal, value); }
		}

		public double PropertyUKNonCurrentAssets
		{
			get { return _propertyUKNonCurrentAssets; }
			set { SetProperty(ref _propertyUKNonCurrentAssets, value); }
		}

		public double PropertyUKCurrentAssets
		{
			get { return _propertyUKCurrentAssets; }
			set { SetProperty(ref _propertyUKCurrentAssets, value); }
		}

		public double PropertyUKGrandTotal
		{
			get { return _propertyUKGrandTotal; }
			set { SetProperty(ref _propertyUKGrandTotal, value); }
		}

		public ObservableCollection<Adjustment> PropertyUKIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> PropertyUKOtherAdjustments
		{
			get; set;
		}

		//Property Non UK Entity Totals
		public double PropertyNonUKRevenueLCT
		{
			get { return _propertyNonUKRevenueLCT; }
			set { SetProperty(ref _propertyNonUKRevenueLCT, value); }
		}

		public double PropertyNonUKPIDs
		{
			get { return _propertyNonUKPIDs; }
			set { SetProperty(ref _propertyNonUKPIDs, value); }
		}

		public double PropertyNonUKOtherIncomeTotal
		{
			get { return _propertyNonUKOtherIncomeTotal; }
			set { SetProperty(ref _propertyNonUKOtherIncomeTotal, value); }
		}

		public double PropertyNonUKNetFinanceCosts
		{
			get { return _propertyNonUKNetFinanceCosts; }
			set { SetProperty(ref _propertyNonUKNetFinanceCosts, value); }
		}

		public double PropertyNonUKOtherAdjustmentsTotal
		{
			get { return _propertyNonUKOtherAdjustmentsTotal; }
			set { SetProperty(ref _propertyNonUKOtherAdjustmentsTotal, value); }
		}

		public double PropertyNonUKNonCurrentAssets
		{
			get { return _propertyNonUKNonCurrentAssets; }
			set { SetProperty(ref _propertyNonUKNonCurrentAssets, value); }
		}

		public double PropertyNonUKCurrentAssets
		{
			get { return _propertyNonUKCurrentAssets; }
			set { SetProperty(ref _propertyNonUKCurrentAssets, value); }
		}

		public double PropertyNonUKGrandTotal
		{
			get { return _propertyNonUKGrandTotal; }
			set { SetProperty(ref _propertyNonUKGrandTotal, value); }
		}

		public ObservableCollection<Adjustment> PropertyNonUKIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> PropertyNonUKOtherAdjustments
		{
			get; set;
		}

		//Residual Combined Total
		public double ResidualCombinedRevenueLCT
		{
			get { return _residualCombinedRevenueLCT; }
			set { SetProperty(ref _residualCombinedRevenueLCT, value); }
		}

		public double ResidualCombinedBeneficialInterests
		{
			get { return _residualCombinedBeneficialInterests; }
			set { SetProperty(ref _residualCombinedBeneficialInterests, value); }
		}

		public double ResidualCombinedOtherIncomeTotal
		{
			get { return _residualCombinedOtherIncomeTotal; }
			set { SetProperty(ref _residualCombinedOtherIncomeTotal, value); }
		}

		public double ResidualCombinedNetFinanceCosts
		{
			get { return _residualCombinedNetFinanceCosts; }
			set { SetProperty(ref _residualCombinedNetFinanceCosts, value); }
		}

		public double ResidualCombinedOtherAdjustmentsTotal
		{
			get { return _residualCombinedOtherAdjustmentsTotal; }
			set { SetProperty(ref _residualCombinedOtherAdjustmentsTotal, value); }
		}

		public double ResidualCombinedNonCurrentAssets
		{
			get { return _residualCombinedNonCurrentAssets; }
			set { SetProperty(ref _residualCombinedNonCurrentAssets, value); }
		}

		public double ResidualCombinedCurrentAssets
		{
			get { return _residualCombinedCurrentAssets; }
			set { SetProperty(ref _residualCombinedCurrentAssets, value); }
		}

		public double ResidualCombinedGrandTotal
		{
			get { return _residualCombinedGrandTotal; }
			set { SetProperty(ref _residualCombinedGrandTotal, value); }
		}

		public ObservableCollection<Adjustment> ResidualCombinedIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> ResidualCombinedOtherAdjustments
		{
			get; set;
		}

		//Residual UK Entity Totals
		public double ResidualUKRevenueLCT
		{
			get { return _residualUKRevenueLCT; }
			set { SetProperty(ref _residualUKRevenueLCT, value); }
		}

		public double ResidualUKBeneficialInterests
		{
			get { return _residualUKBeneficialInterests; }
			set { SetProperty(ref _residualUKBeneficialInterests, value); }
		}

		public double ResidualUKOtherIncomeTotal
		{
			get { return _residualUKOtherIncomeTotal; }
			set { SetProperty(ref _residualUKOtherIncomeTotal, value); }
		}

		public double ResidualUKNetFinanceCosts
		{
			get { return _residualUKNetFinanceCosts; }
			set { SetProperty(ref _residualUKNetFinanceCosts, value); }
		}

		public double ResidualUKOtherAdjustmentsTotal
		{
			get { return _residualUKOtherAdjustmentsTotal; }
			set { SetProperty(ref _residualUKOtherAdjustmentsTotal, value); }
		}

		public double ResidualUKNonCurrentAssets
		{
			get { return _residualUKNonCurrentAssets; }
			set { SetProperty(ref _residualUKNonCurrentAssets, value); }
		}

		public double ResidualUKCurrentAssets
		{
			get { return _residualUKCurrentAssets; }
			set { SetProperty(ref _residualUKCurrentAssets, value); }
		}

		public double ResidualUKGrandTotal
		{
			get { return _residualUKGrandTotal; }
			set { SetProperty(ref _residualUKGrandTotal, value); }
		}

		public ObservableCollection<Adjustment> ResidualUKIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> ResidualUKOtherAdjustments
		{
			get; set;
		}

		//Residual Non UK Entity Totals
		public double ResidualNonUKRevenueLCT
		{
			get { return _residualNonUKRevenueLCT; }
			set { SetProperty(ref _residualNonUKRevenueLCT, value); }
		}

		public double ResidualNonUKBeneficialInterests
		{
			get { return _residualNonUKBeneficialInterests; }
			set { SetProperty(ref _residualNonUKBeneficialInterests, value); }
		}

		public double ResidualNonUKOtherIncomeTotal
		{
			get { return _residualNonUKOtherIncomeTotal; }
			set { SetProperty(ref _residualNonUKOtherIncomeTotal, value); }
		}

		public double ResidualNonUKNetFinanceCosts
		{
			get { return _residualNonUKNetFinanceCosts; }
			set { SetProperty(ref _residualNonUKNetFinanceCosts, value); }
		}

		public double ResidualNonUKOtherAdjustmentsTotal
		{
			get { return _residualNonUKOtherAdjustmentsTotal; }
			set { SetProperty(ref _residualNonUKOtherAdjustmentsTotal, value); }
		}

		public double ResidualNonUKNonCurrentAssets
		{
			get { return _residualNonUKNonCurrentAssets; }
			set { SetProperty(ref _residualNonUKNonCurrentAssets, value); }
		}

		public double ResidualNonUKCurrentAssets
		{
			get { return _residualNonUKCurrentAssets; }
			set { SetProperty(ref _residualNonUKCurrentAssets, value); }
		}

		public double ResidualNonUKGrandTotal
		{
			get { return _residualNonUKGrandTotal; }
			set { SetProperty(ref _residualNonUKGrandTotal, value); }
		}

		public ObservableCollection<Adjustment> ResidualNonUKIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> ResidualNonUKOtherAdjustments
		{
			get; set;
		}

		//Tax Exempt Combined TotalS
		public double TaxExemptCombinedPRBProfitBeforeTax
		{
			get { return _taxExemptCombinedPRBProfitBeforeTax; }
			set { SetProperty(ref _taxExemptCombinedPRBProfitBeforeTax, value); }
		}

		public double TaxExemptCombinedPRBCostsReceivable
		{
			get { return _taxExemptCombinedPRBCostsReceivable; }
			set { SetProperty(ref _taxExemptCombinedPRBCostsReceivable, value); }
		}

		public double TaxExemptCombinedPRBCostsPayable
		{
			get { return _taxExemptCombinedPRBCostsPayable; }
			set { SetProperty(ref _taxExemptCombinedPRBCostsPayable, value); }
		}

		public double TaxExemptCombinedPRBHedgingDerivatives
		{
			get { return _taxExemptCombinedPRBHedgingDerivatives; }
			set { SetProperty(ref _taxExemptCombinedPRBHedgingDerivatives, value); }
		}

		public double TaxExemptCombinedResidualIncome
		{
			get { return _taxExemptCombinedResidualIncome; }
			set { SetProperty(ref _taxExemptCombinedResidualIncome, value); }
		}

		public double TaxExemptCombinedResidualExpense
		{
			get { return _taxExemptCombinedResidualExpense; }
			set { SetProperty(ref _taxExemptCombinedResidualExpense, value); }
		}

		public double TaxExemptCombinedAdjustmentsTotal
		{
			get { return _taxExemptCombinedAdjustmentsTotal; }
			set { SetProperty(ref _taxExemptCombinedAdjustmentsTotal, value); }
		}

		public double TaxExemptCombinedPRBProfits
		{
			get { return _taxExemptCombinedPRBProfits; }
			set { SetProperty(ref _taxExemptCombinedPRBProfits, value); }
		}

		public double TaxExemptCombinedInterestReceivable
		{
			get { return _taxExemptCombinedInterestReceivable; }
			set { SetProperty(ref _taxExemptCombinedInterestReceivable, value); }
		}

		public double TaxExemptCombinedInterestPayable
		{
			get { return _taxExemptCombinedInterestPayable; }
			set { SetProperty(ref _taxExemptCombinedInterestPayable, value); }
		}

		public double TaxExemptCombinedHedgingDerivatives
		{
			get { return _taxExemptCombinedHedgingDerivatives; }
			set { SetProperty(ref _taxExemptCombinedHedgingDerivatives, value); }
		}

		public double TaxExemptCombinedCapitalAllowances
		{
			get { return _taxExemptCombinedCapitalAllowances; }
			set { SetProperty(ref _taxExemptCombinedCapitalAllowances, value); }
		}

		public double TaxExemptCombinedOtherAdjustmentsTotal
		{
			get { return _taxExemptCombinedOtherAdjustmentsTotal; }
			set { SetProperty(ref _taxExemptCombinedOtherAdjustmentsTotal, value); }
		}

		public double TaxExemptCombinedPropertyLossesBroughtForward
		{
			get { return _taxExemptCombinedPropertyLossesBroughtForward; }
			set { SetProperty(ref _taxExemptCombinedPropertyLossesBroughtForward, value); }
		}

		public double TaxExemptCombinedNonREITProfits
		{
			get { return _taxExemptCombinedNonREITProfits; }
			set { SetProperty(ref _taxExemptCombinedNonREITProfits, value); }
		}

		public double TaxExemptCombinedREITProfits
		{
			get { return _taxExemptCombinedREITProfits; }
			set { SetProperty(ref _taxExemptCombinedREITProfits, value); }
		}

		public ObservableCollection<Adjustment> TaxExemptCombinedIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> TaxExemptCombinedOtherAdjustments
		{
			get; set;
		}

		//Tax Exempt UK TotalS
		public double TaxExemptUKPRBProfitBeforeTax
		{
			get { return _taxExemptUKPRBProfitBeforeTax; }
			set { SetProperty(ref _taxExemptUKPRBProfitBeforeTax, value); }
		}

		public double TaxExemptUKPRBCostsReceivable
		{
			get { return _taxExemptUKPRBCostsReceivable; }
			set { SetProperty(ref _taxExemptUKPRBCostsReceivable, value); }
		}

		public double TaxExemptUKPRBCostsPayable
		{
			get { return _taxExemptUKPRBCostsPayable; }
			set { SetProperty(ref _taxExemptUKPRBCostsPayable, value); }
		}

		public double TaxExemptUKPRBHedgingDerivatives
		{
			get { return _taxExemptUKPRBHedgingDerivatives; }
			set { SetProperty(ref _taxExemptUKPRBHedgingDerivatives, value); }
		}

		public double TaxExemptUKResidualIncome
		{
			get { return _taxExemptUKResidualIncome; }
			set { SetProperty(ref _taxExemptUKResidualIncome, value); }
		}

		public double TaxExemptUKResidualExpense
		{
			get { return _taxExemptUKResidualExpense; }
			set { SetProperty(ref _taxExemptUKResidualExpense, value); }
		}

		public double TaxExemptUKAdjustmentsTotal
		{
			get { return _taxExemptUKAdjustmentsTotal; }
			set { SetProperty(ref _taxExemptUKAdjustmentsTotal, value); }
		}

		public double TaxExemptUKPRBProfits
		{
			get { return _taxExemptUKPRBProfits; }
			set { SetProperty(ref _taxExemptUKPRBProfits, value); }
		}

		public double TaxExemptUKInterestReceivable
		{
			get { return _taxExemptUKInterestReceivable; }
			set { SetProperty(ref _taxExemptUKInterestReceivable, value); }
		}

		public double TaxExemptUKInterestPayable
		{
			get { return _taxExemptUKInterestPayable; }
			set { SetProperty(ref _taxExemptUKInterestPayable, value); }
		}

		public double TaxExemptUKHedgingDerivatives
		{
			get { return _taxExemptUKHedgingDerivatives; }
			set { SetProperty(ref _taxExemptUKHedgingDerivatives, value); }
		}

		public double TaxExemptUKCapitalAllowances
		{
			get { return _taxExemptUKCapitalAllowances; }
			set { SetProperty(ref _taxExemptUKCapitalAllowances, value); }
		}

		public double TaxExemptUKOtherAdjustmentsTotal
		{
			get { return _taxExemptUKOtherAdjustmentsTotal; }
			set { SetProperty(ref _taxExemptUKOtherAdjustmentsTotal, value); }
		}

		public double TaxExemptUKPropertyLossesBroughtForward
		{
			get { return _taxExemptUKPropertyLossesBroughtForward; }
			set { SetProperty(ref _taxExemptUKPropertyLossesBroughtForward, value); }
		}

		public double TaxExemptUKNonREITProfits
		{
			get { return _taxExemptUKNonREITProfits; }
			set { SetProperty(ref _taxExemptUKNonREITProfits, value); }
		}

		public double TaxExemptUKREITProfits
		{
			get { return _taxExemptUKREITProfits; }
			set { SetProperty(ref _taxExemptUKREITProfits, value); }
		}

		public ObservableCollection<Adjustment> TaxExemptUKIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> TaxExemptUKOtherAdjustments
		{
			get; set;
		}

		//Tax Exempt Non UK TotalS
		public double TaxExemptNonUKPRBProfitBeforeTax
		{
			get { return _taxExemptNonUKPRBProfitBeforeTax; }
			set { SetProperty(ref _taxExemptNonUKPRBProfitBeforeTax, value); }
		}

		public double TaxExemptNonUKPRBCostsReceivable
		{
			get { return _taxExemptNonUKPRBCostsReceivable; }
			set { SetProperty(ref _taxExemptNonUKPRBCostsReceivable, value); }
		}

		public double TaxExemptNonUKPRBCostsPayable
		{
			get { return _taxExemptNonUKPRBCostsPayable; }
			set { SetProperty(ref _taxExemptNonUKPRBCostsPayable, value); }
		}

		public double TaxExemptNonUKPRBHedgingDerivatives
		{
			get { return _taxExemptNonUKPRBHedgingDerivatives; }
			set { SetProperty(ref _taxExemptNonUKPRBHedgingDerivatives, value); }
		}

		public double TaxExemptNonUKResidualIncome
		{
			get { return _taxExemptNonUKResidualIncome; }
			set { SetProperty(ref _taxExemptNonUKResidualIncome, value); }
		}

		public double TaxExemptNonUKResidualExpense
		{
			get { return _taxExemptNonUKResidualExpense; }
			set { SetProperty(ref _taxExemptNonUKResidualExpense, value); }
		}

		public double TaxExemptNonUKAdjustmentsTotal
		{
			get { return _taxExemptNonUKAdjustmentsTotal; }
			set { SetProperty(ref _taxExemptNonUKAdjustmentsTotal, value); }
		}

		public double TaxExemptNonUKPRBProfits
		{
			get { return _taxExemptNonUKPRBProfits; }
			set { SetProperty(ref _taxExemptNonUKPRBProfits, value); }
		}

		public double TaxExemptNonUKInterestReceivable
		{
			get { return _taxExemptNonUKInterestReceivable; }
			set { SetProperty(ref _taxExemptNonUKInterestReceivable, value); }
		}

		public double TaxExemptNonUKInterestPayable
		{
			get { return _taxExemptNonUKInterestPayable; }
			set { SetProperty(ref _taxExemptNonUKInterestPayable, value); }
		}

		public double TaxExemptNonUKHedgingDerivatives
		{
			get { return _taxExemptNonUKHedgingDerivatives; }
			set { SetProperty(ref _taxExemptNonUKHedgingDerivatives, value); }
		}

		public double TaxExemptNonUKCapitalAllowances
		{
			get { return _taxExemptNonUKCapitalAllowances; }
			set { SetProperty(ref _taxExemptNonUKCapitalAllowances, value); }
		}

		public double TaxExemptNonUKOtherAdjustmentsTotal
		{
			get { return _taxExemptNonUKOtherAdjustmentsTotal; }
			set { SetProperty(ref _taxExemptNonUKOtherAdjustmentsTotal, value); }
		}

		public double TaxExemptNonUKPropertyLossesBroughtForward
		{
			get { return _taxExemptNonUKPropertyLossesBroughtForward; }
			set { SetProperty(ref _taxExemptNonUKPropertyLossesBroughtForward, value); }
		}

		public double TaxExemptNonUKNonREITProfits
		{
			get { return _taxExemptNonUKNonREITProfits; }
			set { SetProperty(ref _taxExemptNonUKNonREITProfits, value); }
		}

		public double TaxExemptNonUKREITProfits
		{
			get { return _taxExemptNonUKREITProfits; }
			set { SetProperty(ref _taxExemptNonUKREITProfits, value); }
		}

		public ObservableCollection<Adjustment> TaxExemptNonUKIncomeAdjustments
		{
			get; set;
		}

		public ObservableCollection<Adjustment> TaxExemptNonUKOtherAdjustments
		{
			get; set;
		}

		//Summary IAS Profits Property
		public double SummaryPropertyRevenueLCS
		{
			get { return _summaryPropertyRevenueLCS; }
			set { SetProperty(ref _summaryPropertyRevenueLCS, value); }
		}

		public double SummaryPropertyPIDs
		{
			get { return _summaryPropertyPIDs; }
			set { SetProperty(ref _summaryPropertyPIDs, value); }
		}

		public double SummaryPropertyIASOtherIncomeExpense
		{
			get { return _summaryPropertyIASOtherIncomeExpense; }
			set { SetProperty(ref _summaryPropertyIASOtherIncomeExpense, value); }
		}

		public double SummaryPropertyIncomeBeforeCosts
		{
			get { return _summaryPropertyIncomeBeforeCosts; }
			set { SetProperty(ref _summaryPropertyIncomeBeforeCosts, value); }
		}

		public double SummaryPropertyExternalFinancialCosts
		{
			get { return _summaryPropertyExternalFinancialCosts; }
			set { SetProperty(ref _summaryPropertyExternalFinancialCosts, value); }
		}

		public double SummaryPropertyIncomeAfterCosts
		{
			get { return _summaryPropertyIncomeAfterCosts; }
			set { SetProperty(ref _summaryPropertyIncomeAfterCosts, value); }
		}

		public double SummaryPropertyPBTOtherIncomeExpense
		{
			get { return _summaryPropertyPBTOtherIncomeExpense; }
			set { SetProperty(ref _summaryPropertyPBTOtherIncomeExpense, value); }
		}

		public double SummaryPropertyPBTCalculated
		{
			get { return _summaryPropertyPBTCalculated; }
			set { SetProperty(ref _summaryPropertyPBTCalculated, value); }
		}

		//Summary IAS Profits Residual
		public double SummaryResidualRevenueLCS
		{
			get { return _summaryResidualRevenueLCS; }
			set { SetProperty(ref _summaryResidualRevenueLCS, value); }
		}

		public double SummaryResidualBeneficialInterestsIncome
		{
			get { return _summaryResidualBeneficialInterestsIncome; }
			set { SetProperty(ref _summaryResidualBeneficialInterestsIncome, value); }
		}

		public double SummaryResidualIASOtherIncomeExpense
		{
			get { return _summaryResidualIASOtherIncomeExpense; }
			set { SetProperty(ref _summaryResidualIASOtherIncomeExpense, value); }
		}

		public double SummaryResidualIncomeBeforeCosts
		{
			get { return _summaryResidualIncomeBeforeCosts; }
			set { SetProperty(ref _summaryResidualIncomeBeforeCosts, value); }
		}

		public double SummaryResidualResidualFinancialCosts
		{
			get { return _summaryResidualResidualFinancialCosts; }
			set { SetProperty(ref _summaryResidualResidualFinancialCosts, value); }
		}

		public double SummaryResidualIncomeAfterCosts
		{
			get { return _summaryResidualIncomeAfterCosts; }
			set { SetProperty(ref _summaryResidualIncomeAfterCosts, value); }
		}

		public double SummaryResidualPBTOtherIncomeExpense
		{
			get { return _summaryResidualPBTOtherIncomeExpense; }
			set { SetProperty(ref _summaryResidualPBTOtherIncomeExpense, value); }
		}

		public double SummaryResidualPBTCalculated
		{
			get { return _summaryResidualPBTCalculated; }
			set { SetProperty(ref _summaryResidualPBTCalculated, value); }
		}

		public double SummaryResidualBOBIncomeTest
		{
			get { return _summaryResidualBOBIncomeTest; }
			set { SetProperty(ref _summaryResidualBOBIncomeTest, value); }
		}

		public string SummaryResidualBOBIncomeTestPercentage
		{
			get { return _summaryResidualBOBIncomeTestPercentage; }
			set { SetProperty(ref _summaryResidualBOBIncomeTestPercentage, value); }
		}

		//Summary IAS Profits Total
		public double SummaryTotalISAPERevenueLCS
		{
			get { return _summaryTotalISAPERevenueLCS; }
			set { SetProperty(ref _summaryTotalISAPERevenueLCS, value); }
		}

		public double SummaryTotalISAPEBeneficialInterestsIncome
		{
			get { return _summaryTotalISAPEBeneficialInterestsIncome; }
			set { SetProperty(ref _summaryTotalISAPEBeneficialInterestsIncome, value); }
		}

		public double SummaryTotalISAPEPIDs
		{
			get { return _summaryTotalISAPEPIDs; }
			set { SetProperty(ref _summaryTotalISAPEPIDs, value); }
		}

		public double SummaryTotalISAPEIASOtherIncomeExpense
		{
			get { return _summaryTotalISAPEIASOtherIncomeExpense; }
			set { SetProperty(ref _summaryTotalISAPEIASOtherIncomeExpense, value); }
		}

		public double SummaryTotalISAPEIncomeBeforeCosts
		{
			get { return _summaryTotalISAPEIncomeBeforeCosts; }
			set { SetProperty(ref _summaryTotalISAPEIncomeBeforeCosts, value); }
		}

		public double SummaryTotalISAPEExternalFinancialCosts
		{
			get { return _summaryTotalISAPEExternalFinancialCosts; }
			set { SetProperty(ref _summaryTotalISAPEExternalFinancialCosts, value); }
		}

		public double SummaryTotalISAPEResidualFinancialCosts
		{
			get { return _summaryTotalISAPEResidualFinancialCosts; }
			set { SetProperty(ref _summaryTotalISAPEResidualFinancialCosts, value); }
		}

		public double SummaryTotalISAPEIncomeAfterCosts
		{
			get { return _summaryTotalISAPEIncomeAfterCosts; }
			set { SetProperty(ref _summaryTotalISAPEIncomeAfterCosts, value); }
		}

		public double SummaryTotalISAPEPBTOtherIncomeExpense
		{
			get { return _summaryTotalISAPEPBTOtherIncomeExpense; }
			set { SetProperty(ref _summaryTotalISAPEPBTOtherIncomeExpense, value); }
		}

		public double SummaryTotalISAPEPBTCalculated
		{
			get { return _summaryTotalISAPEPBTCalculated; }
			set { SetProperty(ref _summaryTotalISAPEPBTCalculated, value); }
		}

		public Reconciliation SelectedSummaryTotalISAPEReconciliation
		{
			get { return _selectedSummaryTotalISAPEReconciliation; }
			set { SetProperty(ref _selectedSummaryTotalISAPEReconciliation, value); }
		}

		public double SummaryTotalAuditedFinancialStatements
		{
			get { return _summaryTotalISAPEAuditedFinancialStatements; }
			set { SetProperty(ref _summaryTotalISAPEAuditedFinancialStatements, value); }
		}

		public string SummaryBOBIncomeTestResult
		{
			get { return _summaryBOBIncomeTestResult; }
			set { SetProperty(ref _summaryBOBIncomeTestResult, value); }
		}

		public ObservableCollection<Reconciliation> SummaryPBTReconciliations
		{
			get; set;
		}

		//Summary IAS Assets Property
		public double SummaryPropertyIASANonCurrentAssets
		{
			get { return _summaryPropertyIASANonCurrentAssets; }
			set { SetProperty(ref _summaryPropertyIASANonCurrentAssets, value); }
		}

		public double SummaryPropertyIASACurrentAssets
		{
			get { return _summaryPropertyIASACurrentAssets; }
			set { SetProperty(ref _summaryPropertyIASACurrentAssets, value); }
		}

		public double SummaryPropertyIASATotalAssets
		{
			get { return _summaryPropertyIASATotalAssets; }
			set { SetProperty(ref _summaryPropertyIASATotalAssets, value); }
		}

		//Summary IAS Assets Residual
		public double SummaryResidualIASANonCurrentAssets
		{
			get { return _summaryResidualIASANonCurrentAssets; }
			set { SetProperty(ref _summaryResidualIASANonCurrentAssets, value); }
		}

		public double SummaryResidualIASACurrentAssets
		{
			get { return _summaryResidualIASACurrentAssets; }
			set { SetProperty(ref _summaryResidualIASACurrentAssets, value); }
		}

		public double SummaryResidualIASATotalAssets
		{
			get { return _summaryResidualIASATotalAssets; }
			set { SetProperty(ref _summaryResidualIASATotalAssets, value); }
		}

		public double SummaryResidualTotalBOBAssetTest
		{
			get { return _summaryResidualTotalBOBAssetTest; }
			set { SetProperty(ref _summaryResidualTotalBOBAssetTest, value); }
		}

		public string SummaryResidualTotalBOBAssetTestPercentage
		{
			get { return _summaryResidualTotalBOBAssetTestPercentage; }
			set { SetProperty(ref _summaryResidualTotalBOBAssetTestPercentage, value); }
		}

		//Summary IAS Assets Total
		public double SummaryTotalIASANonCurrentAssets
		{
			get { return _summaryTotalIASANonCurrentAssets; }
			set { SetProperty(ref _summaryTotalIASANonCurrentAssets, value); }
		}

		public double SummaryTotalIASACurrentAssets
		{
			get { return _summaryTotalIASACurrentAssets; }
			set { SetProperty(ref _summaryTotalIASACurrentAssets, value); }
		}

		public double SummaryTotalIASATotalAssets
		{
			get { return _summaryTotalIASATotalAssets; }
			set { SetProperty(ref _summaryTotalIASATotalAssets, value); }
		}

		public Reconciliation SelectedSummaryTotalIASAReconciliation
		{
			get { return _selectedSummaryTotalIASAReconciliation; }
			set { SetProperty(ref _selectedSummaryTotalIASAReconciliation, value); }
		}

		public double SummaryTotalIASAFinancialStatements
		{
			get { return _summaryTotalIASAFinancialStatements; }
			set { SetProperty(ref _summaryTotalIASAFinancialStatements, value); }
		}

		public string SummaryISAABOBAssetTestResult
		{
			get { return _summaryISAABOBAssetTestResult; }
			set { SetProperty(ref _summaryISAABOBAssetTestResult, value); }
		}

		public ObservableCollection<Reconciliation> SummaryAssetReconciliations
		{
			get; set;
		}

		//Entities
		public ObservableCollection<Entity> EntitiesList
		{
			get { return _entitiesList; }
			set { SetProperty(ref _entitiesList, value); }
		}

		public Entity SelectedEntity
		{
			get { return _selectedEntity; }
			set { SetProperty(ref _selectedEntity, value); }
		}

		public List<string> YesNoOptionsList
		{
			get
			{
				return Enum.GetValues(typeof(YesNoOptions)).Cast<YesNoOptions>().Select(x => x.GetDescriptionFromEnum()).ToList();
			}
		}

		#endregion Properties

		#region Variables

		//Header Variables
		private string _rEITName;

		private string _principalUTR;
		private string _notes;
		private string _aPE;
		private string _pAPE;
		private string _version;
		private string _dateSubmitted;

		private string _createdBy;
		private string _dateCreated;

		//Test Variables
		private double _incomeTestPercentage;

		private string _incomeTestResult;
		private double _assetTestPercentage;
		private string _assetTestResult;
		private double _pidDist90Amount;
		private double _pidDist100Amount;
		private double _interestCoverAmount;
		private string _interestCoverRatio;
		private string _interestCoverResult;

		private string _paidDividendScheduleConfirmed;
		private Visibility _pdsVisibility;
		private Visibility _sduVisibility;

		//Property Combined Total Variables
		private double _propertyCombinedRevenueLCT;

		private double _propertyCombinedPIDs;
		private double _propertyCombinedOtherIncomeTotal;
		private double _propertyCombinedNetFinanceCosts;
		private double _propertyCombinedOtherAdjustmentsTotal;
		private double _propertyCombinedNonCurrentAssets;
		private double _propertyCombinedCurrentAssets;
		private double _propertyCombinedGrandTotal;
		private List<Adjustment> _propertyCombinedIncomeAdjustments;
		private List<Adjustment> _propertyCombinedOtherAdjustments;

		//Property UK Total Variables
		private double _propertyUKRevenueLCT;

		private double _propertyUKPIDs;
		private double _propertyUKOtherIncomeTotal;
		private double _propertyUKNetFinanceCosts;
		private double _propertyUKOtherAdjustmentsTotal;
		private double _propertyUKNonCurrentAssets;
		private double _propertyUKCurrentAssets;
		private double _propertyUKGrandTotal;
		private List<Adjustment> _propertyUKIncomeAdjustments;
		private List<Adjustment> _propertyUKOtherAdjustments;

		//Property Non UK Total Variables
		private double _propertyNonUKRevenueLCT;

		private double _propertyNonUKPIDs;
		private double _propertyNonUKOtherIncomeTotal;
		private double _propertyNonUKNetFinanceCosts;
		private double _propertyNonUKOtherAdjustmentsTotal;
		private double _propertyNonUKNonCurrentAssets;
		private double _propertyNonUKCurrentAssets;
		private double _propertyNonUKGrandTotal;
		private List<Adjustment> _propertyNonUKIncomeAdjustments;
		private List<Adjustment> _propertyNonUKOtherAdjustments;

		//Residual Combined Total Variables
		private double _residualCombinedRevenueLCT;

		private double _residualCombinedBeneficialInterests;
		private double _residualCombinedOtherIncomeTotal;
		private double _residualCombinedNetFinanceCosts;
		private double _residualCombinedOtherAdjustmentsTotal;
		private double _residualCombinedNonCurrentAssets;
		private double _residualCombinedCurrentAssets;
		private double _residualCombinedGrandTotal;

		//Residual UK Total Variables
		private double _residualUKRevenueLCT;

		private double _residualUKBeneficialInterests;
		private double _residualUKOtherIncomeTotal;
		private double _residualUKNetFinanceCosts;
		private double _residualUKOtherAdjustmentsTotal;
		private double _residualUKNonCurrentAssets;
		private double _residualUKCurrentAssets;
		private double _residualUKGrandTotal;

		//Residual Non UK Total Variables
		private double _residualNonUKRevenueLCT;

		private double _residualNonUKBeneficialInterests;
		private double _residualNonUKOtherIncomeTotal;
		private double _residualNonUKNetFinanceCosts;
		private double _residualNonUKOtherAdjustmentsTotal;
		private double _residualNonUKNonCurrentAssets;
		private double _residualNonUKCurrentAssets;
		private double _residualNonUKGrandTotal;

		//Tax Exempt Combined Total Variables
		private double _taxExemptCombinedPRBProfitBeforeTax;

		private double _taxExemptCombinedPRBCostsReceivable;
		private double _taxExemptCombinedPRBCostsPayable;
		private double _taxExemptCombinedPRBHedgingDerivatives;
		private double _taxExemptCombinedResidualIncome;
		private double _taxExemptCombinedResidualExpense;
		private double _taxExemptCombinedAdjustmentsTotal;
		private double _taxExemptCombinedPRBProfits;
		private double _taxExemptCombinedInterestReceivable;
		private double _taxExemptCombinedInterestPayable;
		private double _taxExemptCombinedHedgingDerivatives;
		private double _taxExemptCombinedCapitalAllowances;
		private double _taxExemptCombinedOtherAdjustmentsTotal;
		private double _taxExemptCombinedPropertyLossesBroughtForward;
		private double _taxExemptCombinedNonREITProfits;
		private double _taxExemptCombinedREITProfits;

		//Tax Exempt UK Total Variables
		private double _taxExemptUKPRBProfitBeforeTax;

		private double _taxExemptUKPRBCostsReceivable;
		private double _taxExemptUKPRBCostsPayable;
		private double _taxExemptUKPRBHedgingDerivatives;
		private double _taxExemptUKResidualIncome;
		private double _taxExemptUKResidualExpense;
		private double _taxExemptUKAdjustmentsTotal;
		private double _taxExemptUKPRBProfits;
		private double _taxExemptUKInterestReceivable;
		private double _taxExemptUKInterestPayable;
		private double _taxExemptUKHedgingDerivatives;
		private double _taxExemptUKCapitalAllowances;
		private double _taxExemptUKOtherAdjustmentsTotal;
		private double _taxExemptUKNonREITProfits;
		private double _taxExemptUKREITProfits;
		private double _taxExemptUKPropertyLossesBroughtForward;

		//Tax Exempt Non UK Total Variables
		private double _taxExemptNonUKPRBProfitBeforeTax;

		private double _taxExemptNonUKPRBCostsReceivable;
		private double _taxExemptNonUKPRBCostsPayable;
		private double _taxExemptNonUKPRBHedgingDerivatives;
		private double _taxExemptNonUKResidualIncome;
		private double _taxExemptNonUKResidualExpense;
		private double _taxExemptNonUKAdjustmentsTotal;
		private double _taxExemptNonUKPRBProfits;
		private double _taxExemptNonUKInterestReceivable;
		private double _taxExemptNonUKInterestPayable;
		private double _taxExemptNonUKHedgingDerivatives;
		private double _taxExemptNonUKCapitalAllowances;
		private double _taxExemptNonUKOtherAdjustmentsTotal;
		private double _taxExemptNonUKNonREITProfits;
		private double _taxExemptNonUKREITProfits;
		private double _taxExemptNonUKPropertyLossesBroughtForward;

		//Summary IAS Profits/Expenses Property Variables
		private double _summaryPropertyRevenueLCS;

		private double _summaryPropertyPIDs;
		private double _summaryPropertyIASOtherIncomeExpense;
		private double _summaryPropertyIncomeBeforeCosts;
		private double _summaryPropertyExternalFinancialCosts;
		private double _summaryPropertyIncomeAfterCosts;
		private double _summaryPropertyPBTOtherIncomeExpense;
		private double _summaryPropertyPBTCalculated;

		//Summary IAS Profits/Expenses Residual Variables
		private double _summaryResidualRevenueLCS;

		private double _summaryResidualBeneficialInterestsIncome;
		private double _summaryResidualIASOtherIncomeExpense;
		private double _summaryResidualIncomeBeforeCosts;
		private double _summaryResidualResidualFinancialCosts;
		private double _summaryResidualIncomeAfterCosts;
		private double _summaryResidualPBTOtherIncomeExpense;
		private double _summaryResidualPBTCalculated;
		private double _summaryResidualBOBIncomeTest;
		private string _summaryResidualBOBIncomeTestPercentage;

		//Summary IAS Profits/Expenses Total Variables
		private double _summaryTotalISAPERevenueLCS;

		private double _summaryTotalISAPEBeneficialInterestsIncome;
		private double _summaryTotalISAPEPIDs;
		private double _summaryTotalISAPEIASOtherIncomeExpense;
		private double _summaryTotalISAPEIncomeBeforeCosts;
		private double _summaryTotalISAPEExternalFinancialCosts;
		private double _summaryTotalISAPEResidualFinancialCosts;
		private double _summaryTotalISAPEIncomeAfterCosts;
		private double _summaryTotalISAPEPBTOtherIncomeExpense;
		private ObservableCollection<Reconciliation> _summaryTotalISAPEReconciliationsList;
		private Reconciliation _selectedSummaryTotalISAPEReconciliation;
		private double _summaryTotalISAPEPBTCalculated;
		private double _summaryTotalISAPEAuditedFinancialStatements;
		private double _summaryTotalISAPEBOBIncomeTest;

		//Summary IAS Assets Property
		private double _summaryPropertyIASANonCurrentAssets;

		private double _summaryPropertyIASACurrentAssets;
		private double _summaryPropertyIASATotalAssets;

		//Summary IAS Assets Residual
		private double _summaryResidualIASANonCurrentAssets;

		private double _summaryResidualIASACurrentAssets;
		private double _summaryResidualIASATotalAssets;
		private double _summaryResidualTotalBOBAssetTest;
		private string _summaryResidualTotalBOBAssetTestPercentage;

		//Summary IAS Assets Total
		private double _summaryTotalIASANonCurrentAssets;

		private double _summaryTotalIASACurrentAssets;
		private double _summaryTotalIASATotalAssets;
		private ObservableCollection<Reconciliation> _summaryTotalIASAReconciliationsList;
		private Reconciliation _selectedSummaryTotalIASAReconciliation;
		private double _summaryTotalIASAFinancialStatements;
		private string _summaryISAABOBAssetTestResult;

		private string _summaryBOBIncomeTestResult;

		//Entities
		private ObservableCollection<Entity> _entitiesList;

		private Entity _selectedEntity;

		private IREITDataService _dataService;

		private REIT _currentREIT;
		private REITTotals _currentREITTotals;

		#endregion Variables

		#region Navigation Methods

		public bool IsNavigationTarget(NavigationContext navigationContext)
		{
			return true;
		}

		public void OnNavigatedFrom(NavigationContext navigationContext)
		{
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITCompany", false));
		}

		public void OnNavigatedTo(NavigationContext navigationContext)
		{
			//PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REIT", true));
			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Search", true));

			PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("Print", true));

			if (navigationContext == null)
				return;

			Guid recordGuid = (Guid)navigationContext.Parameters["REITRecord"];

			_currentREIT = _dataService.GetREITRecord(recordGuid, true);
			_currentREITTotals = _dataService.GetREITTotalsRecord(recordGuid);

			string requestSource = navigationContext.Parameters["RequestSource"] as string;

			if (requestSource == "Parent")
				PrismHelpers.GetEventAggregator().GetEvent<MenuItemEnabledEvent>().Publish(new MenuItemEnabledEventPayload("REITCompany", true));

			DateSubmittedIsReadOnly = true;
			ToggleSubmissionDateEditIsEnabled = (UserSecurityDetails.AccessLevel == AccessLevels.Admin);

			LoadRecord();

			PrismHelpers.GetEventAggregator().GetEvent<TopMenuVisibilityCollapseEvent>().Publish();
		}

		#endregion Navigation Methods

		#region Private Methods

		private void CallPIDScheduleChanged()
		{
			Task.Delay(250).ContinueWith(_ =>
			{
				PDSVisibility = Visibility.Visible;

				_currentREITTotals.PaidDividendScheduleConfirmed = this.PaidDividendScheduleConfirmed;

				_dataService.SaveTotalsPIDScheduleConfirmed(_currentREITTotals);
			});

			Task.Delay(2000).ContinueWith(_ =>
			{
				PDSVisibility = Visibility.Hidden;
			});
		}

		private void CallUpdateREITSubmissionDate()
		{
			Task.Delay(250).ContinueWith(_ =>
			{
				SDUVisibility = Visibility.Visible;

				// leave note on record

				string XMLUpdateNote = string.Format("{0}\n{1} {2} XML Submission Date changed from {3} to {4}",
					_currentREIT.REITNotes, DateTime.Now.ToShortDateString(), UserSecurityDetails.FullNameAndPINumber, OriginalDateSubmitted, DateSubmitted);

				_currentREIT.REITNotes = XMLUpdateNote;

				_dataService.SaveUpdatedREITSubmissionDate(_currentREIT);
			});

			Task.Delay(2000).ContinueWith(_ =>
			{
				SDUVisibility = Visibility.Hidden;

				this.Notes = _currentREIT.REITNotes;

				OriginalDateSubmitted = DateSubmitted;
			});
		}

		private void LoadRecord()
		{
			if (_currentREIT != null && _currentREITTotals != null)
			{
				REITName = _currentREIT.REITName;
				PrincipalUTR = _currentREIT.PrincipalUTR;
				APE = string.Format("{0:dd/MM/yyyy}", _currentREIT.AccountPeriodEnd);
				PAPE = string.Format("{0:dd/MM/yyyy}", _currentREIT.PreviousAccountPeriodEnd);
				Notes = _currentREIT.REITNotes;
				CreatedBy = GetUserNameFromPID(_currentREIT.CreatedBy);
				DateCreated = string.Format("{0:dd/MM/yyyy}", _currentREIT.DateCreated);
				Version = _currentREIT.XMLVersion.ToString();
				DateSubmitted = string.Format("{0:dd/MM/yyyy hh:mm:ss}", _currentREIT.XMLDateSubmitted);

				OriginalDateSubmitted = DateSubmitted;

				List<Entity> uKEntities = new List<Entity>();
				uKEntities = (_currentREIT.Entities).Where(a => a.Jurisdiction == "United Kingdom").ToList();

				List<Entity> nonUKEntities = new List<Entity>();
				nonUKEntities = (_currentREIT.Entities).Where(a => a.Jurisdiction != "United Kingdom").ToList();

				//*** Property Tab ***

				//UK Totals

				PropertyUKRevenueLCT = _currentREITTotals.PropUKRevenueLessCostOfSales;
				PropertyUKPIDs = _currentREITTotals.PropUKPIDs;

				//Property UK Income Adjustments ListView
				List<Adjustment> _propertyUKIncomeAdjustments = new List<Adjustment>();

				_propertyUKIncomeAdjustments = uKEntities.SelectMany(x => x.Adjustments)
														.Where(a => a.AdjustmentCategory == AdjustmentCategories.PropertyRentalBusiness.GetDescriptionFromEnum()
																	&& a.AdjustmentType == AdjustmentTypes.PropertyRentalBusinessOtherIncomeOrExpense.GetDescriptionFromEnum())
														.ToList();

				PropertyUKIncomeAdjustments = new ObservableCollection<Adjustment>(_propertyUKIncomeAdjustments);
				RaisePropertyChanged("PropertyUKIncomeAdjustments");

				PropertyUKOtherIncomeTotal = _currentREITTotals.PropUKOtherIncomeOrExpenseAmount;
				PropertyUKNetFinanceCosts = _currentREITTotals.PropUKNetFinanceCosts;

				//Property UK Other Adjustments ListView
				List<Adjustment> _propertyUKOtherAdjustments = new List<Adjustment>();

				_propertyUKOtherAdjustments = uKEntities.SelectMany(x => x.Adjustments)
														.Where(a => a.AdjustmentCategory == AdjustmentCategories.PropertyRentalBusiness.GetDescriptionFromEnum()
																	&& a.AdjustmentType == AdjustmentTypes.PropertyRentalBusinessOtherAdjustments.GetDescriptionFromEnum())
														.ToList();

				PropertyUKOtherAdjustments = new ObservableCollection<Adjustment>(_propertyUKOtherAdjustments);
				RaisePropertyChanged("PropertyUKOtherAdjustments");

				PropertyUKOtherAdjustmentsTotal = _currentREITTotals.PropUKOtherAdjustmentsAmount;
				PropertyUKNonCurrentAssets = _currentREITTotals.PropUKNonCurrentAssets;
				PropertyUKCurrentAssets = _currentREITTotals.PropUKCurrentAssets;

				PropertyUKGrandTotal = PropertyUKNonCurrentAssets + PropertyUKCurrentAssets;

				//Non Non UK Totals

				PropertyNonUKRevenueLCT = _currentREITTotals.PropNonUKRevenueLessCostOfSales;
				PropertyNonUKPIDs = _currentREITTotals.PropNonUKPIDs;

				//Property Non UK Income Adjustments ListView
				List<Adjustment> _propertyNonUKIncomeAdjustments = new List<Adjustment>();

				_propertyNonUKIncomeAdjustments = nonUKEntities.SelectMany(x => x.Adjustments)
																.Where(a => a.AdjustmentCategory == AdjustmentCategories.PropertyRentalBusiness.GetDescriptionFromEnum()
																			&& a.AdjustmentType == AdjustmentTypes.PropertyRentalBusinessOtherIncomeOrExpense.GetDescriptionFromEnum())
																.ToList();

				PropertyNonUKIncomeAdjustments = new ObservableCollection<Adjustment>(_propertyNonUKIncomeAdjustments);
				RaisePropertyChanged("PropertyNonUKIncomeAdjustments");

				PropertyNonUKOtherIncomeTotal = _currentREITTotals.PropNonUKOtherIncomeOrExpenseAmount;
				PropertyNonUKNetFinanceCosts = _currentREITTotals.PropNonUKNetFinanceCosts;

				//Property Non UK Other Adjustments ListView
				List<Adjustment> _propertyNonUKOtherAdjustments = new List<Adjustment>();

				_propertyNonUKOtherAdjustments = nonUKEntities.SelectMany(x => x.Adjustments)
																.Where(a => a.AdjustmentCategory == AdjustmentCategories.PropertyRentalBusiness.GetDescriptionFromEnum()
																			&& a.AdjustmentType == AdjustmentTypes.PropertyRentalBusinessOtherAdjustments.GetDescriptionFromEnum())
																.ToList();

				PropertyNonUKOtherAdjustments = new ObservableCollection<Adjustment>(_propertyNonUKOtherAdjustments);
				RaisePropertyChanged("PropertyNonUKOtherAdjustments");

				PropertyNonUKOtherAdjustmentsTotal = _currentREITTotals.PropNonUKOtherAdjustmentsAmount;
				PropertyNonUKNonCurrentAssets = _currentREITTotals.PropNonUKNonCurrentAssets;
				PropertyNonUKCurrentAssets = _currentREITTotals.PropNonUKCurrentAssets;

				PropertyNonUKGrandTotal = PropertyNonUKNonCurrentAssets + PropertyNonUKCurrentAssets;

				//Combined Totals

				PropertyCombinedRevenueLCT = _currentREITTotals.PropCombinedRevenueLessCostOfSales;
				PropertyCombinedPIDs = _currentREITTotals.PropCombinedPIDs;

				//Property Combined Income Adjustments ListView
				List<Adjustment> _propertyCombinedIncomeAdjustments = new List<Adjustment>();

				_propertyCombinedIncomeAdjustments = _currentREIT.Entities.SelectMany(x => x.Adjustments)
											.Where(a => a.AdjustmentCategory == AdjustmentCategories.PropertyRentalBusiness.GetDescriptionFromEnum()
														&& a.AdjustmentType == AdjustmentTypes.PropertyRentalBusinessOtherIncomeOrExpense.GetDescriptionFromEnum())
											.ToList();

				PropertyCombinedIncomeAdjustments = new ObservableCollection<Adjustment>(_propertyCombinedIncomeAdjustments);

				RaisePropertyChanged("PropertyCombinedIncomeAdjustments");

				PropertyCombinedOtherIncomeTotal = _currentREITTotals.PropCombinedOtherIncomeOrExpenseAmount;
				PropertyCombinedNetFinanceCosts = _currentREITTotals.PropCombinedNetFinanceCosts;

				//Property Combined Other Adjustments ListView
				List<Adjustment> _propertyCombinedOtherAdjustments = new List<Adjustment>();

				_propertyCombinedOtherAdjustments = _currentREIT.Entities.SelectMany(x => x.Adjustments)
											.Where(a => a.AdjustmentCategory == AdjustmentCategories.PropertyRentalBusiness.GetDescriptionFromEnum()
														&& a.AdjustmentType == AdjustmentTypes.PropertyRentalBusinessOtherAdjustments.GetDescriptionFromEnum())
											.ToList();

				PropertyCombinedOtherAdjustments = new ObservableCollection<Adjustment>(_propertyCombinedOtherAdjustments);
				RaisePropertyChanged("PropertyCombinedOtherAdjustments");

				PropertyCombinedOtherAdjustmentsTotal = _currentREITTotals.PropCombinedOtherAdjustmentsAmount;
				PropertyCombinedNonCurrentAssets = _currentREITTotals.PropCombinedNonCurrentAssets;
				PropertyCombinedCurrentAssets = _currentREITTotals.PropCombinedCurrentAssets;

				PropertyCombinedGrandTotal = PropertyCombinedNonCurrentAssets + PropertyCombinedCurrentAssets;

				//*** Residual Tab ***
				//UK Totals

				ResidualUKRevenueLCT = _currentREITTotals.ResUKRevenueLessCostOfSales;
				ResidualUKBeneficialInterests = _currentREITTotals.ResUKBeneficialInterestsIncome;

				//Residual UK Income Adjustments ListView
				List<Adjustment> _residualUKIncomeAdjustments = new List<Adjustment>();

				_residualUKIncomeAdjustments = uKEntities.SelectMany(x => x.Adjustments)
											.Where(a => a.AdjustmentCategory == AdjustmentCategories.ResidualIncome.GetDescriptionFromEnum()
														&& a.AdjustmentType == AdjustmentTypes.ResidualIncomeOtherIncomeOrExpense.GetDescriptionFromEnum())
											.ToList();

				ResidualUKIncomeAdjustments = new ObservableCollection<Adjustment>(_residualUKIncomeAdjustments);
				RaisePropertyChanged("ResidualUKIncomeAdjustments");

				ResidualUKOtherIncomeTotal = _currentREITTotals.ResUKOtherIncomeOrExpenseAmount;
				ResidualUKNetFinanceCosts = _currentREITTotals.ResUKNetFinanceCosts;

				//Residual UK Other Adjustments ListView
				List<Adjustment> _residualUKOtherAdjustments = new List<Adjustment>();

				_residualUKOtherAdjustments = uKEntities.SelectMany(x => x.Adjustments)
											.Where(a => a.AdjustmentCategory == AdjustmentCategories.ResidualIncome.GetDescriptionFromEnum()
														&& a.AdjustmentType == AdjustmentTypes.ResidualIncomeOtherAdjustments.GetDescriptionFromEnum())
											.ToList();

				ResidualUKOtherAdjustments = new ObservableCollection<Adjustment>(_residualUKOtherAdjustments);
				RaisePropertyChanged("ResidualUKOtherAdjustments");

				ResidualUKOtherAdjustmentsTotal = _currentREITTotals.ResUKOtherAdjustmentsAmount;
				ResidualUKNonCurrentAssets = _currentREITTotals.ResUKNonCurrentAssets;
				ResidualUKCurrentAssets = _currentREITTotals.ResUKCurrentAssets;

				ResidualUKGrandTotal = ResidualUKCurrentAssets + ResidualUKNonCurrentAssets;

				//Non UK Totals

				ResidualNonUKRevenueLCT = _currentREITTotals.ResNonUKRevenueLessCostOfSales;
				ResidualNonUKBeneficialInterests = _currentREITTotals.ResNonUKBeneficialInterestsIncome;

				//Residual Non UK Income Adjustments ListView
				List<Adjustment> _residualNonUKIncomeAdjustments = new List<Adjustment>();

				_residualNonUKIncomeAdjustments = nonUKEntities.SelectMany(x => x.Adjustments)
							.Where(a => a.AdjustmentCategory == AdjustmentCategories.ResidualIncome.GetDescriptionFromEnum()
										&& a.AdjustmentType == AdjustmentTypes.ResidualIncomeOtherIncomeOrExpense.GetDescriptionFromEnum())
							.ToList();

				ResidualNonUKIncomeAdjustments = new ObservableCollection<Adjustment>(_residualNonUKIncomeAdjustments);
				RaisePropertyChanged("ResidualNonUKIncomeAdjustments");

				ResidualNonUKOtherIncomeTotal = _currentREITTotals.ResNonUKOtherIncomeOrExpenseAmount;
				ResidualNonUKNetFinanceCosts = _currentREITTotals.ResNonUKNetFinanceCosts;

				//Residual Non UK Other Adjustments ListView
				List<Adjustment> _residualNonUKOtherAdjustments = new List<Adjustment>();

				_residualNonUKOtherAdjustments = nonUKEntities.SelectMany(x => x.Adjustments)
							.Where(a => a.AdjustmentCategory == AdjustmentCategories.ResidualIncome.GetDescriptionFromEnum()
										&& a.AdjustmentType == AdjustmentTypes.ResidualIncomeOtherAdjustments.GetDescriptionFromEnum())
							.ToList();

				ResidualNonUKOtherAdjustments = new ObservableCollection<Adjustment>(_residualNonUKOtherAdjustments);
				RaisePropertyChanged("ResidualNonUKOtherAdjustments");

				ResidualNonUKOtherAdjustmentsTotal = _currentREITTotals.ResNonUKOtherAdjustmentsAmount;
				ResidualNonUKNonCurrentAssets = _currentREITTotals.ResNonUKNonCurrentAssets;
				ResidualNonUKCurrentAssets = _currentREITTotals.ResNonUKCurrentAssets;

				ResidualNonUKGrandTotal = ResidualNonUKCurrentAssets + ResidualNonUKNonCurrentAssets;

				//Combined Totals

				ResidualCombinedRevenueLCT = _currentREITTotals.ResCombinedRevenueLessCostOfSales;
				ResidualCombinedBeneficialInterests = _currentREITTotals.ResCombinedBeneficialInterestsIncome;

				//Residual Combined Income Adjustments ListView
				List<Adjustment> _residualCombinedIncomeAdjustments = new List<Adjustment>();

				_residualCombinedIncomeAdjustments = _currentREIT.Entities.SelectMany(x => x.Adjustments)
							.Where(a => a.AdjustmentCategory == AdjustmentCategories.ResidualIncome.GetDescriptionFromEnum()
										&& a.AdjustmentType == AdjustmentTypes.ResidualIncomeOtherIncomeOrExpense.GetDescriptionFromEnum())
							.ToList();

				ResidualCombinedIncomeAdjustments = new ObservableCollection<Adjustment>(_residualCombinedIncomeAdjustments);
				RaisePropertyChanged("ResidualCombinedIncomeAdjustments");

				ResidualCombinedOtherIncomeTotal = _currentREITTotals.ResCombinedOtherIncomeOrExpenseAmount;
				ResidualCombinedNetFinanceCosts = _currentREITTotals.ResCombinedNetFinanceCosts;

				//Residual Combined Other Adjustments ListView
				List<Adjustment> _residualCombinedOtherAdjustments = new List<Adjustment>();

				_residualCombinedOtherAdjustments = _currentREIT.Entities.SelectMany(x => x.Adjustments)
							.Where(a => a.AdjustmentCategory == AdjustmentCategories.ResidualIncome.GetDescriptionFromEnum()
										&& a.AdjustmentType == AdjustmentTypes.ResidualIncomeOtherAdjustments.GetDescriptionFromEnum())
							.ToList();

				ResidualCombinedOtherAdjustments = new ObservableCollection<Adjustment>(_residualCombinedOtherAdjustments);
				RaisePropertyChanged("ResidualCombinedOtherAdjustments");

				ResidualCombinedOtherAdjustmentsTotal = _currentREITTotals.ResCombinedOtherAdjustmentsAmount;
				ResidualCombinedNonCurrentAssets = _currentREITTotals.ResCombinedNonCurrentAssets;
				ResidualCombinedCurrentAssets = _currentREITTotals.ResCombinedCurrentAssets;

				ResidualCombinedGrandTotal = ResidualCombinedCurrentAssets + ResidualCombinedNonCurrentAssets;

				//***Tax Exempt Tab ***
				//UK Totals

				TaxExemptUKPRBProfitBeforeTax = _currentREITTotals.TaxExUKPRBProfitsBeforeTax;
				TaxExemptUKPRBCostsReceivable = _currentREITTotals.TaxExUKPRBIntAndFCsReceivable;
				TaxExemptUKPRBCostsPayable = _currentREITTotals.TaxExUKPRBIntAndFCsPayable;
				TaxExemptUKPRBHedgingDerivatives = _currentREITTotals.TaxExUKPRBHedgingDerivatives;
				TaxExemptUKResidualIncome = _currentREITTotals.TaxExUKPRBResidualIncome;
				TaxExemptUKResidualExpense = _currentREITTotals.TaxExUKPRBResidualExpenses;

				//Tax Exempt Adjustments ListView
				List<Adjustment> _taxExemptUKIncomeAdjustments = new List<Adjustment>();

				_taxExemptUKIncomeAdjustments = uKEntities.SelectMany(x => x.Adjustments)
							.Where(a => a.AdjustmentCategory == AdjustmentCategories.TaxExempt.GetDescriptionFromEnum()
										&& a.AdjustmentType == AdjustmentTypes.TaxExemptAdjustmentsToExcludeOtherTaxDisallowableExpenditureInAccountingPBT.GetDescriptionFromEnum())
							.ToList();

				TaxExemptUKIncomeAdjustments = new ObservableCollection<Adjustment>(_taxExemptUKIncomeAdjustments);
				RaisePropertyChanged("TaxExemptUKIncomeAdjustments");

				TaxExemptUKAdjustmentsTotal = _currentREITTotals.TaxExUKPBTAdjustments;

				TaxExemptUKPRBProfits = _currentREITTotals.TaxExUKUKPRBProfits;
				TaxExemptUKInterestReceivable = _currentREITTotals.TaxExUKIntAndFCsReceivable;
				TaxExemptUKInterestPayable = _currentREITTotals.TaxExUKIntAndFCsPayable;
				TaxExemptUKHedgingDerivatives = _currentREITTotals.TaxExUKHedgingDerivatives;
				TaxExemptUKCapitalAllowances = _currentREITTotals.TaxExUKCapitalAllowances;

				//Tax Exempt Adjustments ListView
				List<Adjustment> _taxExemptUKOtherAdjustments = new List<Adjustment>();

				_taxExemptUKOtherAdjustments = uKEntities.SelectMany(x => x.Adjustments)
						   .Where(a => a.AdjustmentCategory == AdjustmentCategories.TaxExempt.GetDescriptionFromEnum()
									   && a.AdjustmentType == AdjustmentTypes.TaxExemptOtherTaxAdjustments.GetDescriptionFromEnum())
						   .ToList();

				TaxExemptUKOtherAdjustments = new ObservableCollection<Adjustment>(_taxExemptUKOtherAdjustments);
				RaisePropertyChanged("TaxExemptUKOtherAdjustments");

				TaxExemptUKOtherAdjustmentsTotal = _currentREITTotals.TaxExUKOtherTaxAdjustments;

				TaxExemptUKPropertyLossesBroughtForward = _currentREITTotals.TaxExUKUKPropertyBroughtFwd;
				TaxExemptUKNonREITProfits = _currentREITTotals.TaxExUKProfitsExREITSInvProfits;
				TaxExemptUKREITProfits = _currentREITTotals.TaxExUKREITSInvProfits;

				//Non UK Totals

				TaxExemptNonUKPRBProfitBeforeTax = _currentREITTotals.TaxExNonUKPRBProfitsBeforeTax;
				TaxExemptNonUKPRBCostsReceivable = _currentREITTotals.TaxExNonUKPRBIntAndFCsReceivable;
				TaxExemptNonUKPRBCostsPayable = _currentREITTotals.TaxExNonUKPRBIntAndFCsPayable;
				TaxExemptNonUKPRBHedgingDerivatives = _currentREITTotals.TaxExNonUKPRBHedgingDerivatives;
				TaxExemptNonUKResidualIncome = _currentREITTotals.TaxExNonUKPRBResidualIncome;
				TaxExemptNonUKResidualExpense = _currentREITTotals.TaxExNonUKPRBResidualExpenses;

				//Tax Exempt Non UK Other Adjustments ListView
				List<Adjustment> _taxExemptNonUKIncomeAdjustments = new List<Adjustment>();

				_taxExemptNonUKIncomeAdjustments = nonUKEntities.SelectMany(x => x.Adjustments)
						   .Where(a => a.AdjustmentCategory == AdjustmentCategories.TaxExempt.GetDescriptionFromEnum()
									   && a.AdjustmentType == AdjustmentTypes.TaxExemptAdjustmentsToExcludeOtherTaxDisallowableExpenditureInAccountingPBT.GetDescriptionFromEnum())
						   .ToList();

				TaxExemptNonUKIncomeAdjustments = new ObservableCollection<Adjustment>(_taxExemptNonUKIncomeAdjustments);
				RaisePropertyChanged("TaxExemptNonUKIncomeAdjustments");

				TaxExemptNonUKAdjustmentsTotal = _currentREITTotals.TaxExNonUKPBTAdjustments;

				TaxExemptNonUKPRBProfits = _currentREITTotals.TaxExNonUKUKPRBProfits;
				TaxExemptNonUKInterestReceivable = _currentREITTotals.TaxExNonUKIntAndFCsReceivable;
				TaxExemptNonUKInterestPayable = _currentREITTotals.TaxExNonUKIntAndFCsPayable;
				TaxExemptNonUKHedgingDerivatives = _currentREITTotals.TaxExNonUKHedgingDerivatives;
				TaxExemptNonUKCapitalAllowances = _currentREITTotals.TaxExNonUKCapitalAllowances;

				//Tax Exempt Non UK Other Adjustments ListView
				List<Adjustment> _taxExemptNonUKOtherAdjustments = new List<Adjustment>();

				_taxExemptNonUKOtherAdjustments = nonUKEntities.SelectMany(x => x.Adjustments)
						   .Where(a => a.AdjustmentCategory == AdjustmentCategories.TaxExempt.GetDescriptionFromEnum()
									   && a.AdjustmentType == AdjustmentTypes.TaxExemptOtherTaxAdjustments.GetDescriptionFromEnum())
						   .ToList();

				TaxExemptNonUKOtherAdjustments = new ObservableCollection<Adjustment>(_taxExemptNonUKOtherAdjustments);
				RaisePropertyChanged("TaxExemptNonUKOtherAdjustments");

				TaxExemptNonUKOtherAdjustmentsTotal = _currentREITTotals.TaxExNonUKOtherTaxAdjustments;

				TaxExemptNonUKPropertyLossesBroughtForward = _currentREITTotals.TaxExNonUKUKPropertyBroughtFwd;
				TaxExemptNonUKNonREITProfits = _currentREITTotals.TaxExNonUKProfitsExREITSInvProfits;
				TaxExemptNonUKREITProfits = _currentREITTotals.TaxExNonUKREITSInvProfits;

				//Combined Totals

				TaxExemptCombinedPRBProfitBeforeTax = _currentREITTotals.TaxExCombinedPRBProfitsBeforeTax;
				TaxExemptCombinedPRBCostsReceivable = _currentREITTotals.TaxExCombinedPRBIntAndFCsReceivable;
				TaxExemptCombinedPRBCostsPayable = _currentREITTotals.TaxExCombinedPRBIntAndFCsPayable;
				TaxExemptCombinedPRBHedgingDerivatives = _currentREITTotals.TaxExCombinedPRBHedgingDerivatives;
				TaxExemptCombinedResidualIncome = _currentREITTotals.TaxExCombinedPRBResidualIncome;
				TaxExemptCombinedResidualExpense = _currentREITTotals.TaxExCombinedPRBResidualExpenses;

				//Tax Exempt Adjustments ListView
				List<Adjustment> _taxExemptCombinedIncomeAdjustments = new List<Adjustment>();

				_taxExemptCombinedIncomeAdjustments = _currentREIT.Entities.SelectMany(x => x.Adjustments)
						   .Where(a => a.AdjustmentCategory == AdjustmentCategories.TaxExempt.GetDescriptionFromEnum()
									   && a.AdjustmentType == AdjustmentTypes.TaxExemptAdjustmentsToExcludeOtherTaxDisallowableExpenditureInAccountingPBT.GetDescriptionFromEnum())
						   .ToList();

				TaxExemptCombinedIncomeAdjustments = new ObservableCollection<Adjustment>(_taxExemptCombinedIncomeAdjustments);
				RaisePropertyChanged("TaxExemptCombinedIncomeAdjustments");

				TaxExemptCombinedAdjustmentsTotal = _currentREITTotals.TaxExCombinedPBTAdjustments;

				TaxExemptCombinedPRBProfits = _currentREITTotals.TaxExCombinedUKPRBProfits;
				TaxExemptCombinedInterestReceivable = _currentREITTotals.TaxExCombinedIntAndFCsReceivable;
				TaxExemptCombinedInterestPayable = _currentREITTotals.TaxExCombinedIntAndFCsPayable;
				TaxExemptCombinedHedgingDerivatives = _currentREITTotals.TaxExCombinedHedgingDerivatives;
				TaxExemptCombinedCapitalAllowances = _currentREITTotals.TaxExCombinedCapitalAllowances;

				//Tax Exempt Other Adjustments ListView
				List<Adjustment> _taxExemptCombinedOtherAdjustments = new List<Adjustment>();

				_taxExemptCombinedOtherAdjustments = _currentREIT.Entities.SelectMany(x => x.Adjustments)
						   .Where(a => a.AdjustmentCategory == AdjustmentCategories.TaxExempt.GetDescriptionFromEnum()
									   && a.AdjustmentType == AdjustmentTypes.TaxExemptOtherTaxAdjustments.GetDescriptionFromEnum())
						   .ToList();

				TaxExemptCombinedOtherAdjustments = new ObservableCollection<Adjustment>(_taxExemptCombinedOtherAdjustments);
				RaisePropertyChanged("TaxExemptCombinedOtherAdjustments");

				TaxExemptCombinedOtherAdjustmentsTotal = _currentREITTotals.TaxExCombinedOtherTaxAdjustments;

				TaxExemptCombinedPropertyLossesBroughtForward = _currentREITTotals.TaxExCombinedUKPropertyBroughtFwd;

				TaxExemptCombinedNonREITProfits = _currentREITTotals.TaxExCombinedProfitsExREITSInvProfits;
				TaxExemptCombinedREITProfits = _currentREITTotals.TaxExCombinedREITSInvProfits;

				// Tests

				//Balance of Buisness test

				IncomeTestPercentage = _currentREITTotals.IncomeTestPercentage;
				IncomeTestResult = _currentREITTotals.IncomeTestResult;

				AssetTestPercentage = _currentREITTotals.AssetTestPercentage;
				AssetTestResult = _currentREITTotals.AssetTestResult;

				// interest ratio

				InterestCoverRatio = String.Format("{0:0.00}", _currentREITTotals.InterestCoverRatioTestPercentage);
				InterestCoverResult = _currentREITTotals.InterestCoverRatioTestResult;

				//100% PID Distribution Test
				PidDist90Amount = _currentREITTotals.PIDDistribution90Amount;
				PidDist100Amount = _currentREITTotals.PIDDistribution100Amount;
				PaidDividendScheduleConfirmed = _currentREITTotals.PaidDividendScheduleConfirmed;

				//Summary Tab

				//Revenue Less Cost of Sales
				SummaryPropertyRevenueLCS = PropertyCombinedRevenueLCT;
				SummaryResidualRevenueLCS = ResidualCombinedRevenueLCT;
				SummaryTotalISAPERevenueLCS = PropertyCombinedRevenueLCT + ResidualCombinedRevenueLCT;

				//Beneficial Interests
				SummaryResidualBeneficialInterestsIncome = ResidualCombinedBeneficialInterests;
				SummaryTotalISAPEBeneficialInterestsIncome = ResidualCombinedBeneficialInterests;

				//PIDs from other UK REITs
				SummaryPropertyPIDs = PropertyCombinedPIDs;
				SummaryTotalISAPEPIDs = SummaryPropertyPIDs;

				//Other income expense
				SummaryPropertyIASOtherIncomeExpense = PropertyCombinedOtherIncomeTotal;
				SummaryResidualIASOtherIncomeExpense = ResidualCombinedOtherIncomeTotal;
				SummaryTotalISAPEIASOtherIncomeExpense = PropertyCombinedOtherIncomeTotal + ResidualCombinedOtherIncomeTotal;

				//PBT and other comprehensive income
				SummaryPropertyIncomeBeforeCosts = SummaryPropertyRevenueLCS + SummaryPropertyPIDs + SummaryPropertyIASOtherIncomeExpense;
				SummaryResidualIncomeBeforeCosts = SummaryResidualRevenueLCS + SummaryResidualBeneficialInterestsIncome + SummaryResidualIASOtherIncomeExpense;
				SummaryTotalISAPEIncomeBeforeCosts = SummaryTotalISAPERevenueLCS + SummaryTotalISAPEBeneficialInterestsIncome + SummaryTotalISAPEPIDs + SummaryTotalISAPEIASOtherIncomeExpense;

				//Net finance costs external income befire costs
				SummaryPropertyExternalFinancialCosts = PropertyCombinedNetFinanceCosts;
				SummaryTotalISAPEExternalFinancialCosts = SummaryPropertyExternalFinancialCosts;

				//Net finance costs residual
				SummaryResidualResidualFinancialCosts = ResidualCombinedNetFinanceCosts;
				SummaryTotalISAPEResidualFinancialCosts = SummaryResidualResidualFinancialCosts;

				//PBT and other comprehensive income after costs
				SummaryPropertyIncomeAfterCosts = PropertyCombinedNetFinanceCosts + SummaryPropertyIncomeBeforeCosts;
				SummaryResidualIncomeAfterCosts = ResidualCombinedNetFinanceCosts + SummaryResidualIncomeBeforeCosts;
				SummaryTotalISAPEIncomeAfterCosts = SummaryPropertyIncomeAfterCosts + SummaryResidualIncomeAfterCosts;

				//Other income or expense

				SummaryPropertyPBTOtherIncomeExpense = PropertyCombinedOtherAdjustmentsTotal;
				SummaryResidualPBTOtherIncomeExpense = ResidualCombinedOtherAdjustmentsTotal;
				SummaryTotalISAPEPBTOtherIncomeExpense = SummaryPropertyPBTOtherIncomeExpense + SummaryResidualPBTOtherIncomeExpense;

				//PBT calculated in accordance with ISA after REIT adjustments

				SummaryPropertyPBTCalculated = SummaryPropertyPBTOtherIncomeExpense + SummaryPropertyIncomeAfterCosts;
				SummaryResidualPBTCalculated = SummaryResidualPBTOtherIncomeExpense + SummaryResidualIncomeAfterCosts;
				SummaryTotalISAPEPBTCalculated = SummaryPropertyPBTCalculated + SummaryResidualPBTCalculated;

				//Reconcilitaion PBT List View
				List<Reconciliation> _summaryPBTReconciliations = new List<Reconciliation>();
				_summaryPBTReconciliations = _currentREIT.Reconciliations.Where(x => x.ReconciliationType == SummaryReconciliationTypes.PBTReconciliation.GetDescriptionFromEnum())
						   .ToList();

				SummaryPBTReconciliations = new ObservableCollection<Reconciliation>(_summaryPBTReconciliations);
				RaisePropertyChanged("SummaryPBTReconciliations");

				//PBT per audited financial statements
				SummaryTotalAuditedFinancialStatements = SummaryTotalISAPEPBTCalculated + _currentREITTotals.PBTReconsToAuditedFinancialStatement;

				//IAS - Assets
				SummaryPropertyIASANonCurrentAssets = PropertyCombinedNonCurrentAssets;
				SummaryPropertyIASACurrentAssets = PropertyCombinedCurrentAssets;
				SummaryPropertyIASATotalAssets = PropertyCombinedGrandTotal;

				SummaryResidualIASANonCurrentAssets = ResidualCombinedNonCurrentAssets;
				SummaryResidualIASACurrentAssets = ResidualCombinedCurrentAssets;
				SummaryResidualIASATotalAssets = ResidualCombinedGrandTotal;

				SummaryTotalIASANonCurrentAssets = SummaryPropertyIASANonCurrentAssets + SummaryResidualIASANonCurrentAssets;
				SummaryTotalIASACurrentAssets = SummaryPropertyIASACurrentAssets + SummaryResidualIASACurrentAssets;
				SummaryTotalIASATotalAssets = SummaryPropertyIASATotalAssets + SummaryResidualIASATotalAssets;

				//Reconciliation Asset ListVeiw
				List<Reconciliation> _summaryAssetReconciliations = new List<Reconciliation>();
				_summaryAssetReconciliations = _currentREIT.Reconciliations.Where(x => x.ReconciliationType == SummaryReconciliationTypes.AssetReconciliation.GetDescriptionFromEnum())
						   .ToList();

				SummaryAssetReconciliations = new ObservableCollection<Reconciliation>(_summaryAssetReconciliations);
				RaisePropertyChanged("SummaryAssetReconciliations");

				//Assets per financial statement
				SummaryTotalIASAFinancialStatements = SummaryTotalIASATotalAssets + _currentREITTotals.ReconsToAuditedFinancialStatement;

				EntitiesList = new ObservableCollection<Entity>(_currentREIT.Entities.OrderBy(x => x.EntityName));
			}
		}

		private string GetUserNameFromPID(string pid)
		{
			var result = string.Format("{0}, not registered.", pid);

			var tempSystemUser = PrismHelpers.ResolveService<IUserDataService>().GetSystemUser(pid);

			if (tempSystemUser != null)
				result = tempSystemUser.FullNameAndPINumber;

			return result;
		}

		#endregion Private Methods
	}
}
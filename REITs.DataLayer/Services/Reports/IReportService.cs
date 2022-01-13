using Domain.Enums;
using Domain.Models;
using REITs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;

namespace REITs.DataLayer.Services
{
	public interface IReportService
	{
		DataTable ReportResults { get; }
		IList<Guid> SelectedCustomerList { get; set; }

		IList<string> GetAdjustmentCategoryNames();

		IList<CustomerListCheckBoxItem> GetGetCompanyNamesAndGuids();

		IList<string> GetUsedAdjustmentNamesForAdjustmentType(string adjCategory, string adjType);

		IList<string> GetUsedAdjustmentTypesForCategory(string adjCategory);

		IList<string> GetUsedReconciliationNamesForType(string reconType);

		IList<string> GetUsedReconciliationTypes();

		void ProduceReport(ReportType selectedReport, ReportCriteria criteria);
	}
}
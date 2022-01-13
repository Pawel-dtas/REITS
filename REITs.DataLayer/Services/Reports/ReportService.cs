using Domain.Enums;
using Domain.Models;
using REITs.DataLayer.Contexts;
using REITs.Domain.Models;
using REITs.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace REITs.DataLayer.Services
{
	public class ReportService : IReportService
	{
		public DataTable ReportResults { get; private set; }

		private IList<Guid> _selectedCustomerList;

		public IList<Guid> SelectedCustomerList
		{
			get { return _selectedCustomerList; }
			set
			{
				_selectedCustomerList = value;

				if (_selectedCustomerList != null)
				{
					_currentAdjustmentsUsedBySelectedParentREITS = GetAdjustmentsForParentCompanyREITs();
					_currentReconciliationsUsedBySelectedParentREITS = GetReconciliationsForParentCompanyREITs();
				}
			}
		}

		private IList<Adjustment> _currentAdjustmentsUsedBySelectedParentREITS;
		private IList<Reconciliation> _currentReconciliationsUsedBySelectedParentREITS;

		private ApplicationContext currentContext;

		private readonly IReportServiceSQLCommands sqlcmds = new ReportServiceSQLCommands();

		public ReportService()
		{
			if (currentContext == null)
				currentContext = new ApplicationContext();
		}

		public IList<CustomerListCheckBoxItem> GetGetCompanyNamesAndGuids()
		{
			IList<CustomerListCheckBoxItem> tempList = new List<CustomerListCheckBoxItem>();

			using (ApplicationContext context = new ApplicationContext())
			{
				tempList = context.REITParents.Select(i => new CustomerListCheckBoxItem { Guid = i.Id, Name = i.PrincipalCustomerName, SectorsFlag = i.SectorsFlag, IsSelected = false }).Distinct().OrderBy(i => i.Name).ToList();
			}

			return tempList;
		}

		public void ProduceReport(ReportType selectedReport, ReportCriteria criteria)
		{
			GetReportData(selectedReport, criteria);
		}

		private IList<Adjustment> GetAdjustmentsForParentCompanyREITs()
		{
			IList<Adjustment> tempList = currentContext.REITs.Where(r => SelectedCustomerList.Contains(r.ParentId))
								.Include(x => x.Entities.Select(a => a.Adjustments))
								.SelectMany(a => a.Entities.SelectMany(s => s.Adjustments))
								.ToList();

			return tempList;
		}

		private IList<Reconciliation> GetReconciliationsForParentCompanyREITs()
		{
			IList<Reconciliation> tempList = currentContext.REITs.Where(r => SelectedCustomerList.Contains(r.ParentId))
								.Include(x => x.Reconciliations)
								.SelectMany(a => a.Reconciliations)
								.ToList();

			return tempList;
		}

		public IList<string> GetAdjustmentCategoryNames()
		{
			IList<string> tempList = _currentAdjustmentsUsedBySelectedParentREITS
						.OrderBy(x => x.AdjustmentCategory)
						.Select(x => x.AdjustmentCategory)
						.Distinct().ToList();

			return tempList;
		}

		public IList<string> GetUsedAdjustmentTypesForCategory(string adjCategory)
		{
			IList<string> tempList = _currentAdjustmentsUsedBySelectedParentREITS.Where(x => x.AdjustmentCategory == adjCategory)
																				.OrderBy(x => x.AdjustmentType)
																				.Select(x => x.AdjustmentType.Replace(adjCategory, ""))
																				.Distinct().ToList();
			return tempList;
		}

		public IList<string> GetUsedAdjustmentNamesForAdjustmentType(string adjCategory, string adjType)
		{
			IList<string> tempLlist = _currentAdjustmentsUsedBySelectedParentREITS.Where(x => x.AdjustmentType.StartsWith(adjCategory) && x.AdjustmentType.EndsWith(adjType))
																	.OrderBy(x => x.AdjustmentName)
																	.Select(x => x.AdjustmentName)
																	.Distinct().ToList();

			return tempLlist;
		}

		public IList<string> GetUsedReconciliationTypes()
		{
			IList<string> tempList = _currentReconciliationsUsedBySelectedParentREITS.OrderBy(x => x.ReconciliationType)
																				.Select(x => x.ReconciliationType)
																				.Distinct().ToList();
			return tempList;
		}

		public IList<string> GetUsedReconciliationNamesForType(string reconType)
		{
			IList<string> tempLlist = _currentReconciliationsUsedBySelectedParentREITS.Where(x => x.ReconciliationType.EndsWith(reconType))
																	.OrderBy(x => x.ReconciliationType)
																	.Select(x => x.ReconciliationName)
																	.Distinct().ToList();

			return tempLlist;
		}

		private string GetSQLQuery(ReportType selectedReport, ReportCriteria rCrit)
		{
			var tempSQL = string.Empty;

			switch (selectedReport)
			{
				case ReportType.Submissions:
					tempSQL = sqlcmds.Submissions(rCrit);
					break;

				case ReportType.ExpectedSubmissions:
					tempSQL = sqlcmds.ExpectedSubmissions(rCrit);
					break;

				case ReportType.BRRSchedule:
					tempSQL = sqlcmds.BRRSchedule(rCrit);
					break;

				case ReportType.Tests:
					tempSQL = sqlcmds.Tests(rCrit);
					break;

				case ReportType.Analysis:
					tempSQL = (rCrit.AnalysisReportType == ReportAnalysisTypes.Adjustments.ToString()) ? sqlcmds.AnalysisOfAdjustments(rCrit) : sqlcmds.AnalysisOfReconciliations(rCrit);
					break;

				case ReportType.Notes:
					tempSQL = sqlcmds.Notes(rCrit);
					break;

				case ReportType.Summary:
					tempSQL = sqlcmds.Summary(rCrit);
					break;
			}

			return tempSQL;
		}

		private void GetReportData(ReportType selectedReport, ReportCriteria parameters)
		{
			using (SqlConnection con = new SqlConnection(currentContext.Database.Connection.ConnectionString))
			{
				DataTable tempTable = new DataTable();

				try
				{
					using (SqlCommand cmd = con.CreateCommand())
					{
						cmd.CommandType = CommandType.Text;
						cmd.CommandTimeout = 0;
						cmd.CommandText = GetSQLQuery(selectedReport, parameters);
						con.Open();

						cmd.ExecuteNonQuery();

						using (IDataReader reader = cmd.ExecuteReader())
						{
							tempTable.Load(reader);
						}

						ReportResults = tempTable;
					}
				}
				catch
				{
					con.Close();
				}
			}
		}
	}
}
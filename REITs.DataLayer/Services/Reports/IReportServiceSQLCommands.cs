using Domain.Models;

namespace REITs.DataLayer.Services
{
	internal interface IReportServiceSQLCommands
	{
		string AnalysisOfAdjustments(ReportCriteria rc);

		string AnalysisOfReconciliations(ReportCriteria rc);

		string Notes(ReportCriteria rc);

		string Submissions(ReportCriteria rc);

		string ExpectedSubmissions(ReportCriteria rc);

		string BRRSchedule(ReportCriteria rc);

		string Summary(ReportCriteria rc);

		string Tests(ReportCriteria rc);
	}
}
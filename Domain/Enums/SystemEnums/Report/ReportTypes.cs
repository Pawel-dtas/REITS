namespace Domain.Enums
{
	using System.ComponentModel;

	public enum ReportType
	{
		[Description("")]
		NotSet,

		[Description("Submissions")]
		Submissions,

		[Description("Expected Submissions")]
		ExpectedSubmissions,

		[Description("BRR Schedule")]
		BRRSchedule,

		[Description("Tests")]
		Tests,

		[Description("Analysis")]
		Analysis,

		[Description("Notes")]
		Notes,

		[Description("Summary")]
		Summary
	}
}
namespace Domain.Models
{
    public class ReportCriteria
    {
        public string ReportType { get; set; }

        public string ChosenCompanyGuids { get; set; }

        public string Version { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        // Note Specific
        public string NoteSearchWord { get; set; }

        // Analysis Specific

        public string AnalysisReportType { get; set; }
        public string AdjustmentCategory { get; set; }

        public string AnalysisType { get; set; }

        public string AnalysisName { get; set; }

        public bool EntityLevel { get; set; }
    }
}
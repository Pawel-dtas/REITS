using System.ComponentModel;

namespace Domain.Enums
{
	public enum EntityTypes
	{
		[Description("Principal Company")]
		PrincipalCompany,

		[Description("Consolidated")]
		Consolidated,

		[Description("Company")]
		Company,

		[Description("JV Company Subject To Notice Under S586/587 CTA 2010")]
		JVCompanySubjectToNoticeUnderS586587CTA2010,

		[Description("Open Ended Investment Company")]
		OpenEndedInvestmentCompany,

		[Description("Unit Trust")]
		UnitTrust,

		[Description("Partnership")]
		Partnership,

		[Description("Other Non-Corporate Entity")]
		OtherNonCorporateEntity
	}
}
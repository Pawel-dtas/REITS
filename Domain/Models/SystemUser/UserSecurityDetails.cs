using REITs.Domain.Enums;

namespace Domain.Models
{
	public static class UserSecurityDetails
	{
		public static string PINumber { get; set; }

		public static AccessLevels AccessLevel { get; set; }

		public static string FullNameAndPINumber { get; set; }
	}
}
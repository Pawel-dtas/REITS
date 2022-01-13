using System;

namespace REITs.Domain.Models
{
	public class SystemUser : BaseSystemUser
	{
		public string PINumber { get; set; }
		public string TelephoneNumber { get; set; }
		public string Team { get; set; }
		public string JobRole { get; set; }

		public string AccessLevel { get; set; }
		public string CreatedBy { get; set; }
		public DateTime DateCreated { get; set; }

		public string UpdatedBy { get; set; }

		public DateTime DateUpdated { get; set; }

		public bool IsActive { get; set; }

		public string FullNameAndPINumber
		{
			get { return string.Format("{0}, {1} ({2})", Surname, Forename, PINumber); }
		}
	}
}
using System;

namespace Domain.Models
{
	public class SearchResultModel
	{
		public Guid Id { get; set; }
		public Guid ParentId { get; set; }
		public Guid REITId { get; set; }
		public string PrincipalCustomerName { get; set; }

		public string REITName { get; set; }

		public string PrincipalUTR { get; set; }
		public string CustomerReference { get; set; }

		public DateTime APEDate { get; set; }

		public DateTime AccountPeriodEnd { get; set; }
		public DateTime PreviousAccountPeriodEnd { get; set; }

		public int SectorsFlag { get; set; }

		public string EntityName { get; set; }
		public string EntityType { get; set; }
		public string EntityUTR { get; set; }
		public double InterestPercentage { get; set; }
		public string Jurisdiction { get; set; }
		public bool TaxExempt { get; set; }
	}
}
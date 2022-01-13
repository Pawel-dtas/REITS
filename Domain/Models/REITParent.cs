using System;
using System.ComponentModel;

namespace REITs.Domain.Models
{
	public class REITParent : BaseEntity, INotifyPropertyChanged
	{
		public string PrincipalCustomerName { get; set; }

		public string PrincipalUTR { get; set; }
		public string TaxExemptUTR { get; set; }

		public DateTime? APEDate { get; set; }

		public DateTime? ConversionDate { get; set; }
		public DateTime? LastBRRDate { get; set; }

		public string NewReit { get; set; }
		public DateTime? NextBRRDate { get; set; }

		public string MarketsListedOn { get; set; }

		public Int64 MarketCapital { get; set; }
		public string MarketInfo { get; set; }

		public string CCM { get; set; }

		public string CoOrd { get; set; }
		public string CTG7 { get; set; }

		public string CTHO { get; set; }

		public string InformedConsent { get; set; }

		public string SAO { get; set; }

		public int SectorsFlag { get; set; }

		public DateTime? LeftRegime { get; set; }

		public string Notes { get; set; }

		public REITParent()
		{ }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
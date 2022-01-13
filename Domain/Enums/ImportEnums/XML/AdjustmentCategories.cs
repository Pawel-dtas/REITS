using System.ComponentModel;

namespace Domain.Enums
{
	public enum AdjustmentCategories
	{
		[Description("Property Rental Business")]
		PropertyRentalBusiness,

		[Description("Residual Income")]
		ResidualIncome,

		[Description("Tax Exempt")]
		TaxExempt
	}
}
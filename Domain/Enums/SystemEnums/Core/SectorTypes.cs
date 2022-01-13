using System;
using System.ComponentModel;

namespace REITs.Domain.Enums
{
	[Flags]
	public enum SectorTypes : int
	{
		[Description("ADVERTISING")]
		ADVERTISING = 1,

		[Description("CARE HOMES")]
		CAREHOMES = 2,

		[Description("GROUND RENTS")]
		GROUNDRENTS = 4,

		[Description("HEALTHCARE")]
		HEALTHCARE = 8,

		[Description("HOTELS")]
		HOTELS = 16,

		[Description("INDUSTRIAL")]
		INDUSTRIAL = 32,

		[Description("LEISURE")]
		LEISURE = 64,

		[Description("LOGISTICS")]
		LOGISTICS = 128,

		[Description("ML INDUSTRIAL")]
		MLINDUSTRIAL = 256,

		[Description("ML RETAIL")]
		MLRETAIL = 1024,

		[Description("OFFICE")]
		OFFICE = 2048,

		[Description("RESIDENTIAL")]
		RESIDENTIAL = 4096,

		[Description("RETAIL")]
		RETAIL = 8192,

		[Description("SOCIAL HOUSING")]
		SOCIALHOUSING = 16384,

		[Description("STORAGE")]
		STORAGE = 32768,

		[Description("STUDENT ACCOM")]
		STUDENTACCOM = 65536
	}
}
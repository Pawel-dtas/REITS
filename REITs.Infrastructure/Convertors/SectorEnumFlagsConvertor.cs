using Domain;
using REITs.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace REITs.Infrastructure.Convertors
{
	public class SectorEnumFlagsConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			SectorTypes flags = (SectorTypes)value;

			List<string> SectorDescriptions = new List<string>();

			foreach (SectorTypes ei in Enum.GetValues(typeof(SectorTypes)))
			{
				if (flags.HasFlag(ei))
					SectorDescriptions.Add(ei.GetDescriptionFromEnum());
			}

			return string.Join(" | ", SectorDescriptions.ToArray());
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}
}
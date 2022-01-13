using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace REITs.Infrastructure.Convertors
{
	public class StarWidthConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double listViewWidth = (double)values[0];
			double usedWidth = 0;

			if (listViewWidth == 0)
				return 0;

			ListView listview = values[1] as ListView;

			GridView gv = listview.View as GridView;

			for (int i = 0; i < gv.Columns.Count; i++)
			{
				if (!Double.IsNaN(gv.Columns[i].ActualWidth))
					usedWidth += gv.Columns[i].ActualWidth;
			}

			double result = Math.Floor(listViewWidth - usedWidth);

			return result;
		}

		public object[] ConvertBack(object values, Type[] targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace REITs.Infrastructure
{
    public class ComboBoxNullConverter : BaseConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return -1;
            return value;
        }
    }
}
using System;
using System.Globalization;
using System.Windows.Data;

namespace REITs.Infrastructure.Convertors
{
    public class DateToYearConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime inputDate = (DateTime)value;

            return inputDate.ToString("yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int inputYear = System.Convert.ToInt32(value);

            DateTime newDate = new DateTime(inputYear, 1, 1);

            return newDate;
        }
    }
}
using Domain;
using System;
using System.Globalization;
using System.Windows.Data;

namespace REITs.Infrastructure.Convertors
{
    public class EnumConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BaseEnumExtension.GetDescriptionFromEnum(value as Enum);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
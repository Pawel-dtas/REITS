﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace REITs.Infrastructure.Convertors
{
    public class VisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Visible" : "Collapsed";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
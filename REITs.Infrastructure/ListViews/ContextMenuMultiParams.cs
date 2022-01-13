using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace REITs.Infrastructure
{
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }

    public class PassThroughConverter : BaseConverter, IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IList<string> strList = new List<string>();

            try
            {
                strList = values.Cast<string>().ToList();
            }
            catch { }

            return strList;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
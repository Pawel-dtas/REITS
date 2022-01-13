using REITs.DataLayer.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Data;

namespace REITs.Application
{
	public class UserPIConvertor : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var origValue = (value != null) ? value.ToString() : string.Empty;

			return (string.IsNullOrEmpty(origValue)) ? origValue : StaticUserList.StaticListOfUsers.Where(u => u.PINumber == origValue).Select(x => x.FullNameAndPINumber).FirstOrDefault();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			string origValue = value.ToString();

			return Regex.Match(origValue, @"\d+").Value ?? origValue;
		}
	}
}
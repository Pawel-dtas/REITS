using REITs.Infrastructure.Convertors;
using REITs.Infrastructure.CustomControls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace REITs.Infrastructure
{
	public class DynamicListViewConverter : BaseConverter, IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			ColumnConfig config = value as ColumnConfig;

			if (config != null)
			{
				GridView gridView = new GridView();

				foreach (var column in config.Columns)
				{
					var binding = new Binding(column.DataField);

					if (column.IsDate)
						binding.StringFormat = "dd/MM/yyyy";  // date only

					if (column.IsYear)
						binding.StringFormat = "yyyy";  // date only

					if (column.IsMonthYear)
						binding.StringFormat = "MM yyyy";  // date only

					if (column.IsAPE)
						binding.StringFormat = "dd MMM";  // date only

					if (column.IsEnumFlag)
						binding.Converter = new SectorEnumFlagsConvertor();  // date only

					GridViewColumnHeader colHeader = new GridViewColumnHeader() { Content = column.Header };
					colHeader.Tag = column.DataField;
					colHeader.HorizontalContentAlignment = HorizontalAlignment.Stretch;
					colHeader.HorizontalAlignment = HorizontalAlignment.Stretch;
					colHeader.Background = new SolidColorBrush(Colors.LightGreen);

					gridView.Columns.Add(new GridViewColumn()
					{
						DisplayMemberBinding = binding,
						Header = colHeader,
						Width = column.Width
					});
				}

				return gridView;
			}

			return Binding.DoNothing;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}

	public class DynamicListViewMultiConverter : BaseConverter, IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			var listView = values[0] as SortableListView;

			var config = values[1] as ColumnConfig;

			if (config != null)
			{
				var gridView = new GridView();

				foreach (var column in config.Columns)
				{
					var binding = new Binding(column.DataField);

					if (column.IsDate)
						binding.StringFormat = "dd/MM/yyyy";

					if (column.IsDateTime)
						binding.StringFormat = "dd/MM/yyyy  HH:mm";

					if (column.Width == 0)  // pad to remaining width
					{
						double total = 0;

						for (int i = 0; i < gridView.Columns.Count - 1; i++)
						{
							total += gridView.Columns[i].Width;
						}

						column.Width = Math.Max(0, System.Convert.ToInt32(listView.ActualWidth - total));
					}

					GridViewColumnHeader colHeader = new GridViewColumnHeader() { Content = column.Header };
					colHeader.Tag = column.DataField;
					colHeader.HorizontalContentAlignment = HorizontalAlignment.Stretch;

					GridViewColumn newGridViewColumn = new GridViewColumn();
					newGridViewColumn.DisplayMemberBinding = binding;
					newGridViewColumn.Header = colHeader;
					newGridViewColumn.Width = column.Width;

					if (column.IsWrapped)
					{
						newGridViewColumn.DisplayMemberBinding = null;
						newGridViewColumn.CellTemplate = GetDataTemplate(binding);
					}

					gridView.Columns.Add(newGridViewColumn);
				}

				return gridView;
			}

			return Binding.DoNothing;
		}

		private DataTemplate GetDataTemplate(Binding dataBind)
		{
			DataTemplate template = new DataTemplate();

			FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
			factory.SetValue(TextBlock.TextWrappingProperty, TextWrapping.Wrap);
			factory.SetValue(TextBlock.TextProperty, dataBind);

			template.VisualTree = factory;

			return template;
		}

		public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
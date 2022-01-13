using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Windows;

namespace REITs.Infrastructure
{
	public static class ToHelper
	{
		public static object ToType<T>(this object obj, T type)
		{
			var tmp = Activator.CreateInstance(Type.GetType(type.ToString()));

			foreach (PropertyInfo pi in obj.GetType().GetProperties())
			{
				try
				{
					tmp.GetType().GetProperty(pi.Name).SetValue(tmp,
											  pi.GetValue(obj, null), null);
				}
				catch { }
			}

			return tmp;
		}

		public static object ToNonAnonymousList<T>(this List<T> list, Type t)
		{
			var genericType = typeof(List<>).MakeGenericType(t);

			var l = Activator.CreateInstance(genericType);

			MethodInfo addMethod = l.GetType().GetMethod("Add");

			foreach (T item in list)
			{
				addMethod.Invoke(l, new object[] { item.ToType(t) });
			}

			return l;
		}

		public static IList<T> CreateListFrom<T>(this ICollectionView sourceList)
		{
			List<T> tempList = new List<T>();
			var enumerator = ((IEnumerable)sourceList.SourceCollection).GetEnumerator();

			while (enumerator.MoveNext())
			{
				tempList.Add((T)enumerator.Current);
			}

			return tempList;
		}

		public static DataTable ToDataTable<T>(this IList<T> data)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

			DataTable table = new DataTable();

			foreach (PropertyDescriptor prop in properties)
				table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

			foreach (T item in data)
			{
				DataRow row = table.NewRow();
				foreach (PropertyDescriptor prop in properties)
					row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
				table.Rows.Add(row);
			}
			return table;
		}

		public static void CopyToClipboard(DataTable theDataTable)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 1; i <= 4; i++)
			{
				sb.Append(theDataTable.Columns[i].ColumnName + "\t");
			}

			sb.Remove(sb.Length - 1, 1);
			sb.Append(Environment.NewLine);

			foreach (DataRow row in theDataTable.Rows)
			{
				for (int i = 1; i <= 4; i++)
				{
					sb.Append(row[i].ToString() + "\t");
				}

				sb.Append(Environment.NewLine);
			}

			try
			{
				Clipboard.SetText(sb.ToString());

				//File.WriteAllText("C:/Temp/test.csv", sb.ToString());
			}
			catch { }
		}
	}
}
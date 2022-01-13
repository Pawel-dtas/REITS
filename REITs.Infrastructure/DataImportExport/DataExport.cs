using BDApp.Office.Tools;
using REITs.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace REITs.Infrastructure
{
	public static class DataExport
	{
		public static string ExportFileName { get; private set; }

		public static bool ExportData<T>(IList<T> sourceList, string suggestedFileName = "", params string[] columns)
		{
			bool success = false;

			string outputPath = SetExportFileName(suggestedFileName);

			if (!string.IsNullOrEmpty(outputPath))
			{
				try
				{
					DataSet ds = sourceList.ToDataSet<T>(columns);

					ExcelTools xltool = new ExcelTools();
					xltool.Export(outputPath, ds);

					ds = null;

					success = true;
				}
				catch (Exception ex) { Debug.Print(string.Format("Error exporting. Ex: {0}", ex.InnerException)); }
			}

			return success;
		}

		public static bool ExportRawData(this DataTable dataTable, string suggestedFileName = "")
		{
			bool success = false;

			string outputPath = SetExportFileName(suggestedFileName);

			if (!string.IsNullOrEmpty(outputPath))
			{
				try
				{
					DataSet ds = new DataSet();
					dataTable.ToRawList();
					ds.Tables.Add(dataTable);

					ExcelTools xltool = new ExcelTools();
					xltool.Export(outputPath, ds);

					ds = null;

					success = true;
				}
				catch { }
			}

			return success;
		}

		public static bool IsDataExportable<T>(string propName)
		{
			bool tempBool = false;

			var attrInfo = typeof(T).GetProperty(propName)
							 .GetCustomAttributes(typeof(DataExportable), false)
							 .Cast<DataExportable>().FirstOrDefault();

			if (attrInfo != null)
				tempBool = attrInfo.Exportable;

			return tempBool;
		}

		private static string SetExportFileName(string suggestedFileName)
		{
			string tempFileName = (string.IsNullOrEmpty(suggestedFileName)) ? "DataExportResults " : suggestedFileName;

			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog
			{
				FileName = string.Format("REITS-{0}-{1}_report", DateTime.Now.ToString("yyyy-MM-dd"), tempFileName).Replace(" ", "_"),
				DefaultExt = ".xlsx",
				Filter = "Excel documents(*.xlsx) | *.xlsx"
			};

			// Show open file dialog box
			Nullable<bool> result = dlg.ShowDialog();

			// Process open file dialog box results
			if (result == true)
				ExportFileName = dlg.FileName;

			return ExportFileName;
		}

		private static DataSet ToDataSet<T>(this IList<T> list, params string[] columns)
		{
			Type elementType = typeof(T);
			DataSet ds = new DataSet();
			DataTable t = new DataTable();
			ds.Tables.Add(t);

			foreach (var propInfo in elementType.GetProperties())
			{
				bool exportColumn = false;

				if (IsDataExportable<T>(propInfo.Name) || columns.Length > 0)
				{
					if (columns.Length > 0)
					{
						if (columns.Contains(propInfo.Name))
							exportColumn = true;
					}
					else
					{
						exportColumn = true;
					}

					try
					{
						if (exportColumn)
							t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType);
					}
					catch (Exception ex) { Debug.Print(string.Format("Error exporting column value: {0} ex: {1}", propInfo.Name, ex.InnerException)); }
				}
			}

			foreach (T item in list)
			{
				DataRow row = t.NewRow();
				foreach (var propInfo in elementType.GetProperties())
				{
					bool exportColumn = false;

					if (IsDataExportable<T>(propInfo.Name) || columns.Length > 0)
					{
						if (columns.Length > 0)
						{
							if (columns.Contains(propInfo.Name))
								exportColumn = true;
						}
						else
						{
							exportColumn = true;
						}

						try
						{
							if (exportColumn)
								row[propInfo.Name] = propInfo.GetValue(item, null);
						}
						catch (Exception ex) { Debug.Print(string.Format("Error exporting column value: {0} ex: {1}", propInfo.Name, ex.InnerException)); }
					}
				}

				t.Rows.Add(row);
			}

			return ds;
		}

		private static List<string[]> ToRawList(this DataTable table)
		{
			return table.Rows.Cast<DataRow>()
			   .Select(row => table.Columns.Cast<DataColumn>()
				  .Select(col => Convert.ToString(row[col]))
			   .ToArray())
			.ToList();
		}
	}
}
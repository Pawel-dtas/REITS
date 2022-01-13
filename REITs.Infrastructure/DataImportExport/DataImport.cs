using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;

namespace REITs.Infrastructure
{
    public static class DataImport
    {
        public static string ChooseExcelFileToImport()
        {
            return DisplayDialogChooseExcelFile();
        }

        public static string ChooseTextFileToImport()
        {
            return DisplayDialogChooseTextFile();
        }

        public static ObservableCollection<BulkTraderResult> ImportExcelFileAllFields(string dataFileName)
        {
            return ImportExcelFile(dataFileName, true);
        }

        public static ObservableCollection<BulkTraderResult> ImportExcelFileVATNumbers(string dataFileName)
        {
            return ImportExcelFile(dataFileName, false);
        }

        public static ObservableCollection<BulkTraderResult> ClearList()
        {
            return new ObservableCollection<BulkTraderResult>();
        }

        public static string ReadTextFile(string dataFileName)
        {
            return ReadPlainTextFile(dataFileName);
        }

        // private methods 

        private static string DisplayDialogChooseTextFile()
        {
            string tempDataFile = string.Empty;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "TextFile"; // Default file name
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents(*.txt) | *.txt";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
                tempDataFile = dlg.FileName;

            return tempDataFile;
        }

        private static string DisplayDialogChooseExcelFile()
        {
            string tempDataFile = string.Empty;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Document"; // Default file name
            dlg.DefaultExt = ".xls";
            dlg.Filter = "Excel documents(*.xls;*.xlsx) | *.xls; *.xlsx";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
                tempDataFile = dlg.FileName;

            return tempDataFile;
        }

        private static ObservableCollection<BulkTraderResult> ImportExcelFile(string dataFileName, bool allFields)
        {
            ObservableCollection<BulkTraderResult> tempCollection = new ObservableCollection<BulkTraderResult>();

            try
            {
                tempCollection = new ObservableCollection<BulkTraderResult>(ReadFromExcel(dataFileName, allFields));
            }
            catch (Exception ex){ Debug.Print(ex.InnerException.ToString()); }

            return tempCollection;
        }

        private static string ReadPlainTextFile(string dataFileName)
        {
            string fileText = string.Empty;

            using (var reader = File.OpenText(dataFileName))
            {
                fileText = reader.ReadToEnd();
            }

            return fileText;
        }

        private static ObservableCollection<BulkTraderResult> ReadFromExcel(string dataFileName, bool allFields = true)
        {
            string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dataFileName + ";Extended Properties=Excel 12.0;Persist Security Info=True";

            ObservableCollection<BulkTraderResult> tempTraders = new ObservableCollection<BulkTraderResult>();

            OleDbConnection Conn = new OleDbConnection(excelConnectionString);
            OleDbCommand Cmd = new OleDbCommand();

            Conn.Open();
            Cmd.Connection = Conn;
            Cmd.CommandText = "SELECT * FROM [Sheet1$]";

            using (DbDataReader dataReader = Cmd.ExecuteReader())
            {
                while (dataReader.Read())
                {
                    if (!string.IsNullOrEmpty(GetStringValueIfExists(dataReader, "VATNumber")))
                    {
                        if (string.IsNullOrEmpty(GetStringValueIfExists(dataReader, "TraderName")))
                            allFields = false;

                        if (allFields)
                        {
                            tempTraders.Add(new BulkTraderResult
                            {
                                VATNumber = GetStringValueIfExists(dataReader, "VATNumber"),
                                VATBranch = GetStringValueIfExists(dataReader, "VATBranch").PadLeft(3, '0'),
                                TraderName = GetStringValueIfExists(dataReader, "TraderName"),
                                TraderAddress = GetStringValueIfExists(dataReader, "TraderAddress"),
                                LVOCode = GetStringValueIfExists(dataReader, "LVO"),
                                Band = GetStringValueIfExists(dataReader, "Band"),
                                IntraStat = GetBoolValueIfExists(dataReader, "Intra"),
                                ExtraStat = GetBoolValueIfExists(dataReader, "Extra"),
                                TradeClass = GetStringValueIfExists(dataReader, "TradeClass"),
                                EmailAddress = GetStringValueIfExists(dataReader, "EmailAddress")
                            });
                        } else {
                            tempTraders.Add(new BulkTraderResult
                            {
                                VATNumber = GetStringValueIfExists(dataReader, "VATNumber"),
                                VATBranch = GetStringValueIfExists(dataReader, "VATBranch").PadLeft(3, '0')

                            });
                        }
                    }
                }

                dataReader.Close();

                Conn.Close();
            }

            return tempTraders;
        }

        private static string GetStringValueIfExists(DbDataReader dataReader, string columnName)
        {
            string tempValue = string.Empty;

            try
            {
                if (dataReader[columnName] != null)
                    tempValue = dataReader[columnName].ToString();
            }
            catch { }

            return tempValue;
        }

        private static bool? GetBoolValueIfExists(DbDataReader dataReader, string columnName)
        {
            bool? tempValue = null;

            try
            {
                if (dataReader[columnName] != null)
                {
                    if (dataReader[columnName].ToString().ToUpper().Contains("Y"))
                        tempValue = true;

                    if (dataReader[columnName].ToString().ToUpper().Contains("N"))
                        tempValue = false;
                }
            }
            catch { }

            return tempValue;
        }
    }
}

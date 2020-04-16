using AJG.VirtualTrainer.Helper.General;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace AJG.VirtualTrainer.Helper.Excel
{
    public enum AJGExcelDocSelectionType
    {
        MostRecent,
        DocName_AbsoluteValue,
        DocName_Regex,
        DocName_StartsWith,
        DocName_ContainsSubString
    }
    public enum AJGExcelDocSearchFilter
    {
        None,
        IsEqualTo,
        IsNotEqual,
        ContainsSubString,
        IsGreaterThan,
        IsLessThan,
        IsGreaterThanOrEqualTo,
        IsLessThanOrEqualTo
    }
    public enum AJGExcelLogicalOperator
    {
        None,
        And,
        Or
    }
    public enum AJGExcelDocDeleteMode
    {
        None,
        MoveTo,
        Delete
    }
    //public class KeyValue
    //{
    //    public string Key { get; set; }
    //    public string Value { get; set; }

    //    public KeyValue(string Key, string Value)
    //    {
    //        this.Key = Key;
    //        this.Value = Value;
    //    }
    //}
    public class ResultRow
    {
        public List<KeyValuePair<string, string>> Columns { get; set; }

        public ResultRow()
        {
            this.Columns = new List<KeyValuePair<string, string>>();
        }
    }
    public class ExcelHelper : IDisposable
    {
        #region [ Properties ]

        public string DocumentPath { get; }
        public List<ResultRow> Results { get; set; }
        public string oledbConnectionString { get; }
        public bool HasHeaderRow { get; set; }
        public List<string> supportedFileFOrmat
        {
            get
            {
                return new List<string>() { ".xls", ".xlsx", ".csv" };
            }
        }
        public string FileName { get; private set; }
        private FileFormat targetFileFormat { get; set; }

        OleDbCommand oleExcelCommand = null;
        OleDbDataReader oleExcelReader = null;
        OleDbConnection oleExcelConnection = null;

        private enum FileFormat
        {
            xls,
            xlsx,
            csv
        }

        #endregion

        #region [ Constructors ]

        public ExcelHelper(string documentPath, bool HasHeaderRow)
        {
            if (!File.Exists(documentPath))
            {
                throw new FileNotFoundException(string.Format("The file at location: {0}, could not be found.", documentPath));
            }

            FileInfo fileInfo = new FileInfo(documentPath);

            if (!supportedFileFOrmat.Contains(fileInfo.Extension))
            {
                throw new FormatException("The document must be either: xls, xlsx or a csv file.");
            }

            this.FileName = fileInfo.Name;

            switch (fileInfo.Extension)
            {
                case ".xls":
                    this.oledbConnectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR={1}'", documentPath, HasHeaderRow ? "YES" : "NO");
                    this.targetFileFormat = FileFormat.xls;
                    break;
                case ".xlsx":
                    this.oledbConnectionString = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0;HDR={1}'", documentPath, HasHeaderRow ? "YES" : "NO");
                    this.targetFileFormat = FileFormat.xlsx;
                    break;
                case ".csv":
                    this.oledbConnectionString = string.Format(@"Provider=Microsoft.Jet.OleDb.4.0; Data Source={0};Extended Properties=""Text;HDR={1};FMT=Delimited""", Path.GetDirectoryName(documentPath), HasHeaderRow ? "YES" : "NO");
                    this.targetFileFormat = FileFormat.csv;
                    break;
            }

            this.oleExcelCommand = default(OleDbCommand);
            this.oleExcelReader = default(OleDbDataReader);
            this.oleExcelConnection = new OleDbConnection(oledbConnectionString);
            oleExcelConnection.Open();

            this.DocumentPath = documentPath;
            this.Results = new List<ResultRow>();
        }

        #endregion [ Constructors ]

        #region [ Public Methods ]

        public void GetResultsFromDocument(string sqlQuery)
        {
            GetResultsFromDocument(sqlQuery, false);
        }
        public void GetResultsFromDocument(string sqlQuery, bool removeSpacesFromFieldValue)
        {
            this.Results = new List<ResultRow>();

            if (this.oleExcelConnection.State != ConnectionState.Open)
            {
                this.oleExcelConnection = new OleDbConnection(oledbConnectionString);
            }

            try
            {
                oleExcelCommand = oleExcelConnection.CreateCommand();
                oleExcelCommand.CommandText = this.targetFileFormat == FileFormat.csv ? string.Format(sqlQuery, this.FileName) : sqlQuery;
                oleExcelCommand.CommandType = CommandType.Text;
                using (oleExcelReader = oleExcelCommand.ExecuteReader())
                {
                    while (oleExcelReader.Read())
                    {
                        ResultRow resultRow = new ResultRow();

                        int count = oleExcelReader.FieldCount;
                        string displaytext = string.Empty;
                        for (int i = 0; i < oleExcelReader.FieldCount; i++)
                        {
                            string fieldName = oleExcelReader.GetName(i);
                            string fieldValue = oleExcelReader[fieldName].ToString();

                            if (removeSpacesFromFieldValue)
                            {
                                resultRow.Columns.Add(new KeyValuePair<string, string>(fieldName, fieldValue.Replace(" ", "")));
                            }
                            else
                            {
                                resultRow.Columns.Add(new KeyValuePair<string, string>(fieldName, fieldValue));
                            }
                        }

                        this.Results.Add(resultRow);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void ExecuteCommandNoResult(string sqlQuery)
        {
            if (this.oleExcelConnection.State != ConnectionState.Open)
            {
                this.oleExcelConnection = new OleDbConnection(oledbConnectionString);
            }

            try
            {
                oleExcelCommand = oleExcelConnection.CreateCommand();
                oleExcelCommand.CommandText = sqlQuery;

                oleExcelCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            if (this.oleExcelConnection.State == ConnectionState.Open)
            {
                this.oleExcelConnection.Close();
                this.oleExcelConnection.Dispose();
            }
        }

        #endregion
    }
}


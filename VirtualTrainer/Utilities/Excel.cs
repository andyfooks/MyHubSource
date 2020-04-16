using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.Reflection;
using System.ComponentModel;

namespace VirtualTrainer
{
    public class ExcelUtlity
    {
        static public List<objectProperty> GetOrderedObjectPropertyDetails<t>()
        {
            var info = typeof(t).GetProperties();
            List<objectProperty> objectProerties = new List<objectProperty>();
            foreach (PropertyInfo pi in info)
            {
                MemberInfo property = typeof(t).GetProperty(pi.Name);
                var displayNameAttribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().SingleOrDefault();
                string displayName = displayNameAttribute == null || string.IsNullOrEmpty(displayNameAttribute.DisplayName) ? property.Name : displayNameAttribute.DisplayName;

                var descriptionAttribute = property.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>().SingleOrDefault();
                bool gotDescription = false;
                int description = -1;
                gotDescription = descriptionAttribute != null ? int.TryParse(descriptionAttribute.Description, out description) : false;

                var dateStringFormatAttribute = property.GetCustomAttributes(typeof(DateFormatAttribute), true).Cast<DateFormatAttribute>().SingleOrDefault();
                string dateStringFormat = dateStringFormatAttribute == null || string.IsNullOrEmpty(dateStringFormatAttribute.StringFormat) ? String.Empty : dateStringFormatAttribute.StringFormat;

                if (gotDescription)
                {
                    objectProerties.Add(new objectProperty() { DisplayText = displayName, propertyName = pi.Name, DateStringFormat = dateStringFormat, Order = description, propertyType = pi.PropertyType });
                }
            }
            return objectProerties.OrderBy(e => e.Order).ToList();
        }
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }
        public static Type GetPropType(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetType();
        }
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        [System.AttributeUsage(System.AttributeTargets.Property)]
        public class DateFormatAttribute : System.Attribute
        {
            private string stringFormat;
            public string StringFormat
            {
                get { return this.stringFormat; }
            }
            public DateFormatAttribute(string stringFormat)
            {
                this.stringFormat = stringFormat;
            }
        }

        public string WriteObjectsToExcel(List<List<SqlRowField>> rowData)
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
            string fullProcessPath = ass.Location;
            string currentDir = Path.GetDirectoryName(fullProcessPath);
            string documentFullPath = string.Format(@"{0}\{1}.xlsx", currentDir.TrimEnd('\\'), Guid.NewGuid().ToString());

            FileInfo newFile = new FileInfo(documentFullPath);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(documentFullPath);
            }

            try
            {
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    if (rowData.Any())
                    {
                        // Add the headers
                        List<SqlRowField> fields = rowData[0];
                        for (int i = 0; i < fields.Count(); i++)
                        {
                            SqlRowField field = fields[i];
                            worksheet.Cells[1, i + 1].Value = field.Fieldame;
                        }

                        // Now add the row data
                        for (int rd = 0; rd < rowData.Count(); rd++)
                        {
                            List<SqlRowField> columns = rowData[rd];
                            for (int r = 0; r < columns.Count(); r++)
                            {
                                SqlRowField field = columns[r];
                                worksheet.Cells[rd + 2, r + 1].Value = field.Value;
                            }
                        }

                        for (int j = 0; j < fields.Count(); j++)
                        {
                            SqlRowField field = fields[j];
                            if (field.FieldType == typeof(DateTime))
                            {
                                ExcelRange range = worksheet.Cells[2, j + 1, rowData.Count(), j + 1];//.Style.Numberformat.Format = "DD/MM/YYYY";

                            }
                        }

                        //Create an autofilter for the range
                        //worksheet.Cells[string.Format("A1:{0}1", GetExcelColumnName(rowData[0].Count()))].AutoFilter = true;
                        worksheet.Cells.AutoFitColumns(0);
                        package.Save();
                    }
                    return documentFullPath;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // excelSheet = null;
                // excelworkBook = null;
                //if (System.IO.File.Exists(documentFullPath))
                //System.IO.File.Delete(documentFullPath);
            }
        }
        public string WriteObjectsToExcel<t>(List<t> objects, List<objectProperty> objectProperties, string DocumentSaveFullPath = "")
        {
            if (string.IsNullOrEmpty(DocumentSaveFullPath))
            {
                System.Reflection.Assembly ass = System.Reflection.Assembly.GetExecutingAssembly();
                string fullProcessPath = ass.Location;
                string currentDir = Path.GetDirectoryName(fullProcessPath);
                DocumentSaveFullPath = string.Format(@"{0}\{1}.xlsx", currentDir.TrimEnd('\\'), Guid.NewGuid().ToString());
            }          

            FileInfo newFile = new FileInfo(DocumentSaveFullPath);
            if (newFile.Exists)
            {
                newFile.Delete();  // ensures we create a new workbook
                newFile = new FileInfo(DocumentSaveFullPath);
            }

            try
            {
                using (ExcelPackage package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Sheet1");

                    int colCount = 1;
                    // Add the headers
                    foreach (objectProperty property in objectProperties)
                    {
                        worksheet.Cells[1, colCount].Value = property.DisplayText;

                        int rowCount = 2;

                        foreach (t emp in objects)
                        {
                            object propertyValue = GetPropValue(emp, property.propertyName);
                            worksheet.Cells[rowCount, colCount].Value = propertyValue;
                            rowCount++;
                        }

                        if (property.propertyType == typeof(DateTime) || property.propertyType.FullName.Contains("DateTime"))
                        {
                            worksheet.Cells[2, colCount, rowCount, colCount].Style.Numberformat.Format = string.IsNullOrEmpty(property.DateStringFormat) ? "dd-MMM-yy HH:mm:ss" : property.DateStringFormat;
                        }

                        colCount++;
                    }

                    //Create an autofilter for the range
                    worksheet.Cells[string.Format("A1:{0}1", GetExcelColumnName(objectProperties.Count()))].AutoFilter = true;
                    worksheet.Cells.AutoFitColumns(0);
                    package.Save();

                    return DocumentSaveFullPath;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                // excelSheet = null;
                // excelworkBook = null;
                //if (System.IO.File.Exists(documentFullPath))
                //System.IO.File.Delete(documentFullPath);
            }
        }
    }
    public class SqlRowField
    {
        public string Fieldame { get; set; }
        public string Value { get; set; }
        public Type FieldType { get; set; }
    }
    public class objectProperty
    {
        public string propertyName { get; set; }
        public string DisplayText { get; set; }
        public int Order { get; set; }
        public string DateStringFormat { get; set; }
        public Type propertyType { get; set; }
    }
}

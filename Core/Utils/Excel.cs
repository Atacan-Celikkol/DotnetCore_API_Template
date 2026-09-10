using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Core.Utilities.Excel
{
    public class Excel
    {
        private static readonly Lazy<Excel> Lazy = new Lazy<Excel>(() => new Excel());
        public static Excel Instance => Lazy.Value;

        #region DataToExcel

        public byte[] CreateExcelDocument<T>(List<T> sourceList)
        {
            var dataSet = new DataSet();
            dataSet.Tables.Add(ListToDataTable(sourceList));
            return CreateExcelDocumentAsStream(dataSet);
        }

        private byte[] CreateExcelDocumentAsStream(DataSet dataSet)
        {
            var stream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook, true))
            {
                WriteExcelFile(dataSet, document);
            }
            stream.Flush();
            stream.Position = 0;

            var data = new byte[stream.Length];

            stream.Read(data, 0, data.Length);
            stream.Close();
            return data;
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceList"></param>
        /// <returns></returns>
        private DataTable ListToDataTable<T>(List<T> sourceList)
        {
            var dataTable = new DataTable();
            if (sourceList == null)
            {
                return dataTable;
            }
            var properties = typeof(T).GetProperties();

            foreach (var propertyInfo in properties)
            {
                dataTable.Columns.Add(new DataColumn(propertyInfo.Name, GetNullableType(propertyInfo.PropertyType)));
            }

            foreach (var item in sourceList)
            {
                var row = dataTable.NewRow();

                foreach (var propertyInfo in properties)
                {
                    if (!IsNullableType(propertyInfo.PropertyType))
                    {
                        row[propertyInfo.Name] = propertyInfo.GetValue(item, null);
                    }
                    else
                    {
                        row[propertyInfo.Name] = (propertyInfo.GetValue(item, null) ?? DBNull.Value);
                    }
                }

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Type GetNullableType(Type type)
        {
            var returnType = type;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                returnType = Nullable.GetUnderlyingType(type);
            }
            return returnType;
        }

        /// <summary>
        /// https://msdn.microsoft.com/en-us/library/ms366789.aspx
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private bool IsNullableType(Type type)
        {
            return (type == typeof(string) ||
                   type.IsArray ||
                   (type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Nullable<>)));
        }

        private void WriteExcelFile(DataSet ds, SpreadsheetDocument spreadsheet)
        {
            //  Create the Excel file contents.  This function is used when creating an Excel file either writing
            //  to a file, or writing to a MemoryStream.
            spreadsheet.AddWorkbookPart();
            spreadsheet.WorkbookPart.Workbook = new Workbook();

            //  My thanks to James Miera for the following line of code (which prevents crashes in Excel 2010)
            var workbookView = new WorkbookView();
            var bookViews = new BookViews(workbookView);
            spreadsheet.WorkbookPart.Workbook.Append(bookViews);

            //  If we don't add a "WorkbookStylesPart", OLEDB will refuse to connect to this .xlsx file !
            var workbookStylesPart = spreadsheet.WorkbookPart.AddNewPart<WorkbookStylesPart>("rIdStyles");
            var stylesheet = new Stylesheet();
            workbookStylesPart.Stylesheet = stylesheet;

            //  Loop through each of the DataTables in our DataSet, and create a new Excel Worksheet for each.
            uint worksheetNumber = 1;
            foreach (DataTable dt in ds.Tables)
            {
                var newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet();

                // create sheet data
                newWorksheetPart.Worksheet.AppendChild(new SheetData());

                // save worksheet
                WriteDataTableToExcelWorksheet(dt, newWorksheetPart);
                newWorksheetPart.Worksheet.Save();

                // create the worksheet to workbook relation
                if (worksheetNumber == 1)
                    spreadsheet.WorkbookPart.Workbook.AppendChild(new Sheets());

                spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>().AppendChild(new Sheet()
                {
                    Id = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart),
                    SheetId = worksheetNumber,
                    Name = dt.TableName
                });

                worksheetNumber++;
            }
            spreadsheet.WorkbookPart.Workbook.Save();
        }

        private void WriteDataTableToExcelWorksheet(DataTable dataTable, WorksheetPart worksheetPart)
        {
            var worksheet = worksheetPart.Worksheet;
            var sheetData = worksheet.GetFirstChild<SheetData>();
            var numberOfColumns = dataTable.Columns.Count;
            var isNumericColumn = new bool[numberOfColumns];
            var excelColumnNames = new string[numberOfColumns];

            for (var n = 0; n < numberOfColumns; n++)
            {
                excelColumnNames[n] = GetExcelColumnName(n);
            }

            uint rowIndex = 1;
            var headerRow = new Row { RowIndex = rowIndex };

            sheetData.Append(headerRow);

            for (var colInx = 0; colInx < numberOfColumns; colInx++)
            {
                var col = dataTable.Columns[colInx];
                AppendTextCell(excelColumnNames[colInx] + "1", col.ColumnName, headerRow);
                isNumericColumn[colInx] = (col.DataType.FullName == "System.Decimal") || (col.DataType.FullName == "System.Int32");
            }

            foreach (DataRow dr in dataTable.Rows)
            {
                ++rowIndex;
                var newExcelRow = new Row { RowIndex = rowIndex };
                sheetData.Append(newExcelRow);
                for (var colInx = 0; colInx < numberOfColumns; colInx++)
                {
                    var cellValue = dr.ItemArray[colInx].ToString();

                    if (isNumericColumn[colInx])
                    {
                        double cellNumericValue;
                        if (!double.TryParse(cellValue, out cellNumericValue)) continue;
                        cellValue = cellNumericValue.ToString(CultureInfo.InvariantCulture);
                        AppendNumericCell(excelColumnNames[colInx] + rowIndex, cellValue, newExcelRow);
                    }
                    else
                    {
                        AppendTextCell(excelColumnNames[colInx] + rowIndex, cellValue, newExcelRow);
                    }
                }
            }
        }

        private void AppendTextCell(string cellReference, string cellStringValue, Row excelRow)
        {
            var cell = new Cell()
            {
                CellReference = cellReference,
                DataType = CellValues.String
            };
            var cellValue = new CellValue { Text = cellStringValue };
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        private void AppendNumericCell(string cellReference, string cellStringValue, Row excelRow)
        {
            var cell = new Cell()
            {
                CellReference = cellReference
            };
            var cellValue = new CellValue { Text = cellStringValue };
            cell.Append(cellValue);
            excelRow.Append(cell);
        }

        private string GetExcelColumnName(int columnIndex)
        {
            string columnName;
            if (columnIndex < 26)
            {
                columnName = ((char)('A' + columnIndex)).ToString();
                return columnName;
            }
            var firstChar = (char)('A' + (columnIndex / 26) - 1);
            var secondChar = (char)('A' + (columnIndex % 26));
            columnName = $"{firstChar}{secondChar}";
            return columnName;
        }

        #endregion DataToExcel

        #region ExcelToData

        public List<T> ReadExcelDocument<T>(byte[] byteArray)
        {
            var document = new MemoryStream(byteArray);
            var list = new List<T>();
            var spreadSheetDocument = SpreadsheetDocument.Open(document, false);

            using (spreadSheetDocument)
            {
                var sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                var relationshipId = sheets.First().Id.Value;
                var worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                var workSheet = worksheetPart.Worksheet;
                var sheetData = workSheet.GetFirstChild<SheetData>();
                var rows = sheetData.Descendants<Row>();

                var skip = true;
                foreach (var row in rows)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }

                    var data = Activator.CreateInstance(typeof(T));

                    var lenght = typeof(T).GetProperties().Length;

                    foreach (var property in typeof(T).GetProperties())
                    {
                        var cellIndex = GetIndex(spreadSheetDocument, rows.First(), property.Name);
                        var cell = (Cell)row.ElementAt(cellIndex);

                        var stringValue = GetCellValue(spreadSheetDocument, cell, property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTimeOffset), property.PropertyType);
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            if (stringValue == "$$$")
                            {
                                return list;
                            }
                            var typeConverter = TypeDescriptor.GetConverter(property.PropertyType);
                            var propValue = typeConverter.ConvertFromString(stringValue);
                            property.SetValue(data, propValue);
                        }
                    }
                    list.Add((T)data);
                }
            }
            return list;
        }

        private int GetIndex(SpreadsheetDocument document, Row firstRow, string columnName)
        {
            var i = 0;
            foreach (var openXmlElement in firstRow)
            {
                var cell = (Cell)openXmlElement;
                var cellvalue = GetCellValue(document, cell);
                if (string.Equals(columnName, cellvalue, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
                i++;
            }
            throw new ArgumentOutOfRangeException();
        }

        private string GetCellValue(SpreadsheetDocument document, Cell cell, bool isDate = false, Type type = null)
        {
            var stringTablePart = document.WorkbookPart.SharedStringTablePart;
            string value;
            try
            {
                value = cell.CellValue.InnerXml;
            }
            catch (Exception e)
            {
                throw e;
            }

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }

            if (isDate)
            {
                var ifdate = double.Parse(value);
                return DateTime.FromOADate(ifdate).ToString();
            }
            return value;
        }

        #endregion ExcelToData
    }
}
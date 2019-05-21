using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.Entities.VastMetaDB;
using IQ.Entities.VastDB.Helpers;

namespace IQ.Entities.VastDB
{
    /// <summary>
    /// Created:        01/29/2015
    /// Author:         David J. McKee
    /// Purpose:        This is a common response for Read or Search Requests.
    /// </summary>
    public class SearchResponse
    {
        public Table Table = null;                                  // Contains all meta information about the given entity
        public List<string[]> RowList = new List<string[]>();
        public int Page = 0;
        public int RowsPerPage = 0;
        public int TotalPages = 0;
        public int TotalRows = 0;

        public Table ChildTableRowTable = null;                                     // Contains all meta information about the ChildTableRowList
        public List<string[]> ChildTableRowList = null;

        public Table ClientActionRowTable = null;                                   // Contains all meta information about the ClientActionRowList
        public List<string[]> ClientActionRowList = null;

        public static void CopyRowList(SearchResponse source, SearchResponse dest)
        {
            foreach (string[] row in source.RowList)
            {
                dest.AppendRow(source.ReadRow(row));
            }
        }

        public IQ.Entities.VastMetaDB.Column ColumnByColumnID(string id)
        {
            return TableColumnByColumnID(this.Table, id);
        }

        public IQ.Entities.VastMetaDB.Column TableColumnByColumnID(Table table, string ID)
        {
            IQ.Entities.VastMetaDB.Column column = null;

            foreach (IQ.Entities.VastMetaDB.Column localColumn in table.ColumnList)
            {
                if (localColumn.ID == ID)
                {
                    column = localColumn;
                    break;
                }
            }

            return column;
        }

        private void EnsureTable(Dictionary<string, string> row)
        {
            if (this.Table == null)
            {
                this.LoadTableFromColumnIdValuePair(string.Empty, row);
            }

            if (this.Table != null)
            {
                if ((this.Table.ColumnList == null) ||
                        ((this.Table.ColumnList.Count == 0)))
                {
                    this.Table.ColumnList = new List<IQ.Entities.VastMetaDB.Column>();
                    int originalPosition = 0;

                    foreach (string key in row.Keys)
                    {
                        this.Table.ColumnList.Add(new IQ.Entities.VastMetaDB.Column(originalPosition, key));
                        originalPosition++;
                    }
                }
            }
        }

        /// <summary>
        /// Created:        08/31/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends an entire row in one show
        /// Critical:       The Column Schema ordinal position must start at 0!
        /// </summary>
        public void AppendRow(Dictionary<string, string> row)
        {
            EnsureTable(row);

            if (this.RowList == null)
            {
                this.RowList = new List<string[]>();
            }

            AppendTableRow(this.Table, this.RowList, row);
        }

        public void AppendTableRow(Table table, List<string[]> rowList, Dictionary<string, string> row)
        {
            string[] newRow = new string[table.ColumnList.Count];

            foreach (string key in row.Keys)
            {
                IQ.Entities.VastMetaDB.Column col = TableColumnByColumnID(table, key);

                if (col != null)
                {
                    newRow[col.Schema.OrdinalPosition] = row[key];
                }
            }

            rowList.Add(newRow);
        }

        /// <summary>
        /// Created:        10/06/2015
        /// Author:         David J. McKee
        /// Purpose:        Replaces and entire row in one shot
        /// Critical:       The Column Schema ordinal position must start at 0!
        /// </summary>
        public void ReplaceRow(int rowIndex, Dictionary<string, string> row)
        {
            string[] newRow = new string[this.Table.ColumnList.Count];

            foreach (string key in row.Keys)
            {
                IQ.Entities.VastMetaDB.Column col = this.ColumnByColumnID(key);

                if (col != null)
                {
                    newRow[col.Schema.OrdinalPosition] = row[key];
                }
            }

            if (this.RowList == null)
            {
                this.RowList = new List<string[]>();
            }

            this.RowList[rowIndex] = newRow;
        }

        public Dictionary<string, string> ReadRow(string[] row)
        {
            return ReadTableRow(this.Table, row);
        }

        public Dictionary<string, string> ReadTableRow(Table table, string[] row)
        {
            Dictionary<string, string> columnData = new Dictionary<string, string>();

            foreach (IQ.Entities.VastMetaDB.Column column in table.ColumnList)
            {
                try
                {
                    Column localColumn = TableColumnByColumnID(table, column.ID);

                    if (localColumn != null)
                    {
                        if (localColumn.Schema != null)
                        {
                            if (localColumn.Schema.OrdinalPosition < row.Length)
                            {
                                string value = row[localColumn.Schema.OrdinalPosition];

                                columnData.Add(column.ID, value);
                            }
                        }
                    }
                }
                catch { }
            }

            return columnData;
        }

        /// <summary>
        /// Created:        02/11/2016
        /// Author:         David J. McKee
        /// Purpose:        Sets a specific value in a specific row and column.  
        /// Critical:       rowIndex starts at 0
        /// </summary>        
        public void SetCellValue(string columnId, int rowIndex, string value)
        {
            if (this.RowList.Count >= rowIndex + 1)
            {
                // Step 1. Get the row data.
                string[] row = this.RowList[rowIndex];

                // Parse the row
                Dictionary<string, string> rowData = this.ReadRow(row);

                // Update the column
                if (rowData.ContainsKey(columnId))
                {
                    rowData[columnId] = value;

                    // Replace the current row.
                    this.ReplaceRow(rowIndex, rowData);
                }
            }
        }

        public string GetCellValue(string columnId, int rowIndex)
        {
            string result = string.Empty;

            if (this.RowList.Count >= rowIndex + 1)
            {
                // Step 1. Get the row data.
                string[] row = this.RowList[rowIndex];

                // Parse the row
                Dictionary<string, string> rowData = this.ReadRow(row);

                // Update the column
                if (rowData.ContainsKey(columnId))
                {
                    result = rowData[columnId];
                }
            }

            return result;
        }

        public void LoadTableFromColumnIdValuePair(string tableId,
                                                        Dictionary<string, string> columnIdValuePair)
        {
            this.Table = TableHelper.LoadTableFromColumnIdValuePair(tableId, columnIdValuePair, this.Table);
        }

        /// <summary>
        /// Created:        08/12/2015
        /// Author:         David J. McKee
        /// Purpose:        Returns the value of a given column.
        /// Important:      The default value is used when the column does not exist or when the value is empty.
        /// </summary>        
        public static string SafeReadString(Dictionary<string, string> columnGUIDValuePair, string key, string defaultValue)
        {
            string result = defaultValue;

            if (columnGUIDValuePair != null)
            {
                if (columnGUIDValuePair.ContainsKey(key))
                {
                    string colValue = columnGUIDValuePair[key].ToString();

                    if (string.IsNullOrWhiteSpace(colValue) == false)
                    {
                        result = colValue;
                    }
                }
            }

            return result;
        }


        public static string SafeReadString(Dictionary<string, string> columnGUIDValuePair, string key)
        {
            return SafeReadString(columnGUIDValuePair, key, string.Empty);
        }

        public static DateTime SafeReadDate(Dictionary<string, string> columnGUIDValuePair, string key, DateTime defaultValue)
        {
            DateTime result = defaultValue;

            string boolText = SafeReadString(columnGUIDValuePair, key);

            if (string.IsNullOrWhiteSpace(boolText) == false)
            {
                DateTime newBool;
                if (DateTime.TryParse(boolText, out newBool))
                {
                    result = newBool;
                }
            }

            return result;
        }

        public static bool SafeReadBool(Dictionary<string, string> columnGUIDValuePair, string key, bool defaultValue)
        {
            bool result = defaultValue;

            string boolText = SafeReadString(columnGUIDValuePair, key);

            if (string.IsNullOrWhiteSpace(boolText) == false)
            {
                bool newBool;
                if (bool.TryParse(boolText, out newBool))
                {
                    result = newBool;
                }
                else
                {
                    // Boolean values may be stored as integers 0 (false) and 1 (true).
                    if (boolText == "0")
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public static int SafeReadInt32(Dictionary<string, string> columnGUIDValuePair, string key, int defaultValue)
        {
            int result = defaultValue;

            string boolText = SafeReadString(columnGUIDValuePair, key);

            int newInt;
            if (int.TryParse(boolText, out newInt))
            {
                result = newInt;
            }

            return result;
        }

        public string FieldValueByID(string[] row, string Id)
        {
            string columnValue = string.Empty;

            IQ.Entities.VastMetaDB.Column column = this.ColumnByColumnID(Id);

            if (column != null)
            {
                columnValue = row[column.Schema.OrdinalPosition];
            }

            return columnValue;
        }

        public IQ.Entities.VastMetaDB.Column FieldByID(string id)
        {
            IQ.Entities.VastMetaDB.Column column = this.ColumnByColumnID(id);

            return column;
        }
    }
}

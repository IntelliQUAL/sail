using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.Entities.VastMetaDB;
using IQ.Entities.VastDB.Helpers;

namespace IQ.Entities.VastDB
{
    public class Entity
    {
        // Indicates how to handle the response e.g. Handle like a "New", "Create", "Read", "Update", "Delete" or "Search".
        // This will usually match the original request but is not required to.
        public string ActionResponseType = string.Empty;
        public string SpecificActionResponseType = string.Empty;                    // Used in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC to force an alternate action from the client.
        public string CustomAssemblyLineName = string.Empty;                        // Used in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC to force an alternate action from the client.
        public string TableID = null;
        public string ParentTableGUID = null;
        public string ParentTablePrimaryKey = null;
        public Dictionary<string, string> ColumnIdValuePair = null;                 // ColumnGUID (key) / Value Pair    
        public Table Table = null;                                                  // Contains all meta information about the given entity
        public bool UseTransaction = true;                                          // Use a database transaction on insert or update
        public List<IQ.Entities.VastMetaDB.Column> ChildTableColumnList = null;

        public Table ChildTableRowTable = null;                                     // Contains all meta information about the ChildTableRowList
        public List<string[]> ChildTableRowList = null;

        public Table ClientActionRowTable = null;                                   // Contains all meta information about the ClientActionRowList
        public List<string[]> ClientActionRowList = null;

        public string sysLoginID = string.Empty;                                    // Primary Key from the sysLogin table within the Master database
        public EntitySearch SearchInfo = null;                                      // Used in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC to force a search from the client.

        private string _primaryKey = null;

        public string PrimaryKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_primaryKey))
                {
                    _primaryKey = this.GetColumnValue(this.PrimaryKeyColumnID);
                }

                return _primaryKey;
            }
            set
            {
                _primaryKey = value;
            }
        }

        private string _primaryKeyColumnId = string.Empty;

        public string PrimaryKeyColumnID
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_primaryKeyColumnId))
                {
                    if (this.Table != null)
                    {
                        _primaryKeyColumnId = this.Table.PrimaryKeyColumnID;
                    }
                }

                return _primaryKeyColumnId;
            }
            set
            {
                _primaryKeyColumnId = value;

                if (this.Table != null)
                {
                    this.Table.PrimaryKeyColumnID = _primaryKeyColumnId;
                }
            }
        }

        public void SetColumnValue(string columnId, string value)
        {
            if (this.ColumnIdValuePair == null)
            {
                this.ColumnIdValuePair = new Dictionary<string, string>();
            }

            this.ColumnIdValuePair[columnId] = value;
        }

        public void SafeAddColumnValue(string columnName, object columnValue)
        {
            string newColumnValue = string.Empty;

            if (columnValue != null)
            {
                newColumnValue = columnValue.ToString();
            }

            SetColumnValue(columnName, newColumnValue);
        }

        public void SetDefaultColumnValue(string columnId, string value)
        {
            if (this.Table != null)
            {
                Column col = this.Table.GetColumn(columnId);

                if (col != null)
                {
                    if (col.Schema != null)
                    {
                        col.Schema.DefaultValue = value;
                    }
                }
            }
        }

        public string GetColumnValue(string columnId)
        {
            string value = null;

            if (this.ColumnIdValuePair != null)
            {
                if (this.ColumnIdValuePair.ContainsKey(columnId))
                {
                    value = this.ColumnIdValuePair[columnId];
                }
            }

            return value;
        }

        public IQ.Entities.VastMetaDB.Column ChildTableColumnByColumnID(string ID)
        {
            IQ.Entities.VastMetaDB.Column column = null;

            foreach (IQ.Entities.VastMetaDB.Column localColumn in this.ChildTableColumnList)
            {
                if (localColumn.ID == ID)
                {
                    column = localColumn;
                    break;
                }
            }

            return column;
        }

        public Dictionary<string, string> ReadRow(string[] row)
        {
            Dictionary<string, string> columnData = new Dictionary<string, string>();

            foreach (IQ.Entities.VastMetaDB.Column column in this.ChildTableColumnList)
            {
                try
                {
                    string value = row[ChildTableColumnByColumnID(column.ID).Schema.OrdinalPosition];

                    columnData.Add(column.ID, value);
                }
                catch { }
            }

            return columnData;
        }

        public void LoadTableFromColumnIdValuePair()
        {
            this.Table = TableHelper.LoadTableFromColumnIdValuePair(this.TableID, this.ColumnIdValuePair, this.Table);
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
    }
}

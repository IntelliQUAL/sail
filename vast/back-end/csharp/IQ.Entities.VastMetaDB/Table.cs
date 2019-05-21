using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using VastMetaDB.BaseClasses;

namespace IQ.Entities.VastMetaDB
{
    /// <summary>
    /// Created:        01/29/2015
    /// Author:         David J. McKee
    /// Purpose:        A Table contains one or more columns.
    /// </summary>
    public class Table : CommonBase
    {
        private string _tableOperationTableName = string.Empty;
        private string _tableOperationDatabaseName = string.Empty;

        public string ButtonText = string.Empty;                            // Primary Form Submit Button Text
        public string ViewType = string.Empty;

        public string AccountCode = string.Empty;                           // We used AccountCode here because this is NOT the full instance GUID
        public string DatabaseName = string.Empty;                          // Equates to a traditional database...Allows transition to another database / module.        
        public string ParentTableGUID = null;

        public List<Column> ColumnList = null;
        //public List<PrimaryKey> PrimaryKeyList = null;                    // In the future we may support multi-part primary keys but for now we use PrimaryColumnID
        public string PrimaryKeyColumnID = string.Empty;
        public List<ForeignKey> ForeignKeyList = null;
        public List<Index> IndexList = null;

        public TableDataType TableType = TableDataType.Standard;
        public string DefaultSortColumn;
        public IQ.Entities.VastMetaDB.Enums.SortOrder DefaultSortOrder;
        public string DataDisplayNameFormat;                                // Display format when table is presented as a list.
        public bool ShowFreeFormSearch = true;

        // Column Visibility Metadata Source
        public string ColumnVisibilityInstanceGUID = string.Empty;
        public string ColumnVisibilityDatabaseName = string.Empty;
        public string ColumnVisibilityTableName = string.Empty;

        // Table Schema Metadata Source
        public string TableSchemaInstanceGUID = string.Empty;
        public string TableSchemaDatabaseName = string.Empty;
        public string TableSchemaTableName = string.Empty;

        // Common sysTableOperation
        public string TableOperationInstanceGUID = string.Empty;
        public string TableOperationUrl = string.Empty;                 // If set, operation list and settings are pulled from a url.
        public string TableOperationDatabaseName
        {
            get
            {
                string tableOperationDatabaseName = _tableOperationDatabaseName;

                if (string.IsNullOrWhiteSpace(tableOperationDatabaseName))
                {
                    tableOperationDatabaseName = this.DatabaseName;
                }

                return tableOperationDatabaseName;
            }

            set
            {
                _tableOperationTableName = value;
            }
        }

        public string TableOperationTableName
        {
            get
            {
                string tableOperationTableName = _tableOperationTableName;

                if (string.IsNullOrWhiteSpace(tableOperationTableName))
                {
                    tableOperationTableName = this.ID;
                }

                return tableOperationTableName;
            }

            set
            {
                _tableOperationTableName = value;
            }
        }

        public bool AllowDragDropReorder = false;
        public bool AllowNew = true;                    // Is the end-user allowed to create a new record?
        public bool AllowRead = true;                   // Is the end-user allowed read any existing record. This normally results in an 'Edit' link on each individual row.
        public bool AllowDelete = true;                 // Is the end-user allowed to delete any existing record. This normally results in a 'Delete' link on each individual row.

        public string IconName = string.Empty;

        public Enums.AssemblyLineMode AssemblyLineMode = Enums.AssemblyLineMode.Replace;

        public IQ.Entities.VastMetaDB.AttributeData AttributeData = new IQ.Entities.VastMetaDB.AttributeData();

        public Column GetColumn(string columnId)
        {
            Column col = null;

            if (this.ColumnList != null)
            {
                foreach (var localCol in this.ColumnList)
                {
                    if (localCol.ID == columnId)
                    {
                        col = localCol;
                        break;
                    }
                }
            }

            return col;
        }

        public int MaxOrdinalPosition
        {
            get
            {
                int maxOrdinalPosition = 0;

                if ((this.ColumnList != null) && (this.ColumnList.Count > 0))
                {
                    foreach (IQ.Entities.VastMetaDB.Column col in this.ColumnList)
                    {
                        if (col.Schema.OrdinalPosition > maxOrdinalPosition)
                        {
                            maxOrdinalPosition = col.Schema.OrdinalPosition;
                        }
                    }
                }
                else
                {
                    maxOrdinalPosition = -1;    // So adding 1 equals 0
                }

                return maxOrdinalPosition;
            }
        }
    }
}


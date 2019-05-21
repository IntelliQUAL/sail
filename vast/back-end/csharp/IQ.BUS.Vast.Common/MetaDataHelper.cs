using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;
using IQ.Entities.VastMetaDB.Enums;

namespace IQ.BUS.Vast.Common
{
    public static class MetaDataHelper
    {
        public const string SCHEMA_TABLE_SUFFIX = "ColumnSchema";
        public const string COLUMN_VISIBILITY_TABLE_SUFFIX = "ColumnVisibility";

        private static List<IQ.Entities.VastMetaDB.Column> ColumnListFromNew(IContext context,
                                                                                string databaseName,
                                                                                string tableId)
        {
            List<IQ.Entities.VastMetaDB.Column> columnList = new List<IQ.Entities.VastMetaDB.Column>();

            IStandardTable newRepo = (IStandardTable)context.GetByName(Context.INTERFACE_STANDARD_TABLE);

            // Execute the Repository
            string instanceGUID = context.GetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID).ToString();

            IQ.Entities.VastDB.Entity dataModelEntity = newRepo.New(context, instanceGUID, databaseName, string.Empty, string.Empty, string.Empty, tableId);

            int ordinalPosition = 0;

            foreach (string colKey in dataModelEntity.ColumnIdValuePair.Keys)
            {
                // Critical OrdinalPosition comes from the New request. It is part of the table schema not meta data.
                IQ.Entities.VastMetaDB.Column column = new IQ.Entities.VastMetaDB.Column(ordinalPosition, colKey);
                column.Value = dataModelEntity.ColumnIdValuePair[colKey];

                column.Schema.DisplayName = column.ID;

                columnList.Add(column);

                ordinalPosition++;
            }

            return columnList;
        }

        public static IStandardTable ReadMetabaseName(IContext context, string databaseName, out string metabaseName)
        {
            metabaseName = ReadMetabaseName(databaseName);

            IStandardTable repo = (IStandardTable)context.GetByName(Context.INTERFACE_STANDARD_TABLE_OR_BLOB_METADATA);

            return repo;
        }

        public static string ReadMetabaseName(string databaseName)
        {
            string metabaseName = string.Empty;

            if (string.IsNullOrWhiteSpace(databaseName) == false)
            {
                metabaseName = databaseName + Metadata.METADATA_DATABASE_SUFFIX;
            }

            return metabaseName;
        }

        /// <summary>
        /// Created:        07/08/2015
        /// Author:         David J. McKee
        /// Purpose:        Returns all rows within the Schema Column Table for the given data table.
        /// </summary>        
        private static SearchResponse SearchColumnSchema(IContext context,
                                                            string instanceGUID,
                                                            string databaseName,
                                                            string tableId)
        {
            SearchResponse columnMetaDataList = null;

            try
            {
                string metabaseName;
                IStandardTable repo = ReadMetabaseName(context, databaseName, out metabaseName);
                string schemaTableName = tableId + SCHEMA_TABLE_SUFFIX;

                columnMetaDataList = repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, schemaTableName, null);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return columnMetaDataList;
        }

        /// <summary>
        /// Created:        07/08/2015
        /// Author:         David J. McKee
        /// Purpose:        Determines which fields or returned for each action type, the display name for each field and the display sequence.      
        /// </summary>        
        private static SearchResponse SearchColumnVisibility(IContext context,
                                                                string instanceGUID,
                                                                string databaseName,
                                                                string tableId,
                                                                string action)
        {
            EntitySearch searchCriteria = new EntitySearch();

            searchCriteria.TableID = tableId + COLUMN_VISIBILITY_TABLE_SUFFIX;
            searchCriteria.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_ACTION, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, action));

            string metabaseName;
            IStandardTable repo = ReadMetabaseName(context, databaseName, out metabaseName);

            SearchResponse columnMetaDataList = repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, searchCriteria.TableID, searchCriteria);

            return columnMetaDataList;
        }

        private static void AppendDataType(IContext context,
                                            Dictionary<string, string> columnIdValuePair,
                                            IQ.Entities.VastMetaDB.Column columnCopy,
                                            string sourceDataInstanceGUID,
                                            string sourceDataDatabaseName,
                                            string sourceDataTableId)
        {
            string schemaDataType = string.Empty;

            try
            {
                schemaDataType = DataHelper.SafeReadString(columnIdValuePair, "DataType");

                if (string.IsNullOrWhiteSpace(schemaDataType))
                {
                    schemaDataType = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_COLUMN_DATA_TYPE);
                }

                if (string.IsNullOrWhiteSpace(schemaDataType) == false)
                {
                    try
                    {
                        columnCopy.Schema.DataType = (IQ.Entities.VastMetaDB.Enums.ColumnDataType)Enum.Parse(typeof(IQ.Entities.VastMetaDB.Enums.ColumnDataType), schemaDataType);
                    }
                    catch (System.Exception ex2)
                    {
                        StringBuilder detail = new StringBuilder();

                        detail.AppendLine("instanceGUID: " + sourceDataInstanceGUID);
                        detail.AppendLine("databaseName: " + sourceDataDatabaseName);
                        detail.AppendLine("TableName: " + sourceDataTableId);
                        detail.AppendLine("ColumnDataType: " + schemaDataType);

                        context.Get<IExceptionHandler>().HandleException(context, ex2, detail.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex, "ColumnName: " + columnCopy.ID + ", and schemaDataType: " + schemaDataType);
            }
        }

        private static void AppendSchemaFormat(IContext context,
                                                Dictionary<string, string> columnIdValuePair,
                                                IQ.Entities.VastMetaDB.Column columnCopy)
        {
            try
            {
                string copySchemaFormat = DataHelper.SafeReadString(columnIdValuePair, Columns.COL_FORMAT);

                if (string.IsNullOrWhiteSpace(copySchemaFormat))
                {
                    copySchemaFormat = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_DATA_FORMAT);
                }

                if (string.IsNullOrWhiteSpace(copySchemaFormat) == false)
                {
                    columnCopy.Schema.Format = (IQ.Entities.VastMetaDB.DataFormat)Enum.Parse(typeof(IQ.Entities.VastMetaDB.DataFormat), copySchemaFormat);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        06/26/2015
        /// Author:         David J. McKee
        /// Purpose:        Builds an enum list from a given table.
        /// Important:      Meta Data about the table is used to determine the PrimaryKeyColumn and the DisplayNameFormat
        /// Updates:        09/28/2015 - Added 'valueColumn' to override the column the value is pulled from. If blank or null PrimaryKeyColumnID is used.
        /// </summary>
        public static string EnumFromTable(IContext context,
                                                IStandardTable repo,
                                                string instanceGuid,
                                                string databaseName,
                                                string tableName,
                                                string valueColumn)
        {
            StringBuilder enumText = new StringBuilder();

            try
            {
                SearchResponse searchResponse = repo.Search(context, instanceGuid, databaseName, string.Empty, string.Empty, tableName, null);

                if (searchResponse.RowList.Count > 0)
                {
                    // Pull the Meta Data for the given table.
                    Entities.VastMetaDB.Table table = new Entities.VastMetaDB.Table();
                    table.ID = tableName;
                    AppendTableMetaData(context, databaseName, table, Actions.ACTION_NEW);
                    table.ColumnList = ColumnListFromNew(context, databaseName, table.ID);

                    if (string.IsNullOrWhiteSpace(valueColumn))
                    {
                        valueColumn = table.PrimaryKeyColumnID;
                    }

                    foreach (var row in searchResponse.RowList)
                    {
                        Dictionary<string, string> rowDictionary = searchResponse.ReadRow(row);

                        enumText.Append(rowDictionary[valueColumn]);
                        enumText.Append("|");

                        if (string.IsNullOrWhiteSpace(table.DataDisplayNameFormat))
                        {
                            table.DataDisplayNameFormat = valueColumn;
                        }

                        string columnDisplayName = table.DataDisplayNameFormat;

                        const string COMMA_ESCPARE = "&#44;";

                        columnDisplayName = columnDisplayName.Replace(",", COMMA_ESCPARE);

                        foreach (var col in table.ColumnList)
                        {
                            columnDisplayName = columnDisplayName.Replace(col.ID, rowDictionary[col.ID]);
                        }

                        enumText.Append(columnDisplayName + ",");
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return enumText.ToString();
        }

        /// <summary>
        /// Created:        07/03/2015
        /// Author:         David J. McKee
        /// Purpose:        Parses a string in the encoded format (used by group names and enums)
        /// </summary>        
        public static Dictionary<string, string> ParseEncodedList(IContext context, string encodedText)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            try
            {
                string[] columnGroupInfoList = encodedText.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                const int ID = 0;
                const int DISPLAY_NAME = 1;

                foreach (string columnGroupInfo in columnGroupInfoList)
                {
                    string[] parts = columnGroupInfo.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    string key = parts[ID];

                    if (parts.Length > 1)
                    {
                        string value = parts[DISPLAY_NAME];
                        result[key] = value;
                    }
                    else
                    {
                        result[key] = key;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        private static void AppendSchemaInputType(IContext context,
                                                    Dictionary<string, string> columnIdValuePair,
                                                    IQ.Entities.VastMetaDB.Column columnCopy,
                                                    string sourceDataInstanceGUID,
                                                    string sourceDataDatabaseName,
                                                    string sourceDataTableId)
        {
            try
            {
                string copySchemaInputType = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_INPUT_TYPE);

                if (string.IsNullOrWhiteSpace(copySchemaInputType) == false)
                {
                    try
                    {
                        columnCopy.Schema.InputType = (IQ.Entities.VastMetaDB.DataInputType)Enum.Parse(typeof(IQ.Entities.VastMetaDB.DataInputType), copySchemaInputType, true);
                    }
                    catch (System.Exception ex2)
                    {
                        StringBuilder detail = new StringBuilder();

                        detail.AppendLine("instanceGUID: " + sourceDataInstanceGUID);
                        detail.AppendLine("databaseName: " + sourceDataDatabaseName);
                        detail.AppendLine("TableName: " + sourceDataTableId);
                        detail.AppendLine("COL_INPUT_TYPE: " + copySchemaInputType);

                        context.Get<IExceptionHandler>().HandleException(context, ex2, detail.ToString());
                    }

                    string instanceGUID = context.GetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID).ToString();
                    string databaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();

                    string metabaseName;
                    IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);

                    switch (columnCopy.Schema.InputType)
                    {
                        case Entities.VastMetaDB.DataInputType.UserPicker:
                            columnCopy.Schema.Enum = EnumFromTable(context, repo, instanceGUID, IQ.Entities.VastDB.Const.Databases.CAT_GUID_MASTER, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_LOGIN, null);
                            break;

                        case Entities.VastMetaDB.DataInputType.FormPicker:
                            // Reads all tables from the menu.                                                        
                            columnCopy.Schema.Enum = EnumFromTable(context, repo, instanceGUID, metabaseName, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_MENU, IQ.Entities.VastDB.Const.Columns.COL_GUID_FORM_ID);
                            break;

                        case Entities.VastMetaDB.DataInputType.RadioButtons:
                        case Entities.VastMetaDB.DataInputType.CheckBoxList:
                        case Entities.VastMetaDB.DataInputType.DropDown:
                        case Entities.VastMetaDB.DataInputType.SelectList:
                            if (string.IsNullOrWhiteSpace(columnCopy.Schema.Enum) &&
                                (string.IsNullOrWhiteSpace(columnCopy.Schema.ForeignTableID) == false))
                            {
                                columnCopy.Schema.Enum = EnumFromTable(context, repo, instanceGUID, databaseName, columnCopy.Schema.ForeignTableID.Trim(), null);

                                // Clear the ForeignTableID so the data is not also pulled from the client.
                                columnCopy.Schema.ForeignTableID = string.Empty;
                            }
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void AppendMinMax(IContext context,
                                            Dictionary<string, string> columnIdValuePair,
                                            IQ.Entities.VastMetaDB.Column columnCopy)
        {
            try
            {
                columnCopy.Schema.ValidateMinMax = Entity.SafeReadBool(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_VALIDATE_MIN_MAX, false);

                if (columnCopy.Schema.ValidateMinMax)
                {
                    try
                    {
                        string numberString = columnIdValuePair[IQ.Entities.VastDB.Const.Columns.COL_MAX].Trim();
                        numberString = numberString.Replace(",", string.Empty);

                        double valueAsDouble = Convert.ToDouble(numberString);

                        columnCopy.Schema.Max = Convert.ToInt32(valueAsDouble);
                    }
                    catch (System.Exception ex)
                    {
                        context.Get<IExceptionHandler>().HandleException(context, ex, "ColumnName: " + columnCopy.ID);
                    }

                    try
                    {
                        string minNumberString = columnIdValuePair[IQ.Entities.VastDB.Const.Columns.COL_MIN].Trim();
                        minNumberString = minNumberString.Replace(",", string.Empty);

                        double minValueAsDouble = Convert.ToDouble(minNumberString);

                        columnCopy.Schema.Min = Convert.ToInt32(minValueAsDouble);
                    }
                    catch (System.Exception ex)
                    {
                        context.Get<IExceptionHandler>().HandleException(context, ex, "ColumnName: " + columnCopy.ID);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        02/09/2015
        /// Author:         David J. McKee
        /// Purpose:        Reads the columns and related schemea
        /// Important:      does not load 'Value' or 'SensitiveValue'
        /// </summary>        
        public static List<IQ.Entities.VastMetaDB.Column> SearchColumnList(IContext context,
                                                                            string columnVisibilityInstanceGUID,
                                                                            string columnVisibilityDatabaseName,
                                                                            string columnVisibilityTableName,
                                                                            string tableId,
                                                                            string tablePrimaryKeyColumnId,
                                                                            string action)
        {
            List<IQ.Entities.VastMetaDB.Column> columnList = new List<IQ.Entities.VastMetaDB.Column>();

            try
            {
                // Critical OrdinalPosition comes from the New request. It is part of the table schema not meta data.
                string databaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();

                string finalDatabaseName = databaseName;

                // This logic must match that of IQ.OPS.Search.SearchRepo
                string metabaseName;
                switch (tableId)
                {
                    case IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_BRAND:
                        ReadMetabaseName(context, IQ.Entities.VastDB.Const.Databases.CAT_GUID_MASTER, out metabaseName);
                        finalDatabaseName = metabaseName;
                        break;

                    case IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_MENU:
                        if (databaseName.EndsWith(Metadata.METADATA_DATABASE_SUFFIX))
                        {
                            finalDatabaseName = databaseName;
                        }
                        else
                        {
                            MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);
                            finalDatabaseName = metabaseName;
                        }
                        break;

                    default:
                        finalDatabaseName = databaseName;
                        break;
                }

                columnList = ColumnListFromNew(context, finalDatabaseName, tableId);

                // Read the Visibility for the Column, DisplayName, and Sequence per action
                AppendColumnVisibilityMetaData(context, columnVisibilityInstanceGUID, columnVisibilityDatabaseName, columnVisibilityTableName, tablePrimaryKeyColumnId, ref columnList, action);

                // Read all the other meta data for the given field.
                // IMPORTANT: FOR NOW WE ASSUME THE columnVisibility* values also apply to the ColumnSchema table.
                AppendColumnSchemaMetaData(context, columnVisibilityInstanceGUID, columnVisibilityDatabaseName, columnVisibilityTableName, tablePrimaryKeyColumnId, columnList);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return columnList;
        }

        /// <summary>
        /// Created:        06/24/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends values from the *ColumnVisibility table     
        /// IMPORTANT:      instanceGUID and databaseName are specific to the VisibilityMetaData
        /// </summary>        
        private static void AppendColumnVisibilityMetaData(IContext context,
                                                            string instanceGUID,
                                                            string databaseName,
                                                            string tableId,
                                                            string tablePrimaryKeyColumnId,
                                                            ref List<IQ.Entities.VastMetaDB.Column> columnList,
                                                            string action)
        {
            try
            {
                // Set display sequence from the Meta Data
                SearchResponse columnVisibilityMetaDataList = SearchColumnVisibility(context, instanceGUID, databaseName, tableId, action);

                foreach (string[] columnMetaData in columnVisibilityMetaDataList.RowList)
                {
                    Dictionary<string, string> columnIdValuePair = columnVisibilityMetaDataList.ReadRow(columnMetaData);

                    string colId = DataHelper.SafeReadString(columnIdValuePair, Columns.COL_GUID_ID);

                    colId = colId.Replace(IQ.Entities.VastDB.Const.Columns.PRIMARY_KEY_PLACEHOLDER, tablePrimaryKeyColumnId);

                    foreach (IQ.Entities.VastMetaDB.Column localCol in columnList)
                    {
                        if (localCol.ID == colId)
                        {
                            localCol.Schema.Internal = (InternalVisibility)Entity.SafeReadInt32(columnIdValuePair, "Internal", (int)InternalVisibility.External);
                            localCol.Schema.DisplayName = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_GUID_DISPLAY_NAME);
                            localCol.Schema.DisplaySequence = Entity.SafeReadString(columnIdValuePair, "DisplaySequence");
                            break;
                        }
                    }
                }

                columnList = SortByDisplaySequence(context, columnList);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static List<Entities.VastMetaDB.Column> SortByDisplaySequence(IContext context,
                                                                                List<IQ.Entities.VastMetaDB.Column> columnList)
        {
            try
            {
                // Find the Max sequence
                double maxSeq = 0.0;

                foreach (IQ.Entities.VastMetaDB.Column localCol in columnList)
                {
                    double displaySequence;
                    if (double.TryParse(localCol.Schema.DisplaySequence, out displaySequence))
                    {
                        if (displaySequence > maxSeq)
                        {
                            maxSeq = displaySequence;
                        }
                    }
                }

                // Fill in all missing sequences.
                foreach (IQ.Entities.VastMetaDB.Column localCol in columnList)
                {
                    double displaySequence;
                    if (double.TryParse(localCol.Schema.DisplaySequence, out displaySequence))
                    {
                        if (displaySequence == 0)
                        {
                            localCol.Schema.DisplaySequence = maxSeq.ToString();
                            maxSeq++;
                        }
                    }
                    else
                    {
                        localCol.Schema.DisplaySequence = maxSeq.ToString();
                        maxSeq++;
                    }
                }

                // Sort Columns
                columnList = columnList.OrderBy(o => o.Schema.DisplaySequence).ToList();
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return columnList;
        }

        /// <summary>
        /// Created:        06/24/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends data from the *ColumnSchema table      
        /// </summary>        
        private static void AppendColumnSchemaMetaData(IContext context,
                                                        string instanceGUID,
                                                        string databaseName,
                                                        string tableId,
                                                        string tablePrimaryKeyColumnId,
                                                        List<IQ.Entities.VastMetaDB.Column> columnList)
        {
            SearchResponse columnSchemaMetaDataList = SearchColumnSchema(context, instanceGUID, databaseName, tableId);

            foreach (string[] columnMetaData in columnSchemaMetaDataList.RowList)
            {
                Dictionary<string, string> columnIdValuePair = columnSchemaMetaDataList.ReadRow(columnMetaData);

                string colId = DataHelper.SafeReadString(columnIdValuePair, Columns.COL_GUID_ID);

                foreach (IQ.Entities.VastMetaDB.Column localCol in columnList)
                {
                    if (localCol.ID == colId)
                    {
                        localCol.Schema.DefaultValue = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_DEFAULT_VALUE);
                        localCol.Schema.HasDefaultValue = Entity.SafeReadBool(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHMEA_HAS_DEFAULT_VALUE, false);
                        localCol.Schema.DateCreated = Entity.SafeReadDate(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_GUID_CREATED_DATE, DateTime.Now);
                        localCol.Schema.Description = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_GUID_DESC);
                        localCol.Schema.HelpTitle = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHMEA_HELP_TITLE);
                        localCol.Schema.HelpText = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHMEA_HELP_TEXT);
                        localCol.Schema.SortDescending = Entity.SafeReadBool(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_SORT_DESC, false);
                        localCol.Schema.Required = Entity.SafeReadBool(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_REQUIRED, false);

                        localCol.Schema.ForeignModuleID = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FOREIGN_MODULE_ID);
                        localCol.Schema.ForeignTableID = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FOREIGN_TABLE_ID);
                        localCol.Schema.ForeignTableSearchCriteria = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FOREIGN_TABLE_SEARCH_CRITERIA);
                        localCol.Schema.Pattern = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_PATTERN);
                        localCol.Schema.GroupID = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_GROUP_ID);
                        localCol.Schema.FilterEnabled = Entity.SafeReadBool(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FILTER_ENABLED, false);
                        localCol.Schema.FilterDefaultValue = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FILTER_DEFAULT_VALUE);
                        localCol.Schema.Enum = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_ENUM);
                        localCol.Schema.Placeholder = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_PLACEHOLDER);

                        AppendDataType(context, columnIdValuePair, localCol, instanceGUID, databaseName, tableId);
                        AppendSchemaFormat(context, columnIdValuePair, localCol);

                        AppendSchemaInputType(context, columnIdValuePair, localCol, instanceGUID, databaseName, tableId);

                        AppendMinMax(context, columnIdValuePair, localCol);
                        AppendFilterType(context, columnIdValuePair, localCol);

                        localCol.Schema.Unique = Entity.SafeReadBool(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_UNIQUE, false);
                        localCol.Schema.UniqueErrorMessage = Entity.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_UNIQUE_ERROR_MESSAGE, "Please specify a unique value for '" + localCol.Schema.DisplayName + "'");

                        // Set Field Layout Grid System (this is just like bootstrap)
                        SetFieldLayout(context, columnIdValuePair, localCol);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Created:        01/24/2016
        /// Author:         David J. McKee
        /// Purpose:        We follow the boostrap grid layout model
        /// </summary>        
        private static void SetFieldLayout(IContext context, Dictionary<string, string>
                                            columnIdValuePair,
                                            Entities.VastMetaDB.Column localCol)
        {
            try
            {
                localCol.Schema.Layout.xs = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_XS_GRID_COLS);
                localCol.Schema.Layout.sm = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_SM_GRID_COLS);
                localCol.Schema.Layout.md = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_MD_GRID_COLS);
                localCol.Schema.Layout.lg = DataHelper.SafeReadString(columnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_LG_GRID_COLS);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        06/22/2015
        /// Author:         David J. McKee
        /// Purpose:        Sets the FilterType based on the input type of the column.      
        /// </summary>        
        private static void AppendFilterType(IContext context, Dictionary<string, string> columnIdValuePair, Entities.VastMetaDB.Column localCol)
        {
            try
            {
                switch (localCol.Schema.InputType)
                {
                    case Entities.VastMetaDB.DataInputType.RadioButtons:
                    case Entities.VastMetaDB.DataInputType.DropDown:
                    case Entities.VastMetaDB.DataInputType.CheckBox:
                    case Entities.VastMetaDB.DataInputType.SelectList:
                    case Entities.VastMetaDB.DataInputType.UserPicker:
                    case Entities.VastMetaDB.DataInputType.FormPicker:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.SingleSelectList;
                        break;

                    case Entities.VastMetaDB.DataInputType.CheckBoxList:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.MultiSelectList;
                        break;

                    case Entities.VastMetaDB.DataInputType.DatePicker:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.DateRange;
                        break;

                    case Entities.VastMetaDB.DataInputType.DateTimePicker:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.DateTimeRange;
                        break;

                    case Entities.VastMetaDB.DataInputType.TimePicker:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.TimeRange;
                        break;

                    case Entities.VastMetaDB.DataInputType.Hidden:
                    case Entities.VastMetaDB.DataInputType.FileUpload:
                    case Entities.VastMetaDB.DataInputType.Map:
                    case Entities.VastMetaDB.DataInputType.Signature:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.NoFilter;
                        break;

                    case Entities.VastMetaDB.DataInputType.Lookup:
                    case Entities.VastMetaDB.DataInputType.LookupWithAdd:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.Lookup;
                        break;

                    case Entities.VastMetaDB.DataInputType.MaskedTextBox:
                    case Entities.VastMetaDB.DataInputType.MaskedTextBoxWithConfirm:
                    case Entities.VastMetaDB.DataInputType.MemoBox:
                    case Entities.VastMetaDB.DataInputType.Number:
                    case Entities.VastMetaDB.DataInputType.Spinner:
                    case Entities.VastMetaDB.DataInputType.Text:
                    case Entities.VastMetaDB.DataInputType.TextBox:
                    case Entities.VastMetaDB.DataInputType.URL:
                        localCol.Schema.FilterType = Entities.VastMetaDB.Enums.FilterType.FreeFormSearch;
                        break;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        04/16/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends meta data to the given table from sysTableSchema
        /// </summary>        
        public static void AppendTableMetaData(IContext context,
                                                    string databaseName,
                                                    IQ.Entities.VastMetaDB.Table output,
                                                    string action)
        {
            try
            {
                // Read the Schema Row for this table.
                string instanceGUID = context.GetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID).ToString();

                string metabaseName;
                IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);

                IQ.Entities.VastDB.Entity tableMetaData = repo.Read(context,
                                                                    instanceGUID,
                                                                    metabaseName,
                                                                    string.Empty,
                                                                    string.Empty,
                                                                    string.Empty,
                                                                    Tables.TABLE_GUID_SYS_TABLE_SCHEMA,
                                                                    Columns.COL_GUID_ID,
                                                                    output.ID);

                switch (action)
                {
                    case IQ.Entities.VastDB.Const.Actions.ACTION_NEW:
                        output.DisplayName = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_NEW_DISPLAY_NAME);
                        output.ButtonText = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_NEW_BUTTON_TEXT);
                        break;

                    case IQ.Entities.VastDB.Const.Actions.ACTION_CREATE:
                        output.DisplayName = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_CREATE_DISPLAY_NAME);
                        output.ButtonText = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_CREATE_BUTTON_TEXT);
                        break;

                    case IQ.Entities.VastDB.Const.Actions.ACTION_READ:
                        output.DisplayName = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_UPDATE_DISPLAY_NAME);
                        output.ButtonText = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_UPDATE_BUTTON_TEXT);
                        break;

                    case IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH:
                        output.DisplayName = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_GRID_DISPLAY_NAME);
                        output.ButtonText = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_NEW_BUTTON_TEXT);
                        break;
                }

                output.AssemblyLineMode = ReadAssemblyLineMode(context, tableMetaData, action);

                output.IconName = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_ICON_NAME);

                // We assume at this point that each table must have single primary key column
                string primaryKeyColumnId = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_PRIMARY_KEY_COLUMN_ID);

                if (string.IsNullOrWhiteSpace(primaryKeyColumnId) == false)
                {
                    output.PrimaryKeyColumnID = primaryKeyColumnId;
                }
                else if (string.IsNullOrWhiteSpace(output.PrimaryKeyColumnID))
                {
                    // WARNING: By convention we append "ID" to the table name in order to create the primary key
                    // 12/8/2016 - DJM: We now do this here automatically because it is a pain to chase this down every time someone forgets to configure it.
                    output.PrimaryKeyColumnID = output.ID + "ID";
                }

                output.DataDisplayNameFormat = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_DATA_DISPLAY_NAME_FORMAT);

                output.ColumnGroupIdList = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_GROUP_ID_LIST);

                // Default Sort Information
                output.DefaultSortColumn = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_DEFAULT_SORT_COLUMN);

                output.AllowDragDropReorder = Entity.SafeReadBool(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_ALLOW_DRAG_DROP_REORDER, false);
                output.AllowNew = Entity.SafeReadBool(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_ALLOW_NEW, true);
                output.AllowRead = Entity.SafeReadBool(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_ALLOW_READ, true);
                output.AllowDelete = Entity.SafeReadBool(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_ALLOW_DELETE, true);

                string defaultSortOrderText = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_DEFAULT_SORT_ORDER);

                if (string.IsNullOrWhiteSpace(defaultSortOrderText) == false)
                {
                    IQ.Entities.VastMetaDB.Enums.SortOrder sortOrder;
                    if (Enum.TryParse<IQ.Entities.VastMetaDB.Enums.SortOrder>(defaultSortOrderText, out sortOrder))
                    {
                        output.DefaultSortOrder = sortOrder;
                    }
                }

                // ShowFreeFormSearch
                output.ShowFreeFormSearch = Entity.SafeReadBool(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_SHOW_FREE_FORM_SEARCH, true);

                // Common *ColumnVisibility
                output.ColumnVisibilityInstanceGUID = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_VISIBILITY_INSTANCE_GUID, instanceGUID);
                output.ColumnVisibilityDatabaseName = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_VISIBILITY_DATABASE_NAME, databaseName);
                output.ColumnVisibilityTableName = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_VISIBILITY_TABLE_NAME, output.ID);

                // Common *ColumnSchema
                output.TableSchemaInstanceGUID = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_INSTANCE_GUID, instanceGUID);
                output.TableSchemaDatabaseName = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_DATABASE_NAME, databaseName);
                output.TableSchemaTableName = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_TABLE_NAME, output.ID);

                // Common sysTableOperation
                output.TableOperationInstanceGUID = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_OPERATION_INSTANCE_GUID, output.TableSchemaInstanceGUID);
                output.TableOperationDatabaseName = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_OPERATION_DATABASE_NAME, output.TableSchemaDatabaseName);
                output.TableOperationTableName = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_OPERATION_TABLE_NAME, output.TableSchemaTableName);
                output.TableOperationUrl = Entity.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_COLUMN_SYS_TABLE_OPERATION_URL, string.Empty);

                // Append Additional Attributes.
                AppendAdditionalAttributes(context, tableMetaData, output);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        03/18/2016
        /// Author:         David J. McKee
        /// Purpose:        Translates each column to a Key, and each column value to a value
        /// Client example:
        ///     // Loop through each row adding each value.
        ///     angular.forEach(data.AttributeList, function (rowValue, rowKey) {
        ///        var key = ReadFieldValue("Key", rowValue, data.FieldSchemaList);
        ///        var value = ReadFieldValue("Value", rowValue, data.FieldSchemaList);
        ///
        ///        $scope.viewData[viewName].Attributes[key] = value;
        ///
        ///    });
        /// </summary>        
        private static void AppendAdditionalAttributes(IContext context, Entity tableMetaData, IQ.Entities.VastMetaDB.Table output)
        {
            try
            {
                if (tableMetaData.ColumnIdValuePair != null)
                {
                    foreach (string key in tableMetaData.ColumnIdValuePair.Keys)
                    {
                        string[] keyValue = new string[2] { key, tableMetaData.ColumnIdValuePair[key] };
                        output.AttributeData.RowList.Add(keyValue);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static AssemblyLineMode ReadAssemblyLineMode(IContext context, Entity tableMetaData, string action)
        {
            AssemblyLineMode assemblyLineMode = AssemblyLineMode.Replace;

            try
            {
                string assemblyLineModeString = DataHelper.SafeReadString(tableMetaData.ColumnIdValuePair, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + action, AssemblyLineMode.Replace.ToString());

                AssemblyLineMode newAssemblyLineMode;
                if (Enum.TryParse<AssemblyLineMode>(assemblyLineModeString, out newAssemblyLineMode))
                {
                    assemblyLineMode = newAssemblyLineMode;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return assemblyLineMode;
        }

        public static bool ExcludeInternalColumns(IContext context,
                                                    IQ.Entities.VastMetaDB.Table table,
                                                    out List<IQ.Entities.VastMetaDB.Column> removedColumnList)
        {
            bool anyHiddenFound = false;
            removedColumnList = new List<Entities.VastMetaDB.Column>();

            try
            {
                bool hiddenFound = true;

                while (hiddenFound)
                {
                    hiddenFound = false;

                    foreach (var column in table.ColumnList)
                    {
                        if (column.Schema != null)
                        {
                            if (column.Schema.Internal == InternalVisibility.Internal)
                            {
                                hiddenFound = true;
                                anyHiddenFound = true;
                                removedColumnList.Add(column);
                                table.ColumnList.Remove(column);
                                break;
                            }
                            else if (column.Schema.Internal == InternalVisibility.ExternalHidden)
                            {
                                column.Schema.InputType = Entities.VastMetaDB.DataInputType.Hidden;
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return anyHiddenFound;
        }

        /// <summary>
        /// Created:        08/04/2015
        /// Author:         David J. McKee
        /// Purpose:        Removes invalid chars relative the a file or path.
        /// </summary>        
        public static string RemoveInvalidChars(string text)
        {
            text = text.Replace(" ", string.Empty).Replace("-", string.Empty).Replace(".", string.Empty).Replace("'", string.Empty);

            foreach (char invalidPathChar in System.IO.Path.GetInvalidPathChars())
            {
                text.Replace(invalidPathChar.ToString(), string.Empty);
            }

            foreach (char invalidFilenameCahr in System.IO.Path.GetInvalidFileNameChars())
            {
                text.Replace(invalidFilenameCahr.ToString(), string.Empty);
            }

            return text.Trim();
        }
    }
}

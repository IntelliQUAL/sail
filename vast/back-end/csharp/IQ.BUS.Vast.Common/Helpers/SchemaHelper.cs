using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.Entities.VastDB;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.Vast.Common.Helpers
{
    public static class SchemaHelper
    {
        public static void EnsureTable(IContext context,
                                        string tableId,
                                        string primaryKeyColumnId,
                                        string instanceGUID,
                                        string databaseName,
                                        bool addMenuEntry,
                                        bool useTierTwoSysTableSchema)
        {
            if (string.IsNullOrWhiteSpace(tableId) == false)
            {
                string rootDatabaseName;
                Entities.VastDB.Entity tempEntity;
                FindTableInRootDatabase(context, databaseName, out tableId, out rootDatabaseName, out tempEntity);

                IStandardTable standardRepo = (IStandardTable)context.GetByName(IQ.Entities.VastDB.Const.Context.INTERFACE_STANDARD_TABLE);

                ITableManager tableManager = (ITableManager)standardRepo;

                IQ.Entities.VastMetaDB.Table newTable = new Entities.VastMetaDB.Table();

                newTable.ID = tableId;
                newTable.DisplayName = newTable.ID;
                newTable.PrimaryKeyColumnID = primaryKeyColumnId;
                newTable.ColumnList = new List<Entities.VastMetaDB.Column>();

                if (string.IsNullOrWhiteSpace(rootDatabaseName) == false)
                {
                    // Ensure the physical table.
                    if (tableManager.EnsureTable(context, instanceGUID, rootDatabaseName, newTable))
                    {
                        string metabaseName;
                        IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, rootDatabaseName, out metabaseName);

                        // Create the ColumnVisibility table
                        SchemaHelper.EnsureColumnVisibilityTable(context, tableId, tableManager, instanceGUID, metabaseName, repo);

                        // Create the ColumnSchema table
                        SchemaHelper.EnsureColumnSchemaTable(context, tableId, tableManager, instanceGUID, metabaseName, repo);

                        IQ.Entities.VastDB.Entity parentMenu = null;

                        if (addMenuEntry)
                        {
                            // Create an Entry in the Menu
                            parentMenu = SchemaHelper.EnsureSysMenuEntry(context, instanceGUID, rootDatabaseName, newTable);
                        }

                        if (useTierTwoSysTableSchema)
                        {
                            // Ensure*ColumnVisiblity and *ColumnSchema for sysTableSchema within the *MetabaseMetabase
                            IQ.BUS.Vast.Common.DataHelper.EnsureTierTwoSysTableSchema(context, instanceGUID, databaseName, newTable.ID, parentMenu, repo, string.Empty);
                        }
                    }
                }
            }
        }

        public static void EnsureColumnVisibilityTable(IContext context,
                                                        string baseTableId,
                                                        ITableManager tableManager,
                                                        string instanceGUID,
                                                        string metabaseName,
                                                        IStandardTable repo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baseTableId) == false)
                {
                    string tableId = baseTableId + "ColumnVisibility";

                    IQ.Entities.VastMetaDB.Table newTable = new Entities.VastMetaDB.Table();
                    newTable.ID = tableId;
                    newTable.PrimaryKeyColumnID = tableId + "ID";
                    newTable.ColumnList = new List<Entities.VastMetaDB.Column>();

                    tableManager.EnsureTable(context, instanceGUID, metabaseName, newTable);

                    EnsureTableColumns(context, repo, instanceGUID, metabaseName, tableId,
                                        Columns.COL_GUID_ID,
                                        Columns.COL_GUID_ACTION,
                                        Columns.COL_GUID_INTERNAL,
                                        Columns.COL_GUID_DISPLAY_NAME,
                                        Columns.COL_GUID_DISPLAY_SEQUENCE);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void EnsureColumnSchemaTable(IContext context,
                                                        string baseTableId,
                                                        ITableManager tableManager,
                                                        string instanceGUID,
                                                        string metabaseName,
                                                        IStandardTable repo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(baseTableId) == false)
                {
                    string tableId = baseTableId + IQ.BUS.Vast.Common.MetaDataHelper.SCHEMA_TABLE_SUFFIX;

                    IQ.Entities.VastMetaDB.Table newTable = new Entities.VastMetaDB.Table();
                    newTable.ID = tableId;
                    newTable.PrimaryKeyColumnID = tableId + "ID";
                    newTable.ColumnList = new List<Entities.VastMetaDB.Column>();

                    tableManager.EnsureTable(context, instanceGUID, metabaseName, newTable);

                    EnsureTableColumns(context, repo, instanceGUID, metabaseName, tableId,
                                        Columns.COL_GUID_ID,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_DEFAULT_VALUE,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHMEA_HAS_DEFAULT_VALUE,
                                        IQ.Entities.VastDB.Const.Columns.COL_GUID_CREATED_DATE,
                                        IQ.Entities.VastDB.Const.Columns.COL_GUID_DESC,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHMEA_HELP_TITLE,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHMEA_HELP_TEXT,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_SORT_DESC,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_REQUIRED,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FOREIGN_MODULE_ID,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FOREIGN_TABLE_ID,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FOREIGN_TABLE_SEARCH_CRITERIA,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_PATTERN,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_GROUP_ID,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FILTER_ENABLED,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_FILTER_DEFAULT_VALUE,
                                        IQ.Entities.VastDB.Const.Columns.COL_SCHEMA_ENUM,
                                        IQ.Entities.VastDB.Const.Columns.COL_PLACEHOLDER,
                                        IQ.Entities.VastDB.Const.Columns.COL_COLUMN_DATA_TYPE,
                                        IQ.Entities.VastDB.Const.Columns.COL_DATA_FORMAT,
                                        IQ.Entities.VastDB.Const.Columns.COL_INPUT_TYPE,
                                        IQ.Entities.VastDB.Const.Columns.COL_VALIDATE_MIN_MAX,
                                        IQ.Entities.VastDB.Const.Columns.COL_MAX,
                                        IQ.Entities.VastDB.Const.Columns.COL_MIN,
                                        IQ.Entities.VastDB.Const.Columns.COL_UNIQUE,
                                        IQ.Entities.VastDB.Const.Columns.COL_UNIQUE_ERROR_MESSAGE,
                                        IQ.Entities.VastDB.Const.Columns.COL_XS_GRID_COLS,
                                        IQ.Entities.VastDB.Const.Columns.COL_SM_GRID_COLS,
                                        IQ.Entities.VastDB.Const.Columns.COL_MD_GRID_COLS,
                                        IQ.Entities.VastDB.Const.Columns.COL_LG_GRID_COLS);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        06/24/2015
        /// Author:         David J. McKee
        /// Purpose:        Creates a menu entry to allow for interacting with the spreadsheet data      
        /// </summary>        
        public static IQ.Entities.VastDB.Entity EnsureSysMenuEntry(IContext context,
                                                string instanceGUID,
                                                string databaseName,
                                                Entities.VastMetaDB.Table newTable)
        {
            IQ.Entities.VastDB.Entity newMenuItem = new Entities.VastDB.Entity();

            try
            {
                newMenuItem.ColumnIdValuePair = new Dictionary<string, string>();

                //newMenuItem.SetColumnValue("ParentSysMenuID", ""); // null
                newMenuItem.SetColumnValue("DisplaySequence", "99");
                newMenuItem.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_DISPLAY_NAME, newTable.DisplayName);
                newMenuItem.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_FORM_ID, newTable.ID);
                newMenuItem.SetColumnValue("ActionType", "Search");
                newMenuItem.SetColumnValue("MenuStyle", "VNav");
                newMenuItem.SetColumnValue("ClassText", "btn btn-w-md btn-gap-v btn-round btn-warning");

                string metabaseName;
                IStandardTable standardRepo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);

                standardRepo.Create(context, instanceGUID, metabaseName, string.Empty, string.Empty, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_MENU, newMenuItem);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return newMenuItem;
        }

        public static void FindTableInRootDatabase(IContext context,
                                                        string matabaseDatabaseName,
                                                        out string tableId,
                                                        out string rootDatabaseName,
                                                        out Entities.VastDB.Entity tempEntity)
        {
            // Backup the current database in context
            string originalContextDatabaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();
            tempEntity = new Entity();
            rootDatabaseName = string.Empty;
            tableId = string.Empty;

            try
            {
                if ((context.Payload<Entity>() != null) &&
                    (context.Payload<Entity>().Table != null))
                {
                    // Search All Columns from the destination table
                    tableId = context.Payload<Entity>().Table.ID.Substring(0, context.Payload<Entity>().Table.ID.Length - IQ.BUS.Vast.Common.MetaDataHelper.SCHEMA_TABLE_SUFFIX.Length);
                    rootDatabaseName = matabaseDatabaseName.Substring(0, matabaseDatabaseName.Length - Metadata.METADATA_DATABASE_SUFFIX.Length);

                    // Set the passed-in database as the current context database
                    context.SetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID, rootDatabaseName);

                    FlowTransport<Entities.VastDB.Entity> newEntityContext;
                    List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList;
                    IQ.BUS.Vast.Common.AppendRequestTableSchema.InitTableForOperationList(context, rootDatabaseName, tableId, tempEntity, out newEntityContext, out operationList);

                    IQ.BUS.Vast.Common.DefaultOerationLists.AppendEntityOperationInstance(context, "IQ.BUS.Vast.Common.AppendTableSchema", operationList);

                    foreach (Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operation in operationList)
                    {
                        IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operationToExec = operation.OperationInstance;
                        operationToExec.Execute(context, newEntityContext.Payload, ref tempEntity);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                // Restore the database
                context.SetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID, originalContextDatabaseName);
            }
        }

        /// <summary>
        /// Created:        08/14/2015
        /// Author:         David J. McKee
        /// Purpose:        Ensures that the sysMenu table and all related fields exist.
        /// </summary>        
        public static void EnsureTableColumns(IContext context,
                                                IStandardTable repo,
                                                string instanceGUID,
                                                string databaseName,
                                                string tableId,
                                                params string[] columnNameList)
        {
            try
            {
                ITableManager tableManager = (ITableManager)repo;

                // Ensure the table exists.
                IQ.Entities.VastMetaDB.Table table = new IQ.Entities.VastMetaDB.Table();
                table.ID = tableId;
                table.PrimaryKeyColumnID = table.ID + "ID";
                table.ColumnList = new List<Entities.VastMetaDB.Column>();
                tableManager.EnsureTable(context, instanceGUID, databaseName, table);

                // Read the table existing table.
                IQ.Entities.VastDB.Entity newEntity = repo.New(context, instanceGUID, databaseName, string.Empty, string.Empty, string.Empty, tableId);

                List<string> missingColumns = new List<string>();

                int ordinalPosition = newEntity.ColumnIdValuePair.Keys.Count + 1;

                foreach (string columnName in columnNameList)
                {
                    if (newEntity.ColumnIdValuePair.ContainsKey(columnName) == false)
                    {
                        IQ.Entities.VastMetaDB.Column newColumn = IQ.BUS.Vast.Common.DataHelper.AppendColumn(tableId, newEntity.Table, ordinalPosition, columnName, false);

                        tableManager.EnsureColumn(context, instanceGUID, databaseName, tableId, newColumn.Schema);

                        ordinalPosition++;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }
    }
}

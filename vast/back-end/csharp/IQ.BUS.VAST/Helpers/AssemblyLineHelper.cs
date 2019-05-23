using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.Entities.VastDB;
using IQ.Entities.VastMetaDB;
using IQ.Entities.VastDB.Const;
using IQ.BUS.Vast.Common.Consts;
using IQ.BUS.Vast.Common.Helpers;
using IQ.RepositoryInterfaces.Vast;
using IQ.Entities.VastMetaDB.Enums;
using IQ.Entities.VastMetaDB.SqlEsque;

namespace IQ.BUS.Vast.Helpers
{
    public class AssemblyLineHelper<I, O>
    {
        private const int NULL_ID = -1;

        public void AppendStandardOperations(FlowTransport<I> context,
                                                List<Operation<I, O>> operationList,
                                                string action)
        {
            try
            {
                // Step 1. Build the search request
                IQ.Entities.VastDB.EntitySearch operationSearch = new EntitySearch();
                operationSearch.TableID = Tables.TABLE_GUID_SYS_STANDARD_OPERATION;
                operationSearch.Where = new Where(new Predicate(Columns.COL_GUID_ACTION, OperatorType.EqualTo, action));

                OrderBy orderBy = new OrderBy();
                orderBy.SortColumn = Columns.COL_GUID_SEQUENCE;
                orderBy.SortOrder = SortOrder.Asc;

                operationSearch.OrderBy = orderBy;
                operationSearch.Page = 1;
                operationSearch.RowsPerPage = 100;

                // Step 2. Execute the search request.
                string metabaseName;
                IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, Databases.IQ_CLOUD_DATABASE_GUID, out metabaseName);

                string instanceGUID = ViewModelToDataModel.BuildInstanceGUID(Root.IQ_CLOUD_DOMAIN, string.Empty);

                SearchResponse searchResponse =
                    repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_STANDARD_OPERATION, operationSearch);

                const string COL_OPERATION_ID = "OperationID";

                foreach (string[] row in searchResponse.RowList)
                {
                    Dictionary<string, string> rowData = searchResponse.ReadRow(row);

                    string operationId = rowData[COL_OPERATION_ID];

                    IQ.Entities.VastDB.Entity entity = repo.Read(context, instanceGUID, metabaseName, string.Empty, string.Empty, string.Empty,
                                                                Tables.TABLE_GUID_SYS_OPERATION, Columns.COL_GUID_OPERATION_ID, operationId);

                    string operationFullName = Entity.SafeReadString(entity.ColumnIdValuePair, Columns.COL_GUID_FULL_NAME).Trim();
                    bool async = Entity.SafeReadBool(entity.ColumnIdValuePair, Columns.COL_GUID_ASYNC, false);

                    IOperation<I, O> operationInstance = context.Get<IServiceLocator>().LocateByName<IOperation<I, O>>(context, operationFullName);

                    Operation<I, O> operation = new Operation<I, O>();
                    operation.Async = async;

                    // IMPORTANT: FOR PERFORMANCE STANDARD OPERATIONS DO NOT ALLOW FOR CONFIGURATION
                    operation.ConfigSourceID = string.Empty;
                    operation.ConfigSourceKey = string.Empty;
                    operation.OperationID = Convert.ToInt32(operationId);
                    operation.OperationInstance = operationInstance;
                    operation.Sequence = Convert.ToInt32(rowData[Columns.COL_GUID_SEQUENCE]);

                    operationList.Add(operation);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public bool UseTableAssemblyLine(IContext context,
                                            IQ.Entities.VastMetaDB.Table table,
                                            string action,
                                            List<Operation<I, O>> operationList)
        {
            bool useTableAssemblyLine = false;

            try
            {
                if (string.IsNullOrWhiteSpace(table.TableOperationUrl) == false)
                {
                    // Pull operation list and operation settings from a url.
                }
                else
                {

                    if (string.IsNullOrWhiteSpace(table.TableOperationDatabaseName))
                    {
                        table.TableOperationDatabaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();
                    }

                    if (string.IsNullOrWhiteSpace(table.TableOperationDatabaseName) == false)
                    {
                        // Query the database for Operations for the given table.
                        IQ.Entities.VastDB.EntitySearch entitySearch = new IQ.Entities.VastDB.EntitySearch();

                        IQ.Entities.VastMetaDB.SqlEsque.Predicate predicate =
                            new IQ.Entities.VastMetaDB.SqlEsque.Predicate(IQ.Entities.VastDB.Const.Columns.COL_GUID_ACTION, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, action);

                        entitySearch.Where =
                            new IQ.Entities.VastMetaDB.SqlEsque.Where(predicate,
                                new IQ.Entities.VastMetaDB.SqlEsque.AND(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(IQ.Entities.VastDB.Const.Columns.COL_GUID_TABLE_ID,
                                    IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, table.TableOperationTableName)));

                        string metabaseName;
                        IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, table.TableOperationDatabaseName, out metabaseName);

                        IQ.Entities.VastDB.SearchResponse searchResponse =
                            repo.Search(context, table.TableOperationInstanceGUID, metabaseName, string.Empty, string.Empty, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_TABLE_OPERATION, entitySearch);

                        // Set the ConfigSource
                        context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE, Tables.TABLE_GUID_SYS_TABLE_OPERATION_SETTING);
                        context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_KEY, Columns.COL_GUID_SYS_TABLE_OPERATION_ID);
                        context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_INSTANCE_GUID, table.TableOperationInstanceGUID);
                        context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_DATABASE_NAME, table.TableOperationDatabaseName);

                        LoadOperationsFromList(context, table.TableOperationInstanceGUID, table.TableOperationDatabaseName, operationList, searchResponse);

                        if (operationList.Count > 0)
                        {
                            useTableAssemblyLine = true;
                        }
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();

                        // Trace Warning
                        StackTrace stackTrace = new StackTrace();           // get call stack
                        StackFrame[] stackFrames = stackTrace.GetFrames();  // get method calls (frames)

                        // write call stack method names
                        foreach (StackFrame stackFrame in stackFrames)
                        {
                            stringBuilder.Append("->" + stackFrame.GetMethod().Name);   // write method name
                        }

                        context.Get<ITrace>().Emit(TraceLevel.Info, "Blank database sent to UseTableAssemblyLine: " + stringBuilder.ToString());
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return useTableAssemblyLine;
        }

        /// <summary>
        /// Created:        04/24/2015
        /// Author:         David J. McKee
        /// Purpose:        Loads operations from a given search result.
        /// Important:      Consistent between sysTableOperaion and sysCustomALOperation
        /// </summary>        
        private void LoadOperationsFromList(IContext context,
                                                    string instanceGUID,
                                                    string databaseName,
                                                    List<Operation<I, O>> operationList,
                                                    IQ.Entities.VastDB.SearchResponse searchResponse)
        {
            try
            {
                List<Operation<I, O>> operationObjectList = new List<Operation<I, O>>();

                foreach (var row in searchResponse.RowList)
                {
                    Operation<I, O> operation = new Operation<I, O>();

                    operation.OperationID = SearchResponse.SafeReadInt32(searchResponse.ReadRow(row), IQ.Entities.VastDB.Const.Columns.COL_GUID_OPERATION_ID, NULL_ID);

                    // Used to tie back to operation specific settings.
                    operation.ConfigSourceKey = context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_KEY).ToString();
                    operation.ConfigSourceID = SearchResponse.SafeReadString(searchResponse.ReadRow(row), operation.ConfigSourceKey);
                    operation.Sequence = SearchResponse.SafeReadInt32(searchResponse.ReadRow(row), IQ.Entities.VastDB.Const.Columns.COL_GUID_SEQUENCE, 0);

                    operationObjectList.Add(operation);
                }

                // Sort based on Sequence
                List<Operation<I, O>> sortedList = operationObjectList.OrderBy(o => o.Sequence).ToList();

                foreach (Operation<I, O> operation in sortedList)
                {
                    LoadOperationsFromID(context, instanceGUID, databaseName, operation, operationList);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void LoadOperationsFromID(IContext context,
                                                    string instanceGUID,
                                                    string databaseName,
                                                    Operation<I, O> operation,
                                                    List<Operation<I, O>> operationList)
        {
            try
            {
                string metabaseName;
                IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);

                // Find all operations for the given table for the given action.
                IQ.Entities.VastDB.Entity entity = repo.Read(context, instanceGUID, metabaseName, string.Empty, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_OPERATION, Columns.COL_GUID_OPERATION_ID, operation.OperationID);

                string operationFullName = Entity.SafeReadString(entity.ColumnIdValuePair, Columns.COL_GUID_FULL_NAME).Trim();
                bool async = Entity.SafeReadBool(entity.ColumnIdValuePair, Columns.COL_GUID_ASYNC, false);

                if (string.IsNullOrWhiteSpace(operationFullName) == false)
                {
                    Operation<I, O> dynamicOperation = new Operation<I, O>();

                    dynamicOperation.OperationInstance = context.Get<IServiceLocator>().LocateByName<IOperation<I, O>>(context, operationFullName, operation.ConfigSourceID);
                    dynamicOperation.Async = async;
                    dynamicOperation.ConfigSourceID = operation.ConfigSourceID;
                    dynamicOperation.OperationID = operation.OperationID;
                    dynamicOperation.ConfigSourceKey = operation.ConfigSourceKey;

                    if (dynamicOperation.OperationInstance != null)
                    {
                        operationList.Add(dynamicOperation);
                    }
                    else
                    {
                        context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Error, "Unable to load: " + operationFullName);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        04/21/2015
        /// Author:         David J. McKee
        /// Purpose:        Loads a custom assembly line by name
        /// </summary>        
        public bool UseCustomAssemblyName(IContext context,
                                                List<Operation<I, O>> operationList)
        {
            bool useCustomAssemblyName = false;

            try
            {
                string customAssemblyLineName = string.Empty;

                if (context.GetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME) != null)
                {
                    customAssemblyLineName = context.GetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME).ToString();
                }

                if (string.IsNullOrWhiteSpace(customAssemblyLineName) == false)
                {
                    string instanceGUID = context.GetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID).ToString();
                    string databaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();

                    // First, check the current account and database.
                    useCustomAssemblyName = LoadCustomAssemblyLine(context, operationList, customAssemblyLineName, instanceGUID, databaseName);

                    // Next, check the Master Repository for the given account.
                    if (useCustomAssemblyName == false)
                    {
                        string domainName = context.GetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME).ToString();
                        string accountCode = context.GetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE).ToString();
                        string accountInstanceGUID = IQ.BUS.Vast.Common.Helpers.ViewModelToDataModel.BuildInstanceGUID(domainName, accountCode);

                        useCustomAssemblyName = LoadCustomAssemblyLine(context, operationList, customAssemblyLineName, accountInstanceGUID, Databases.IQ_CLOUD_DATABASE_GUID);
                    }

                    // Next, check the Master Repository for the given domain.
                    if (useCustomAssemblyName == false)
                    {
                        string domainName = context.GetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME).ToString();
                        string rootDomainInstanceGUID = IQ.BUS.Vast.Common.Helpers.ViewModelToDataModel.BuildInstanceGUID(domainName, string.Empty);

                        useCustomAssemblyName = LoadCustomAssemblyLine(context, operationList, customAssemblyLineName, rootDomainInstanceGUID, Databases.IQ_CLOUD_DATABASE_GUID);
                    }

                    // Next, check the Root
                    if (useCustomAssemblyName == false)
                    {
                        string rootInstanceGUID = IQ.BUS.Vast.Common.Helpers.ViewModelToDataModel.BuildInstanceGUID(IQ.BUS.Vast.Common.Consts.Root.IQ_CLOUD_DOMAIN, string.Empty);
                        useCustomAssemblyName = LoadCustomAssemblyLine(context, operationList, customAssemblyLineName, rootInstanceGUID, Databases.IQ_CLOUD_DATABASE_GUID);
                    }
                }
            }
            catch (System.Exception ex)
            {
                useCustomAssemblyName = false;
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return useCustomAssemblyName;
        }

        private bool LoadCustomAssemblyLine(IContext context, List<Operation<I, O>> operationList, string customAssemblyLineName, string instanceGUID, string databaseName)
        {
            bool useCustomAssemblyName = false;

            string metabaseName;
            IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);

            IQ.Entities.VastDB.Entity entity =
                repo.Read(context, instanceGUID, metabaseName, string.Empty, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_CUSTOM_ASSEMBLY_LINE,
                            Columns.COL_GUID_NAME, customAssemblyLineName);

            if (entity != null)
            {
                IQ.Entities.VastDB.EntitySearch searchCriteria = new IQ.Entities.VastDB.EntitySearch();

                int sysCustomAssemblyLineId = Entity.SafeReadInt32(entity.ColumnIdValuePair, IQ.Entities.VastDB.Const.Columns.COL_GUID_SYS_CUSTOM_ASSEMBLY_LINE_ID, NULL_ID);

                if (sysCustomAssemblyLineId > 0)
                {
                    searchCriteria.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(IQ.Entities.VastDB.Const.Columns.COL_GUID_SYS_CUSTOM_ASSEMBLY_LINE_ID,
                        IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, sysCustomAssemblyLineId));

                    // Search the sysCustomALOperation table
                    IQ.Entities.VastDB.SearchResponse searchResponse = repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_CUSTOM_ASSEBLY_LINE_OPERATION, searchCriteria);

                    // Set the ConfigSource
                    context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE, Tables.TABLE_GUID_SYS_CUSTOM_ASSEBLY_LINE_OPERATION_SETTING);
                    context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_KEY, Columns.COL_GUID_SYS_CUSTOM_ASSEMBLY_LINE_OPERATION_ID);
                    context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_INSTANCE_GUID, instanceGUID);
                    context.SetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_DATABASE_NAME, databaseName);

                    LoadOperationsFromList(context, instanceGUID, databaseName, operationList, searchResponse);

                    if (operationList.Count > 0)
                    {
                        useCustomAssemblyName = true;
                    }
                }
            }

            return useCustomAssemblyName;
        }

        public void ExecuteAssemblyLine(FlowTransport<I> context,
                                        ref O response,
                                        List<Operation<I, O>> operationList,
                                        string transactionId,
                                        string instanceGUID,
                                        string databaseName,
                                        string tableId,
                                        string actionType,
                                        bool tableAssemblyLine,
                                        bool customAssemblyLine,
                                        string customAssemblyLineName,
                                        string columnVisibilityInstanceGUID,
                                        string columnVisibilityDatabaseName,
                                        string columnVisibilityTableName,
                                        string sysTableSchemaInstanceGUID,
                                        string sysTableSchemaDatabaseName,
                                        string sysTableSchemaTableName)
        {
            try
            {
                foreach (Operation<I, O> operation in operationList)
                {
                    if (Convert.ToBoolean(context[IQ.Entities.VastDB.Const.Context.EXIT_ASSEMBLY_LINE]) == false)
                    {
                        DateTime startTime = DateTime.Now;

                        if (operation.Async)
                        {
                            ExecuteOperationAsync(context, response, operation);
                        }
                        else
                        {
                            ExecuteOperation(context, ref response, operation.OperationInstance);
                        }

                        try
                        {
                            IQ.BUS.Vast.Common.Helpers.EventLogHelper.AfterOperation(context, operation.OperationInstance.GetType(), operation.OperationID, transactionId,
                                                                                        instanceGUID, databaseName, tableId, actionType, startTime, DateTime.Now, context.Payload, response,
                                                                                        tableAssemblyLine, customAssemblyLine, customAssemblyLineName, operation.Async,
                                                                                        columnVisibilityInstanceGUID, columnVisibilityDatabaseName, columnVisibilityTableName,
                                                                                        sysTableSchemaInstanceGUID, sysTableSchemaDatabaseName, sysTableSchemaTableName,
                                                                                        operation.ConfigSourceID, operation.ConfigSourceKey);
                        }
                        catch (System.Exception ex)
                        {
                            context.Get<IExceptionHandler>().HandleException(context, ex);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        07/09/2015
        /// Author:         David J. McKee
        /// Purpose:        Asyncronously executes the operation using an external process.      
        /// </summary>        
        private void ExecuteOperationAsync(FlowTransport<I> context, O response, Operation<I, O> operation)
        {
            try
            {
                const string QUEUE_NAME_PREFIX = "QueueNamePrefix";
                const string DEFAULT_QUEUE_NAME_PREFIX = ".\\Private$\\";

                string queueNamePrefix = context.Get<IAppConfigFactory>().GetAppConfig(IQ.Entities.VastDB.Const.Context.CONTEXT_VALUE_APP_CODE).ReadSetting(QUEUE_NAME_PREFIX, DEFAULT_QUEUE_NAME_PREFIX);

                IQueueFactory queueFactory = context.Get<IQueueFactory>();

                string queueNameSuffix = IQ.Entities.VastDB.Const.Context.CONTEXT_VALUE_APP_CODE + context[IQ.BUS.Vast.Common.Consts.Context.CONTEXT_KEY_ACTION_TYPE].ToString();

                string queueName = queueNamePrefix.Trim() + queueNameSuffix.Trim();

                IQueue queue = queueFactory.Find(context, queueName);

                context[IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_ID] = operation.ConfigSourceID;

                queue.Enqueue(context, operation.OperationInstance.GetType().FullName, context.Payload.ToString());
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void ExecuteOperation(FlowTransport<I> context,
                                                ref O response,
                                                IOperation<I, O> operation)
        {
            try
            {
                if ((bool)context[IQ.Entities.VastDB.Const.Context.EXIT_ASSEMBLY_LINE] == false)
                {
                    operation.Execute(context, default(I), ref response);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public void LoadOperations(FlowTransport<I> context, Table table, string actionType,
                                    ref bool useTableAssemblyLine, ref bool useCustomAssemblyLine, ref bool useOperationList,
                                    ref List<Operation<I, O>> operationList)
        {
            useOperationList = UseOperationList(context, operationList);

            if (useOperationList == false)
            {
                useCustomAssemblyLine = UseCustomAssemblyName(context, operationList);

                if (useCustomAssemblyLine == false)
                {
                    useTableAssemblyLine = UseTableAssemblyLine(context, table, actionType, operationList);

                    if (useTableAssemblyLine == false)
                    {
                        AppendStandardOperations(context, operationList, actionType);
                    }
                    else
                    {
                        switch (table.AssemblyLineMode)
                        {
                            case AssemblyLineMode.Append:
                                // Start with a new list
                                List<Operation<I, O>> appendListOperationList =
                                    new List<Operation<I, O>>();

                                // Add the standard items.
                                AppendStandardOperations(context, appendListOperationList, actionType);

                                // Append the operationList
                                appendListOperationList.AddRange(operationList);

                                // Replace the operation list with the new list
                                operationList = appendListOperationList;
                                break;

                            case AssemblyLineMode.Prepend:
                                // Add the started items
                                AppendStandardOperations(context, operationList, actionType);
                                break;

                            default:
                                // Do nothing. Assume operations are already loaded.
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Created:        10/13/2016
        /// Author:         David J. McKee
        /// Purpose:        Loads operations from an operation list.
        /// </summary>        
        private bool UseOperationList(IContext context, List<Operation<I, O>> operationList)
        {
            bool useOperationList = false;

            try
            {
                if (context.GetByName(IQ.Entities.VastDB.Const.Context.OPERATION_NAME_LIST) != null)
                {
                    string operationNameList = context.GetByNameAs<string>(IQ.Entities.VastDB.Const.Context.OPERATION_NAME_LIST);

                    if (string.IsNullOrWhiteSpace(operationNameList) == false)
                    {
                        string[] operations = operationNameList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        IAppConfig appConfig = context.Get<IAppConfigFactory>().GetAppConfig(IQ.Entities.VastDB.Const.Context.CONTEXT_VALUE_APP_CODE);

                        foreach (string operationName in operations)
                        {
                            // Attempt to map short name to long name.                            
                            const string VIRTUAL_CLASS_NAME_SUFFIX = "OperationFullName";
                            string operationFullName = appConfig.ReadSetting(operationName + VIRTUAL_CLASS_NAME_SUFFIX, operationName);

                            Operation<I, O> dynamicOperation = new Operation<I, O>();

                            dynamicOperation.OperationInstance = context.Get<IServiceLocator>().LocateByName<IOperation<I, O>>(context, operationFullName);

                            if (dynamicOperation.OperationInstance != null)
                            {
                                operationList.Add(dynamicOperation);
                                useOperationList = true;
                            }
                            else
                            {
                                context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Error, "Unable to load: " + operationFullName);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {

            }

            return useOperationList;
        }
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.Vast.Common.Helpers
{
    public static class EventLogHelper
    {
        private const string DATE_TIME_FORMAT = "MM/dd/yyyy hh:mm:ss.fff tt";

        /// <summary>
        /// Created:        09/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Writes an entry to the event log
        /// </summary>        
        private static void WriteToLogDB(IContext context,
                                            IQ.BUS.Vast.Common.Consts.EventType eventType,
                                            string instanceGuid,
                                            string transactionId,
                                            string fullName,
                                            DateTime startTime,
                                            DateTime? endTime,
                                            object requestPayload,
                                            object response,
                                            string moduleId,
                                            string formId,
                                            string actionType,
                                            bool tableAssemblyLine,
                                            bool customAssemblyLine,
                                            string customAssemblyLineName,
                                            string message,
                                            bool executedAsync,
                                            string columnVisibilityInstanceGUID,
                                            string columnVisibilityDatabaseName,
                                            string columnVisibilityTableName,
                                            string sysTableSchemaInstanceGUID,
                                            string sysTableSchemaDatabaseName,
                                            string sysTableSchemaTableName,
                                            string configSourceID,
                                            string configSourceKey)
        {
            try
            {
                string logSetting = context.GetByName(IQ.Entities.VastDB.Const.Context.LOG_SETTING).ToString();

                bool logCritical;
                bool logError;
                bool logInformation;
                bool logStart;
                bool logStop;
                bool logVerbose;
                bool logWarning;
                CreateLogMatrix(logSetting, out logCritical, out logError, out logInformation, out logStart, out logStop, out logVerbose, out logWarning);

                if (IsLoggingEnabled(eventType, logCritical, logError, logInformation, logStart, logStop, logVerbose, logWarning))
                {
                    double? totalMilliseconds = null;

                    if (endTime != null)
                    {
                        TimeSpan timeSpan = ((DateTime)endTime).Subtract(startTime);
                        totalMilliseconds = timeSpan.TotalMilliseconds;
                    }

                    // Write the log to the database.
                    IQ.Entities.VastDB.Entity entity = new Entities.VastDB.Entity();

                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_TRANSACTION_ID, transactionId);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_ACTION_TYPE, actionType);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_FORM_ID, formId);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_FULL_NAME, fullName);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_MODULE_ID, moduleId);

                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_TABLE_ASSEMBLY_LINE, tableAssemblyLine.ToString());
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_CUSTOM_ASSEMBLY_LINE, customAssemblyLine.ToString());
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_CUSTOM_ASSEMBLY_LINE_NAME, customAssemblyLineName);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_ASYNC, executedAsync.ToString());

                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_MESSAGE, message);

                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_VISIBILITY_INSTANCE_GUID, columnVisibilityInstanceGUID);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_VISIBILITY_DATABASE_NAME, columnVisibilityDatabaseName);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_VISIBILITY_TABLE_NAME, columnVisibilityTableName);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_INSTANCE_GUID, sysTableSchemaInstanceGUID);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_DATABASE_NAME, sysTableSchemaDatabaseName);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_TABLE_NAME, sysTableSchemaTableName);

                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_INSTANCE_GUID, instanceGuid);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_START_TIME, startTime.ToString(DATE_TIME_FORMAT));

                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_CONFIG_SOURCE_KEY, configSourceKey);
                    entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_CONFIG_SOURCE_ID, configSourceID);

                    if (endTime != null)
                    {
                        entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_END_TIME, ((DateTime)endTime).ToString(DATE_TIME_FORMAT));
                    }

                    if (totalMilliseconds != null)
                    {
                        entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_TOTAL_MILLISECONDS, totalMilliseconds.ToString());
                    }

                    if (logVerbose)
                    {
                        string requestJson = context.Get<IResponseHelper>().ObjectToString(requestPayload, SAIL.Framework.Host.Enums.ResponseFormat.JSON);
                        string responseJson = context.Get<IResponseHelper>().ObjectToString(response, SAIL.Framework.Host.Enums.ResponseFormat.JSON);
                        string contextContents = GetContextJson(context);

                        entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_GUID_CONTEXT_CONTENTS, contextContents);
                        entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_REQUEST, requestJson);
                        entity.SetColumnValue(IQ.Entities.VastDB.Const.Columns.COL_RESPONSE, responseJson);
                    }

                    entity.PrimaryKeyColumnID = IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_LOG + "ID";

                    const string LOGGING_DATABASE_SUFFIX = "Logs";

                    string logDatabaseName = moduleId + LOGGING_DATABASE_SUFFIX;

                    IStandardTable repo = (IStandardTable)context.GetByName(Context.INTERFACE_STANDARD_TABLE_OR_BLOB_METADATA);

                    IQ.BUS.Vast.Common.Helpers.SchemaHelper.EnsureTableColumns(context, repo, instanceGuid, logDatabaseName, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_LOG,
                                                                            entity.PrimaryKeyColumnID,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_TOTAL_MILLISECONDS,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_TRANSACTION_ID,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_FULL_NAME,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_ACTION_TYPE,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_TABLE_ASSEMBLY_LINE,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_CUSTOM_ASSEMBLY_LINE,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_CUSTOM_ASSEMBLY_LINE_NAME,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_FORM_ID,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_MODULE_ID,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_MESSAGE,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_ASYNC,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_CONFIG_SOURCE_KEY,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_CONFIG_SOURCE_ID,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_VISIBILITY_DATABASE_NAME,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_VISIBILITY_TABLE_NAME, IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_INSTANCE_GUID,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_DATABASE_NAME, IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_TABLE_NAME,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_INSTANCE_GUID, IQ.Entities.VastDB.Const.Columns.COL_GUID_START_TIME,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_END_TIME,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_CONTEXT_CONTENTS,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_REQUEST,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_RESPONSE,
                                                                            IQ.Entities.VastDB.Const.Columns.COL_GUID_COLUMN_VISIBILITY_INSTANCE_GUID);

                    repo.Create(context, instanceGuid, logDatabaseName, string.Empty, string.Empty, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_LOG, entity);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static string GetContextJson(IContext context)
        {
            string json = "{}";

            try
            {
                IPayload payload = context.Get<IPayloadFactory>().FromJson("{}");

                foreach (string key in context.Collection.Keys)
                {
                    object item = context.Collection[key];

                    if (item != null)
                    {
                        try
                        {
                            // In the future we may serialize non-string objects.
                            if (item is System.String)
                            {
                                string innerText = item.ToString();

                                payload.AppendChildElement(key, innerText);
                            }
                        }
                        catch { }
                    }
                }

                json = payload.Json;
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return json;
        }

        private static bool IsLoggingEnabled(IQ.BUS.Vast.Common.Consts.EventType eventType, bool logCritical, bool logError, bool logInformation, bool logStart, bool logStop, bool logVerbose, bool logWarning)
        {
            bool logEvent = false;

            switch (eventType)
            {
                case Consts.EventType.Critical:
                    logEvent = logCritical;
                    break;

                case Consts.EventType.Error:
                    logEvent = logError;
                    break;

                case Consts.EventType.Information:
                    logEvent = logInformation;
                    break;

                case Consts.EventType.Start:
                    logEvent = logStart;
                    break;

                case Consts.EventType.Stop:
                    logEvent = logStop;
                    break;

                case Consts.EventType.Verbose:
                    logEvent = logVerbose;
                    break;

                case Consts.EventType.Warning:
                    logEvent = logWarning;
                    break;
            }

            return logEvent;
        }

        private static void CreateLogMatrix(string logSetting, out bool logCritical, out bool logError, out bool logInformation, out bool logStart, out bool logStop, out bool logVerbose, out bool logWarning)
        {
            logCritical = false;
            logError = false;
            logInformation = false;
            logStart = false;
            logStop = false;
            logVerbose = false;
            logWarning = false;

            switch (logSetting)
            {
                case IQ.Entities.VastDB.Const.LogSetting.ACTIVITY:  // start and stop events only
                    logCritical = true;
                    logError = true;
                    logInformation = false;
                    logStart = true;
                    logStop = true;
                    logVerbose = false;
                    logWarning = false;
                    break;

                case IQ.Entities.VastDB.Const.LogSetting.ALL:       //  “Verbose” plus “ActivityTracing”
                    logCritical = true;
                    logError = true;
                    logInformation = true;
                    logStart = true;
                    logStop = true;
                    logVerbose = true;
                    logWarning = true;
                    break;

                case IQ.Entities.VastDB.Const.LogSetting.ERROR:     // errors only
                    logCritical = true;
                    logError = true;
                    logInformation = false;
                    logStart = false;
                    logStop = false;
                    logVerbose = false;
                    logWarning = false;
                    break;

                case IQ.Entities.VastDB.Const.LogSetting.INFO:      // errors, warnings, informational messages
                    logCritical = true;
                    logError = true;
                    logInformation = true;
                    logStart = false;
                    logStop = false;
                    logVerbose = false;
                    logWarning = true;
                    break;

                case IQ.Entities.VastDB.Const.LogSetting.OFF:       // Do nothing
                    logCritical = false;
                    logError = false;
                    logInformation = false;
                    logStart = false;
                    logStop = false;
                    logVerbose = false;
                    logWarning = false;
                    break;

                case IQ.Entities.VastDB.Const.LogSetting.VERBOSE:   // “Information” plus additional debugging trace information including API requests and responses in JSON format.
                    logCritical = true;
                    logError = true;
                    logInformation = true;
                    logStart = false;
                    logStop = false;
                    logVerbose = true;
                    logWarning = true;
                    break;

                case IQ.Entities.VastDB.Const.LogSetting.WARNING:   // errors and warnings
                    logCritical = true;
                    logError = true;
                    logInformation = false;
                    logStart = false;
                    logStop = false;
                    logVerbose = false;
                    logWarning = true;
                    break;
            }
        }

        public static void EnterAssemblyLine(IContext context,
                                                System.Type assemblyLineType,
                                                string transactionId,
                                                string instanceGuid,
                                                string moduleId,
                                                string formId,
                                                string actionType,
                                                DateTime startTime,
                                                object requestPayload,
                                                object response,
                                                string customAssemblyLineName)
        {
            bool customAssemblyLine = !string.IsNullOrWhiteSpace(customAssemblyLineName);

            WriteToLogDB(context, Consts.EventType.Start, instanceGuid, transactionId, assemblyLineType.FullName, startTime, null,
                            requestPayload, response, moduleId, formId, actionType, false, customAssemblyLine, customAssemblyLineName, "Enter AssemblyLine",
                            false, string.Empty, string.Empty, string.Empty,
                            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
        }

        public static void ExitAssemblyLine(IContext context,
                                                System.Type assemblyLineType,
                                                string transactionId,
                                                string instanceGuid,
                                                string moduleId,
                                                string formId,
                                                string actionType,
                                                DateTime startTime,
                                                DateTime endTime,
                                                object requestPayload,
                                                object response,
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
            WriteToLogDB(context, Consts.EventType.Start, instanceGuid, transactionId, assemblyLineType.FullName, startTime, endTime,
                            requestPayload, response, moduleId, formId, actionType, tableAssemblyLine, customAssemblyLine, customAssemblyLineName, "Exit AssemblyLine",
                            false, columnVisibilityTableName, columnVisibilityDatabaseName, columnVisibilityTableName,
                            sysTableSchemaInstanceGUID, sysTableSchemaDatabaseName, sysTableSchemaTableName, string.Empty, string.Empty);
        }

        public static void AfterOperation(IContext context,
                                                System.Type operationType,
                                                int operationId,
                                                string transactionId,
                                                string instanceGuid,
                                                string moduleId,
                                                string formId,
                                                string actionType,
                                                DateTime startTime,
                                                DateTime endTime,
                                                object requestPayload,
                                                object response,
                                                bool tableAssemblyLine,
                                                bool customAssemblyLine,
                                                string customAssemblyLineName,
                                                bool executeAsync,
                                                string columnVisibilityInstanceGUID,
                                                string columnVisibilityDatabaseName,
                                                string columnVisibilityTableName,
                                                string sysTableSchemaInstanceGUID,
                                                string sysTableSchemaDatabaseName,
                                                string sysTableSchemaTableName,
                                                string configSourceID,
                                                string configSourceKey)
        {

            StringBuilder message = new StringBuilder();

            message.Append("After Operation");

            if (tableAssemblyLine || customAssemblyLine)
            {
                message.Append(" (");
                message.Append(operationId);
                message.Append(")");
            }

            WriteToLogDB(context, Consts.EventType.Information, instanceGuid, transactionId, operationType.FullName, startTime, endTime,
                            requestPayload, response, moduleId, formId, actionType, tableAssemblyLine, customAssemblyLine, customAssemblyLineName, message.ToString(),
                            executeAsync,
                            columnVisibilityInstanceGUID, columnVisibilityDatabaseName, columnVisibilityTableName,
                            sysTableSchemaInstanceGUID, sysTableSchemaDatabaseName, sysTableSchemaTableName,
                            configSourceID, configSourceKey);

            // Trace to debugger
            string instanceGUID = context.GetByName(Context.INSTANCE_GUID).ToString();
            string databaseId = context.GetByName(Context.DATABASE_ID).ToString();

            string traceMessage = " Operation Type: " + operationType.ToString() + ", " +
                                Context.INSTANCE_GUID + ": " + instanceGUID + ", " +
                                Context.DATABASE_ID + ": " + databaseId + ", " +
                                Columns.COL_GUID_FORM_ID + ": " + formId + ": " +
                                Columns.COL_GUID_ACTION_TYPE + ": " + actionType;
            //Columns.COL_GUID_TRANSACTION_ID + ": " + transactionId;

            context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Info, traceMessage);
        }
    }
}

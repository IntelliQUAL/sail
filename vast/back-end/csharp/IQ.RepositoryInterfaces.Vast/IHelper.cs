using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

namespace IQ.RepositoryInterfaces.Vast
{
    /// <summary>
    /// Created:        06/15/2016
    /// Author:         David J. McKee
    /// Purpose:        Provides common "helper" methods to any operation.
    /// </summary>
    public interface IHelper<TableDataModelType, ColumnDataModelType, EntityDataModelType, SearchRequestDataModelType, SearchResponseDataModelType>
    {
        // Appends meta data to the given table from sysTableSchema
        void AppendTableMetaData(IContext context,
                                    string databaseName,
                                    TableDataModelType output,
                                    string action);

        TableDataModelType GetTableMetaData(IContext context, string databaseName, string tableID);

        // Reads the columns and related schemea
        List<ColumnDataModelType> SearchColumnList(IContext context,
                                                                            string columnVisibilityInstanceGUID,
                                                                            string columnVisibilityDatabaseName,
                                                                            string columnVisibilityTableName,
                                                                            string tableId,
                                                                            string tablePrimaryKeyColumnId,
                                                                            string action);

        // Important: This also returns the Metabase Repository.
        IStandardTable ReadMetabaseName(IContext context, string databaseName, out string metabaseName);
        IStandardTable InitStandardTableRepo();

        EntityDataModelType ExecuteAssemblyLineCRUD(FlowTransport<EntityDataModelType> context);
        EntityDataModelType ExecuteAssemblyLineCRUD(FlowTransport<EntityDataModelType> context, string instanceGUID, string databaseName, string tableName,
                                                        string actionName, string customAssemblyLineName, string accountCode);


        SearchResponseDataModelType ExecuteAssemblyLineSearch(FlowTransport<SearchRequestDataModelType> context);

        SearchResponseDataModelType ExecuteAssemblyLineSearch(FlowTransport<SearchRequestDataModelType> context, string instanceGUID, string databaseName, string tableName,
                                                                string actionName, string customAssemblyLineName, string accountCode);

        // Encode: Builds an enum list from a given table.
        string EnumFromTable(IContext context, IStandardTable repo, string instanceGuid, string databaseName, string tableName, string valueColumn);

        // Decode: Parses a string in the encoded format (used by group names and enums)
        Dictionary<string, string> ParseEncodedList(IContext context, string encodedText);

        // Removes invalid chars relative the a file or path.
        string RemoveInvalidChars(string text);

        string BuildInstanceGUID(string domainName, string accountCode);

        // Initializes an assembly line with the ability to interact with a database and settings / configuration.
        void InitAssemblyLineContext(IContext context, string actionType, out string customAssemblyLineName);
        void InitAssemblyLineContext(IContext context,
                                                    string actionType,
                                                    out string instanceGuid,
                                                    out string moduleId,
                                                    out string customAssemblyLineName,
                                                    out string transactionId);

        // Initializes an assembly line with the ability to interact with a database and settings / configuration.
        void InitContext(IContext context, string domainName);
        void InitRequestAndResponse(string tableId, string actionType, IQ.Entities.VastDB.Entity request, IQ.Entities.VastDB.Entity response);
    }
}

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.Entities.VastDB;
using IQ.BUS.Vast.Common;
using IQ.Entities.VastMetaDB;
using IQ.Entities.VastDB.Const;
using IQ.BUS.Vast.Common.Helpers;
using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.VAST.Helpers
{
    public class Helper : IHelper<Table, Column, Entity, EntitySearch, SearchResponse>
    {
        void IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.AppendTableMetaData(IContext context, string databaseName, Table output, string action)
        {
            MetaDataHelper.AppendTableMetaData(context, databaseName, output, action);
        }

        string IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.BuildInstanceGUID(string domainName, string accountCode)
        {
            string instanceGUID = ViewModelToDataModel.BuildInstanceGUID(domainName, accountCode);

            return instanceGUID;
        }

        string IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.EnumFromTable(IContext context, IStandardTable repo, string instanceGuid, string databaseName, string tableName, string valueColumn)
        {
            throw new NotImplementedException();
        }

        Entity IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.ExecuteAssemblyLineCRUD(FlowTransport<Entity> context, string instanceGUID, string databaseName, string tableName, string actionName, string customAssemblyLineName, string accountCode)
        {
            Entity entityResponse = new Entity();

            try
            {
                context[Context.INSTANCE_GUID] = instanceGUID;
                context[Context.DATABASE_ID] = databaseName;
                context[Context.TABLE_ID] = tableName;
                context[Context.CONTEXT_KEY_ACTION_TYPE] = actionName;
                context[Context.CUSTOM_ASSEMBLY_LINE_NAME] = customAssemblyLineName;
                context[Context.ACCOUNT_CODE] = accountCode;

                switch (actionName)
                {
                    case Actions.ACTION_NEW:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> newAssemblyLine = new IQ.BUS.VAST.AssemblyLines.New();
                        entityResponse = newAssemblyLine.Execute(context);
                        break;

                    case Actions.ACTION_CREATE:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> create = new IQ.BUS.VAST.AssemblyLines.Create();
                        entityResponse = create.Execute(context);
                        break;

                    case Actions.ACTION_READ:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> read = new IQ.BUS.VAST.AssemblyLines.Read();
                        entityResponse = read.Execute(context);
                        break;

                    case Actions.ACTION_UPDATE:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> update = new IQ.BUS.VAST.AssemblyLines.Update();
                        entityResponse = update.Execute(context);
                        break;

                    case Actions.ACTION_DELETE:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> delete = new IQ.BUS.VAST.AssemblyLines.Delete();
                        entityResponse = delete.Execute(context);
                        break;

                    case Actions.ACTION_SEARCH:
                        break;

                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return entityResponse;
        }

        Entity IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.ExecuteAssemblyLineCRUD(FlowTransport<Entity> context)
        {
            Entity entityResponse = new Entity();

            try
            {
                string actionName = context[Context.CONTEXT_KEY_ACTION_TYPE].ToString();

                switch (actionName)
                {
                    case Actions.ACTION_NEW:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> newAssemblyLine = new IQ.BUS.VAST.AssemblyLines.New();
                        entityResponse = newAssemblyLine.Execute(context);
                        break;

                    case Actions.ACTION_CREATE:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> create = new IQ.BUS.VAST.AssemblyLines.Create();
                        entityResponse = create.Execute(context);
                        break;

                    case Actions.ACTION_READ:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> read = new IQ.BUS.VAST.AssemblyLines.Read();
                        entityResponse = read.Execute(context);
                        break;

                    case Actions.ACTION_UPDATE:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> update = new IQ.BUS.VAST.AssemblyLines.Update();
                        entityResponse = update.Execute(context);
                        break;

                    case Actions.ACTION_DELETE:
                        IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> delete = new IQ.BUS.VAST.AssemblyLines.Delete();
                        entityResponse = delete.Execute(context);
                        break;

                    case Actions.ACTION_SEARCH:
                        break;

                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return entityResponse;
        }

        SearchResponse IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.ExecuteAssemblyLineSearch(FlowTransport<EntitySearch> context,
                                                                                                                string instanceGUID,
                                                                                                                string databaseName,
                                                                                                                string tableName,
                                                                                                                string actionName,
                                                                                                                string customAssemblyLineName,
                                                                                                                string accountCode)
        {
            SearchResponse searchResponse = new SearchResponse();

            try
            {
                context[Context.INSTANCE_GUID] = instanceGUID;
                context[Context.DATABASE_ID] = databaseName;
                context[Context.TABLE_ID] = tableName;
                context[Context.CONTEXT_KEY_ACTION_TYPE] = actionName;
                context[Context.CUSTOM_ASSEMBLY_LINE_NAME] = customAssemblyLineName;
                context[Context.ACCOUNT_CODE] = accountCode;

                switch (actionName)
                {
                    case Actions.ACTION_NEW:
                        break;

                    case Actions.ACTION_CREATE:
                        break;

                    case Actions.ACTION_READ:
                        break;

                    case Actions.ACTION_UPDATE:
                        break;

                    case Actions.ACTION_DELETE:
                        break;

                    case Actions.ACTION_SEARCH:
                        IAssemblyLine<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> searchAssemblyLine = new
                            IQ.BUS.VAST.AssemblyLines.Search();
                        searchResponse = searchAssemblyLine.Execute(context);
                        break;

                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return searchResponse;
        }

        SearchResponse IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.ExecuteAssemblyLineSearch(FlowTransport<EntitySearch> context)
        {
            throw new NotImplementedException();
        }

        Table IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.GetTableMetaData(IContext context, string databaseName, string tableID)
        {
            return IQ.BUS.Vast.Common.AppendRequestTableSchema.GetTableMetaData(context, databaseName, tableID);
        }

        void IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.InitAssemblyLineContext(IContext context, string actionType, out string customAssemblyLineName)
        {
            string instanceGuid;
            string moduleId;
            string transactionId;
            string operationNameList;

            CommonConcerns.InitAssemblyLineContext(context, actionType, out instanceGuid, out moduleId, out customAssemblyLineName, out transactionId, out operationNameList);
        }

        void IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.InitContext(IContext context, string domainName)
        {
            CommonConcerns.InitContext(context, domainName);
        }

        Dictionary<string, string> IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.ParseEncodedList(IContext context, string encodedText)
        {
            return IQ.BUS.Vast.Common.MetaDataHelper.ParseEncodedList(context, encodedText);
        }

        IStandardTable IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.ReadMetabaseName(IContext context, string databaseName, out string metabaseName)
        {
            return IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);
        }

        string IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.RemoveInvalidChars(string text)
        {
            return IQ.BUS.Vast.Common.MetaDataHelper.RemoveInvalidChars(text);
        }

        List<Column> IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.SearchColumnList(IContext context, string columnVisibilityInstanceGUID, string columnVisibilityDatabaseName, string columnVisibilityTableName, string tableId, string tablePrimaryKeyColumnId, string action)
        {
            throw new NotImplementedException();
        }

        IStandardTable IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.InitStandardTableRepo()
        {
            throw new NotImplementedException();
        }

        void IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.InitAssemblyLineContext(IContext context, string actionType, out string instanceGuid, out string moduleId, out string customAssemblyLineName, out string transactionId)
        {
            throw new NotImplementedException();
        }

        void IHelper<Table, Column, Entity, EntitySearch, SearchResponse>.InitRequestAndResponse(string tableId, string actionType, Entity request, Entity response)
        {
            throw new NotImplementedException();
        }
    }
}

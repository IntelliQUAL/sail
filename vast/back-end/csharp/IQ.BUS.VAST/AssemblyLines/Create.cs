using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.BUS.Vast.Common;
using IQ.Entities.VastDB;
using IQ.BUS.Vast.Helpers;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;
using IQ.Entities.VastMetaDB.Enums;
using IQ.Entities.VastMetaDB.SqlEsque;

namespace IQ.BUS.VAST.AssemblyLines
{
    /// <summary>
    /// Created:        03/03/2015
    /// Author:         David J. McKee
    /// Purpose:        Allows for inserting a new row into the given table.
    /// </summary>
    public class Create : IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>
    {

        IQ.Entities.VastDB.Entity IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>.Execute(FlowTransport<IQ.Entities.VastDB.Entity> context)
        {
            IQ.Entities.VastDB.Entity response = new IQ.Entities.VastDB.Entity();

            DateTime startTime = DateTime.Now;

            string instanceGuid = string.Empty;
            string moduleId = string.Empty;
            bool useTableAssemblyLine = false;
            bool useCustomAssemblyLine = false;
            string customAssemblyLineName = string.Empty;
            string transactionId = string.Empty;
            string operationNameList = string.Empty;
            bool useOperationList = false;

            try
            {
                context[typeof(IQ.BUS.Vast.Helpers.HelperFactory).FullName] = new IQ.BUS.Vast.Helpers.HelperFactory();

                CommonConcerns.InitAssemblyLineContext(context, Actions.ACTION_CREATE, out instanceGuid, out moduleId, out customAssemblyLineName, out transactionId, out operationNameList);

                IQ.BUS.Vast.Common.Helpers.EventLogHelper.EnterAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, context.Payload.TableID, Actions.ACTION_CREATE, startTime,
                                                                                context.Payload, response, customAssemblyLineName);

                if (context.Payload.Table == null)
                {
                    context.Payload.Table = new IQ.Entities.VastMetaDB.Table();
                    context.Payload.Table.ID = context.Payload.TableID;
                }

                CommonConcerns.InitRequestAndResponse(context.Payload.TableID, Actions.ACTION_CREATE, context.Payload, response);

                // Append all root schema values to the Request table
                string databaseName = context[IQ.Entities.VastDB.Const.Context.DATABASE_ID].ToString();
                MetaDataHelper.AppendTableMetaData(context, databaseName, context.Payload.Table, IQ.Entities.VastDB.Const.Actions.ACTION_CREATE);

                // Build the default operation list is code. This may be overridden via configuration.
                List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList =
                    new List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>>();

                AssemblyLineHelper<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> assemblyLineHelper = new AssemblyLineHelper<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>();

                assemblyLineHelper.LoadOperations(context, response.Table, IQ.Entities.VastDB.Const.Actions.ACTION_CREATE, ref useTableAssemblyLine, ref useCustomAssemblyLine, ref useOperationList, ref operationList);

                if (useOperationList)
                {
                    RequestDataOperationConfigFactory requestDataOperationConfigFactory = new RequestDataOperationConfigFactory();
                    requestDataOperationConfigFactory.LoadFromEntity(context.Payload);
                    context["IOperationConfigFactory"] = requestDataOperationConfigFactory;
                }

                // Execute the AssebmlyLine                
                assemblyLineHelper.ExecuteAssemblyLine(context, ref response, operationList, transactionId, instanceGuid, databaseName, context.Payload.Table.ID,
                                                        IQ.Entities.VastDB.Const.Actions.ACTION_CREATE, useTableAssemblyLine, useCustomAssemblyLine,
                                                        customAssemblyLineName, context.Payload.Table.ColumnVisibilityInstanceGUID, context.Payload.Table.ColumnVisibilityDatabaseName,
                                                        context.Payload.Table.ColumnVisibilityTableName, context.Payload.Table.TableSchemaInstanceGUID,
                                                        context.Payload.Table.TableSchemaDatabaseName, context.Payload.Table.TableSchemaTableName);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                IQ.BUS.Vast.Common.Helpers.EventLogHelper.ExitAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, context.Payload.Table.ID, IQ.Entities.VastDB.Const.Actions.ACTION_CREATE,
                                                                            startTime, DateTime.Now, context.Payload, response, useTableAssemblyLine, useCustomAssemblyLine, customAssemblyLineName,
                                                                            context.Payload.Table.ColumnVisibilityInstanceGUID, context.Payload.Table.ColumnVisibilityDatabaseName, context.Payload.Table.ColumnVisibilityTableName,
                                                                            context.Payload.Table.TableSchemaInstanceGUID, context.Payload.Table.TableSchemaDatabaseName, context.Payload.Table.TableSchemaTableName);
            }

            return response;
        }

        private static void AppendOperationsByIdList(FlowTransport<IQ.Entities.VastDB.Entity> context,
                                                        List<IOperation<IQ.Entities.VastDB.Entity,
                                                        IQ.Entities.VastDB.Entity>> operationList,
                                                        string operationLineIdList)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(operationLineIdList) == false)
                {
                    // Find all operations for the given table for the given action.
                    EntitySearch entitySearch = new IQ.Entities.VastDB.EntitySearch();

                    Predicate predicate = new Predicate(Columns.COL_GUID_OPERATION_ID, OperatorType.In, operationLineIdList);

                    entitySearch.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(predicate);

                    string instanceGUID = context[IQ.Entities.VastDB.Const.Context.INSTANCE_GUID].ToString();
                    string databaseName = context[IQ.Entities.VastDB.Const.Context.DATABASE_ID].ToString();

                    string metabaseName;
                    IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, databaseName, out metabaseName);

                    SearchResponse searchResponse = repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_OPERATION, entitySearch);

                    if (searchResponse.RowList != null)
                    {
                        foreach (string[] row in searchResponse.RowList)
                        {
                            Dictionary<string, string> rowData = searchResponse.ReadRow(row);

                            string operationFullName = Entity.SafeReadString(rowData, Columns.COL_GUID_FULL_NAME);

                            if (string.IsNullOrWhiteSpace(operationFullName) == false)
                            {
                                IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> dynamicOperation =
                                    context.Get<IServiceLocator>().LocateByName<IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>>(context, operationFullName);

                                if (dynamicOperation != null)
                                {
                                    operationList.Add(dynamicOperation);
                                }
                                else
                                {
                                    // TODO: Report error to end user.
                                }
                            }
                        }
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

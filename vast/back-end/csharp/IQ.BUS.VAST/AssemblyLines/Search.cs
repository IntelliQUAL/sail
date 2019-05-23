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
    /// Purpose:        Allows for searching across one or more tables.
    /// </summary>
    public class Search : IAssemblyLine<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>
    {
        IQ.Entities.VastDB.SearchResponse IAssemblyLine<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>.Execute(FlowTransport<IQ.Entities.VastDB.EntitySearch> context)
        {
            IQ.Entities.VastDB.SearchResponse response = new IQ.Entities.VastDB.SearchResponse();

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

                CommonConcerns.InitAssemblyLineContext(context, Actions.ACTION_SEARCH, out instanceGuid, out moduleId, out customAssemblyLineName, out transactionId, out operationNameList);

                IQ.BUS.Vast.Common.Helpers.EventLogHelper.EnterAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, context.Payload.TableID, Actions.ACTION_SEARCH, startTime,
                                                                                context.Payload, response, customAssemblyLineName);

                response.Table = new IQ.Entities.VastMetaDB.Table();
                response.Table.ID = context.Payload.TableID;

                // We always read the sysTableSchema row because it informs everything else.
                // Append all root schema values
                string databaseName = context[IQ.Entities.VastDB.Const.Context.DATABASE_ID].ToString();
                MetaDataHelper.AppendTableMetaData(context, databaseName, response.Table, IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH);

                // Build the default operation list is code. This may be overridden via configuration.
                List<Operation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>> operationList =
                    new List<Operation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>>();

                AssemblyLineHelper<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> assemblyLineHelper = new AssemblyLineHelper<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>();

                assemblyLineHelper.LoadOperations(context, response.Table, IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH, ref useTableAssemblyLine, ref useCustomAssemblyLine, ref useOperationList, ref operationList);

                if (useOperationList)
                {
                    RequestDataOperationConfigFactory requestDataOperationConfigFactory = new RequestDataOperationConfigFactory();

                    IConnectionHost connectionHost = context.Get<IConnectionHost>();

                    requestDataOperationConfigFactory.LoadFromConnectionHost(connectionHost); // Not sure what to do here, QueryString ????
                    context["IOperationConfigFactory"] = requestDataOperationConfigFactory;
                }

                assemblyLineHelper.ExecuteAssemblyLine(context, ref response, operationList, transactionId, instanceGuid, databaseName, response.Table.ID, IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH,
                                                        useTableAssemblyLine, useCustomAssemblyLine, customAssemblyLineName,
                                                        response.Table.ColumnVisibilityInstanceGUID, response.Table.ColumnVisibilityDatabaseName, response.Table.ColumnVisibilityTableName,
                                                        response.Table.TableSchemaInstanceGUID, response.Table.TableSchemaDatabaseName, response.Table.TableSchemaTableName);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                try
                {
                    IQ.BUS.Vast.Common.Helpers.EventLogHelper.ExitAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, response.Table.ID, IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH,
                                                                                startTime, DateTime.Now, context.Payload, response, useTableAssemblyLine, useCustomAssemblyLine, customAssemblyLineName,
                                                                                response.Table.ColumnVisibilityInstanceGUID, response.Table.ColumnVisibilityDatabaseName, response.Table.ColumnVisibilityTableName,
                                                                                response.Table.TableSchemaInstanceGUID, response.Table.TableSchemaDatabaseName, response.Table.TableSchemaTableName);
                }
                catch (System.Exception ex2)
                {
                    context.Get<IExceptionHandler>().HandleException(context, ex2);
                }
            }

            return response;
        }
    }
}

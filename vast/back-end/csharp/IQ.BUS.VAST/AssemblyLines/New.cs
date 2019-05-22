using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.Entities.VastDB;
using IQ.BUS.Vast.Common;
using IQ.BUS.Vast.Helpers;
using IQ.Entities.VastDB.Const;

namespace IQ.BUS.VAST.AssemblyLines
{
    /// <summary>
    /// Created:        03/03/2015
    /// Author:         David J. McKee
    /// Purpose:        Allows for retrieving all data required to input a new record via the user interface
    /// </summary>
    internal class New : IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>
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

                CommonConcerns.InitAssemblyLineContext(context, Actions.ACTION_NEW, out instanceGuid, out moduleId, out customAssemblyLineName, out transactionId, out operationNameList);

                IQ.BUS.Vast.Common.Helpers.EventLogHelper.EnterAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, context.Payload.TableID, Actions.ACTION_NEW, startTime,
                                                                                context.Payload, response, customAssemblyLineName);

                response.Table = new IQ.Entities.VastMetaDB.Table();
                response.Table.ID = context.Payload.TableID;

                CommonConcerns.InitRequestAndResponse(context.Payload.TableID, Actions.ACTION_NEW, context.Payload, response);

                // Append all root schema values to the Request table
                string databaseName = context[IQ.Entities.VastDB.Const.Context.DATABASE_ID].ToString();
                MetaDataHelper.AppendTableMetaData(context, databaseName, response.Table, IQ.Entities.VastDB.Const.Actions.ACTION_NEW);

                // Build the default operation list is code. This may be overridden via configuration.
                List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList = new List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>>();

                AssemblyLineHelper<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> assemblyLineHelper = new AssemblyLineHelper<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>();

                assemblyLineHelper.LoadOperations(context, response.Table, IQ.Entities.VastDB.Const.Actions.ACTION_NEW, ref useTableAssemblyLine, ref useCustomAssemblyLine, ref useOperationList, ref operationList);

                if (useOperationList)
                {
                    RequestDataOperationConfigFactory requestDataOperationConfigFactory = new RequestDataOperationConfigFactory();
                    requestDataOperationConfigFactory.LoadFromEntity(context.Payload);
                    context["IOperationConfigFactory"] = requestDataOperationConfigFactory;
                }

                // Execute the Assembly Line                
                assemblyLineHelper.ExecuteAssemblyLine(context, ref response, operationList, transactionId, instanceGuid, databaseName, response.Table.ID,
                                                        IQ.Entities.VastDB.Const.Actions.ACTION_NEW, useTableAssemblyLine, useCustomAssemblyLine,
                                                        customAssemblyLineName, response.Table.ColumnVisibilityInstanceGUID, response.Table.ColumnVisibilityDatabaseName,
                                                        response.Table.ColumnVisibilityTableName, response.Table.TableSchemaInstanceGUID,
                                                        response.Table.TableSchemaDatabaseName, response.Table.TableSchemaTableName);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(ex);
            }
            finally
            {
                IQ.BUS.Vast.Common.Helpers.EventLogHelper.ExitAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, response.Table.ID, IQ.Entities.VastDB.Const.Actions.ACTION_NEW,
                                                                            startTime, DateTime.Now, context.Payload, response, useTableAssemblyLine, useCustomAssemblyLine, customAssemblyLineName,
                                                                            response.Table.ColumnVisibilityInstanceGUID, response.Table.ColumnVisibilityDatabaseName, response.Table.ColumnVisibilityTableName,
                                                                            response.Table.TableSchemaInstanceGUID, response.Table.TableSchemaDatabaseName, response.Table.TableSchemaTableName);
            }

            return response;
        }
    }
}

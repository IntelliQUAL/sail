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

namespace IQ.BUS.VAST.AssemblyLines
{
    public class Update : IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>
    {
        /// <summary>
        /// Created:        03/03/2015
        /// Author:         David J. McKee
        /// Purpose:        Allows for updating an existing table row.
        /// <returns></returns>
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

                CommonConcerns.InitAssemblyLineContext(context, Actions.ACTION_UPDATE, out instanceGuid, out moduleId, out customAssemblyLineName, out transactionId, out operationNameList);

                IQ.BUS.Vast.Common.Helpers.EventLogHelper.EnterAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, context.Payload.TableID, Actions.ACTION_UPDATE, startTime,
                                                                                context.Payload, response, customAssemblyLineName);

                if (context.Payload.Table == null)
                {
                    context.Payload.Table = new IQ.Entities.VastMetaDB.Table();
                    context.Payload.Table.ID = context.Payload.TableID;
                }

                CommonConcerns.InitRequestAndResponse(context.Payload.TableID, Actions.ACTION_UPDATE, context.Payload, response);

                // Append all root schema values to the Request table
                string databaseName = context[IQ.Entities.VastDB.Const.Context.DATABASE_ID].ToString();
                MetaDataHelper.AppendTableMetaData(context, databaseName, context.Payload.Table, IQ.Entities.VastDB.Const.Actions.ACTION_UPDATE);

                List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList =
                    new List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>>();

                AssemblyLineHelper<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> assemblyLineHelper = new AssemblyLineHelper<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>();

                assemblyLineHelper.LoadOperations(context, response.Table, IQ.Entities.VastDB.Const.Actions.ACTION_UPDATE, ref useTableAssemblyLine, ref useCustomAssemblyLine, ref useOperationList, ref operationList);

                if (useOperationList)
                {
                    RequestDataOperationConfigFactory requestDataOperationConfigFactory = new RequestDataOperationConfigFactory();
                    requestDataOperationConfigFactory.LoadFromEntity(context.Payload);
                    context["IOperationConfigFactory"] = requestDataOperationConfigFactory;
                }

                assemblyLineHelper.ExecuteAssemblyLine(context, ref response, operationList, transactionId, instanceGuid, databaseName, context.Payload.Table.ID,
                                                        IQ.Entities.VastDB.Const.Actions.ACTION_UPDATE, useTableAssemblyLine, useCustomAssemblyLine,
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
                IQ.BUS.Vast.Common.Helpers.EventLogHelper.ExitAssemblyLine(context, this.GetType(), transactionId, instanceGuid, moduleId, context.Payload.Table.ID, IQ.Entities.VastDB.Const.Actions.ACTION_UPDATE,
                                                                            startTime, DateTime.Now, context.Payload, response, useTableAssemblyLine, useCustomAssemblyLine, customAssemblyLineName,
                                                                            context.Payload.Table.ColumnVisibilityInstanceGUID, context.Payload.Table.ColumnVisibilityDatabaseName, context.Payload.Table.ColumnVisibilityTableName,
                                                                            context.Payload.Table.TableSchemaInstanceGUID, context.Payload.Table.TableSchemaDatabaseName, context.Payload.Table.TableSchemaTableName);
            }

            return response;
        }
    }
}
